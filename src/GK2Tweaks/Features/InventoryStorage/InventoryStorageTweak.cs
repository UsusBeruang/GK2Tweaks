using GK2.Framework;
using HarmonyLib;

namespace GK2Tweaks.Features.InventoryStorage
{
    internal sealed class InventoryStorageTweak
    {
        private const string HarmonyId = Plugin.PluginGuid + ".inventory-storage";

        private readonly Gk2ModLogger _log;
        private readonly ItemStackTweak _itemStacks;
        private readonly BigItemTweak _bigItems;
        private readonly PlayerInventoryTweak _playerInventory;
        private readonly ContainerTweak _containers;

        private Harmony _harmony;

        internal InventoryStorageTweak(Gk2ModLogger log)
        {
            _log = log;
            _itemStacks = new ItemStackTweak(log);
            _bigItems = new BigItemTweak(log);
            _playerInventory = new PlayerInventoryTweak(log);
            _containers = new ContainerTweak(log);
        }

        internal ItemStackTweak ItemStacks => _itemStacks;
        internal BigItemTweak BigItems => _bigItems;
        internal PlayerInventoryTweak PlayerInventory => _playerInventory;
        internal ContainerTweak Containers => _containers;

        public void RegisterSettings(Gk2Settings settings)
        {
            _itemStacks.RegisterSettings(settings);
            _bigItems.RegisterSettings(settings);
            _playerInventory.RegisterSettings(settings);
            _containers.RegisterSettings(settings);
        }

        public void Enable()
        {
            if (_harmony != null)
                return;

            InventoryStorageRuntime.Current = this;

            _harmony = new Harmony(HarmonyId);
            InventoryStoragePatchSet.Apply(_harmony);

            _itemStacks.ApplyRuntimeIncrease();
            _playerInventory.ApplyActivePlayerCapacity();
            _containers.ApplyTrackedCapacities();

            _log.Info("Inventory & Storage Tweak enabled.");
        }

        public void Disable()
        {
            _bigItems.ResetSession();
            _containers.ResetSession();

            _harmony?.UnpatchSelf();
            _harmony = null;

            if (ReferenceEquals(InventoryStorageRuntime.Current, this))
                InventoryStorageRuntime.Current = null;

            _log.Info("Inventory & Storage Tweak disabled.");
        }

        public void Tick()
        {
            _bigItems.Tick();
            _playerInventory.ApplyActivePlayerCapacity();
            _containers.ApplyTrackedCapacities();
        }

        public void ResetSession()
        {
            _bigItems.ResetSession();
            _playerInventory.ResetSession();
            _containers.ResetSession();
        }
    }

    internal static class InventoryStorageRuntime
    {
        internal static InventoryStorageTweak Current { get; set; }
    }
}
