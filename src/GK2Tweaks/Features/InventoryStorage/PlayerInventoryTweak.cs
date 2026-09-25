using System;
using BepInEx.Configuration;
using GK2.Framework;

namespace GK2Tweaks.Features.InventoryStorage
{
    internal sealed class PlayerInventoryTweak
    {
        private const int VanillaInventorySize = 20;
        private const int DeepPocketsBonus = 5;
        private const string DeepPocketsPerkId = "perk_deeppockets";

        private readonly Gk2ModLogger _log;

        private ConfigEntry<bool> _enabled;
        private ConfigEntry<int> _inventorySize;
        private ConfigEntry<bool> _prepareForUninstall;

        private int _lastBlockedTarget = -1;
        private int _lastBlockedFill = -1;

        internal PlayerInventoryTweak(Gk2ModLogger log)
        {
            _log = log;
        }

        public void RegisterSettings(Gk2Settings settings)
        {
            _enabled = settings.AddToggle(
                "Inventory & Storage - Player Inventory",
                "Enabled",
                true,
                "Enable Player Inventory Tweak",
                "Apply the configured minimum player inventory capacity.",
                order: 0);

            _inventorySize = settings.AddIntSlider(
                "Inventory & Storage - Player Inventory",
                "InventorySize",
                40,
                20,
                300,
                "Inventory size",
                "Minimum number of slots in the player's main inventory.",
                step: 1,
                order: 10);

            _prepareForUninstall = settings.AddToggle(
                "Inventory & Storage - Player Inventory",
                "PrepareForUninstall",
                false,
                "Prepare player inventory for uninstall",
                "Restore vanilla capacity when enough slots are free. Deep Pockets is preserved.",
                order: 20);

            _enabled.SettingChanged += (_, __) => ApplyActivePlayerCapacity();
            _inventorySize.SettingChanged += (_, __) => ApplyActivePlayerCapacity();
            _prepareForUninstall.SettingChanged += (_, __) => ApplyActivePlayerCapacity();
        }

        internal void OnPlayerPrepared(PlayerData player)
        {
            ApplyCapacity(player);
        }

        internal void BeforeGameSave(GameSave save)
        {
            if (_prepareForUninstall == null || !_prepareForUninstall.Value)
                return;

            ApplyCapacity(save?.playerData);
        }

        internal void ApplyActivePlayerCapacity()
        {
            ApplyCapacity(MainGame.PlayerData);
        }

        internal void ResetSession()
        {
            _lastBlockedTarget = -1;
            _lastBlockedFill = -1;
        }

        private void ApplyCapacity(PlayerData player)
        {
            Item inventory = player?.Inventory?.Data;
            if (inventory == null || _enabled == null)
                return;

            int vanillaCapacity = GetVanillaCapacity();
            bool restoreVanilla = !_enabled.Value || _prepareForUninstall.Value;
            int target = restoreVanilla
                ? vanillaCapacity
                : Math.Max(vanillaCapacity, _inventorySize.Value);

            if (inventory.InventorySize == target)
            {
                ClearBlockedWarning();
                return;
            }

            if (inventory.InventorySize < target)
            {
                inventory.InventorySize = target;
                ClearBlockedWarning();
                _log.Debug($"Player inventory capacity set to {target}.");
                return;
            }

            int occupied = inventory.InventoryFillSize;
            if (occupied <= target)
            {
                inventory.InventorySize = target;
                ClearBlockedWarning();
                _log.Info($"Player inventory capacity reduced to {target}.");
                return;
            }

            if (_lastBlockedTarget == target && _lastBlockedFill == occupied)
                return;

            _lastBlockedTarget = target;
            _lastBlockedFill = occupied;

            _log.Warning(
                $"Player inventory cannot shrink to {target} while {occupied} slots are occupied. "
                + $"Free at least {occupied - target} more slot(s).");
        }

        private static int GetVanillaCapacity()
        {
            int capacity = VanillaInventorySize;
            GameSave save = MainGame.Instance?.GameSave;

            if (save?.perkSystemData != null
                && save.perkSystemData.HasPerk(DeepPocketsPerkId))
            {
                capacity += DeepPocketsBonus;
            }

            return capacity;
        }

        private void ClearBlockedWarning()
        {
            _lastBlockedTarget = -1;
            _lastBlockedFill = -1;
        }
    }
}
