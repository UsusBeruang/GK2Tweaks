using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using GK2.Framework;
using LazyBearTechnology;

namespace GK2Tweaks.Features.InventoryStorage
{
    internal sealed class ContainerTweak
    {
        private const string BigItemStorageId = "wood_container";

        private readonly Gk2ModLogger _log;
        private readonly HashSet<WgoData> _trackedStorages = new HashSet<WgoData>();
        private readonly Dictionary<Wgo, Action<List<Item>>> _widgetRefreshHandlers =
            new Dictionary<Wgo, Action<List<Item>>>();

        private ConfigEntry<bool> _enabled;
        private ConfigEntry<int> _capacityMultiplier;
        private ConfigEntry<bool> _prepareForUninstall;

        private readonly HashSet<string> _blockedWarnings = new HashSet<string>();

        internal ContainerTweak(Gk2ModLogger log)
        {
            _log = log;
        }

        public void RegisterSettings(Gk2Settings settings)
        {
            _enabled = settings.AddToggle(
                "Inventory & Storage - Containers",
                "Enabled",
                true,
                "Enable Container Tweak",
                "Increase supported Big Item storage capacities.",
                order: 0);

            _capacityMultiplier = settings.AddIntSlider(
                "Inventory & Storage - Containers",
                "BigItemStorageMultiplier",
                3,
                1,
                100,
                "Big Item storage multiplier",
                "Multiplier applied to supported Big Item storage capacity.",
                step: 1,
                order: 10);

            _prepareForUninstall = settings.AddToggle(
                "Inventory & Storage - Containers",
                "PrepareForUninstall",
                false,
                "Prepare containers for uninstall",
                "Restore vanilla Big Item storage capacity. Excess contents are dropped beside the storage when necessary.",
                order: 20);

            _enabled.SettingChanged += (_, __) => ApplyTrackedCapacities();
            _capacityMultiplier.SettingChanged += (_, __) => ApplyTrackedCapacities();
            _prepareForUninstall.SettingChanged += (_, __) => ApplyTrackedCapacities();
        }

        internal void OnStoragePrepared(WgoData data)
        {
            if (!IsSupportedStorage(data))
                return;

            _trackedStorages.Add(data);
            ApplyCapacity(data);
        }

        internal void ApplyTrackedCapacities()
        {
            foreach (WgoData storage in _trackedStorages.ToArray())
            {
                if (storage == null)
                {
                    _trackedStorages.Remove(storage);
                    continue;
                }

                ApplyCapacity(storage);
            }
        }

        internal void AddCapacityWidget(Wgo view, ref List<LazyWidgetDataBase> widgets)
        {
            WgoData data = view?.Data;
            if (data == null
                || data.IsHidden
                || widgets == null
                || !IsSupportedStorage(data))
            {
                return;
            }

            int count = CountStoredBigItems(data);
            if (count <= 0)
                return;

            LazyWidgetDataBase widget =
                TooltipTextFactory.CreateCenteredAccentText(
                    $"{count}/{data.Inventory.Data.InventorySize}");

            if (widget != null)
                widgets.Add(widget);
        }

        internal void BindWidgetRefresh(Wgo view)
        {
            WgoData data = view?.Data;
            if (data?.Inventory == null
                || data.Inventory.Data == null
                || !IsSupportedStorage(data)
                || _widgetRefreshHandlers.ContainsKey(view))
            {
                return;
            }

            Action<List<Item>> handler = _ => view.DrawWidgets();
            _widgetRefreshHandlers.Add(view, handler);

            data.Inventory.OnItemsAdd += handler;
            data.Inventory.OnItemsRemove += handler;
        }

        internal void UnbindWidgetRefresh(Wgo view)
        {
            if (view == null
                || !_widgetRefreshHandlers.TryGetValue(view, out Action<List<Item>> handler))
            {
                return;
            }

            WgoData data = view.Data;
            if (data?.Inventory != null)
            {
                data.Inventory.OnItemsAdd -= handler;
                data.Inventory.OnItemsRemove -= handler;
            }

            _widgetRefreshHandlers.Remove(view);
        }

        internal void ResetSession()
        {
            foreach (KeyValuePair<Wgo, Action<List<Item>>> pair in _widgetRefreshHandlers.ToArray())
            {
                WgoData data = pair.Key?.Data;
                if (data?.Inventory == null)
                    continue;

                data.Inventory.OnItemsAdd -= pair.Value;
                data.Inventory.OnItemsRemove -= pair.Value;
            }

            _widgetRefreshHandlers.Clear();
            _trackedStorages.Clear();
            _blockedWarnings.Clear();
        }

        internal bool IsSupportedStorage(WgoData data)
        {
            if (data == null || data.Inventory?.Data == null)
                return false;

            ObjectLinkedToDefinition<WGODef> linked =
                (ObjectLinkedToDefinition<WGODef>)(object)data;

            return linked.Definition != null
                && linked.id == BigItemStorageId;
        }

        private void ApplyCapacity(WgoData data)
        {
            if (!IsSupportedStorage(data) || _enabled == null)
                return;

            ObjectLinkedToDefinition<WGODef> linked =
                (ObjectLinkedToDefinition<WGODef>)(object)data;

            int vanilla = linked.Definition.inventorySize;
            bool explicitRestore = _prepareForUninstall.Value;
            bool restoreVanilla = explicitRestore || !_enabled.Value;
            int target = restoreVanilla
                ? vanilla
                : vanilla * _capacityMultiplier.Value;

            Item inventory = data.Inventory.Data;
            if (inventory.InventorySize == target)
            {
                ClearBlockedWarning(linked.id);
                return;
            }

            if (inventory.InventorySize < target)
            {
                inventory.InventorySize = target;
                ClearBlockedWarning(linked.id);
                return;
            }

            if (inventory.InventoryFillSize > target && explicitRestore)
            {
                if (!EjectExcessItems(data, inventory.InventoryFillSize - target))
                    return;
            }

            if (inventory.InventoryFillSize <= target)
            {
                inventory.InventorySize = target;
                ClearBlockedWarning(linked.id);
                _log.Info($"Big Item storage '{linked.id}' capacity set to {target}.");
                return;
            }

            string warningKey = $"{linked.id}:{target}:{inventory.InventoryFillSize}";
            if (_blockedWarnings.Add(warningKey))
            {
                _log.Warning(
                    $"Big Item storage '{linked.id}' cannot shrink to {target} while "
                    + $"{inventory.InventoryFillSize} capacity is occupied.");
            }
        }

        private bool EjectExcessItems(WgoData data, int amountToRemove)
        {
            if (amountToRemove <= 0)
                return true;

            int remaining = amountToRemove;
            List<Item> snapshot = new List<Item>(data.Inventory.Data.Inventory);

            for (int index = snapshot.Count - 1; index >= 0 && remaining > 0; index--)
            {
                Item source = snapshot[index];
                if (source == null || source.IsEmpty)
                    continue;

                ObjectLinkedToDefinition<ItemDef> linked =
                    (ObjectLinkedToDefinition<ItemDef>)(object)source;

                int requested = Math.Min(source.Count, remaining);
                foreach (Item removed in data.Inventory.RemoveItemById(
                             linked.id,
                             requested,
                             null,
                             null,
                             true))
                {
                    data.MakeDrop(removed);
                    remaining -= removed.Count;

                    if (remaining <= 0)
                        break;
                }
            }

            if (remaining > 0)
            {
                _log.Warning(
                    $"Could not finish restoring Big Item storage capacity; "
                    + $"{remaining} item(s) still need to be removed.");
                return false;
            }

            _log.Info($"Dropped {amountToRemove} excess Big Item(s) while restoring storage.");
            return true;
        }

        private static int CountStoredBigItems(WgoData data)
        {
            int count = 0;

            foreach (Item item in data.Inventory.Data.Inventory)
            {
                if (item == null || item.IsEmpty)
                    continue;

                ItemDef definition =
                    ((ObjectLinkedToDefinition<ItemDef>)(object)item).Definition;

                if (definition != null && (int)definition.itemSize == 2)
                    count += item.Count;
            }

            return count;
        }

        private void ClearBlockedWarning(string storageId)
        {
            _blockedWarnings.RemoveWhere(key => key.StartsWith(storageId + ":", StringComparison.Ordinal));
        }
    }
}
