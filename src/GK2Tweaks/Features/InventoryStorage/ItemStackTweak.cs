using BepInEx.Configuration;
using GK2.Framework;

namespace GK2Tweaks.Features.InventoryStorage
{
    internal sealed class ItemStackTweak
    {
        private readonly Gk2ModLogger _log;

        private ConfigEntry<bool> _enabled;
        private ConfigEntry<int> _minimumStackSize;

        internal ItemStackTweak(Gk2ModLogger log)
        {
            _log = log;
        }

        internal bool Enabled => _enabled != null && _enabled.Value;

        public void RegisterSettings(Gk2Settings settings)
        {
            _enabled = settings.AddToggle(
                "Inventory & Storage - Item Stacks",
                "Enabled",
                true,
                "Enable Item Stack Tweak",
                "Increase stackable item definitions to at least the configured stack size.",
                order: 0);

            _minimumStackSize = settings.AddIntSlider(
                "Inventory & Storage - Item Stacks",
                "MinimumStackSize",
                999,
                2,
                9999,
                "Minimum stack size",
                "Minimum stack size applied to items that are already stackable.",
                step: 1,
                order: 10);

            _enabled.SettingChanged += (_, __) => ApplyRuntimeIncrease();
            _minimumStackSize.SettingChanged += (_, __) => ApplyRuntimeIncrease();
        }

        internal void OnGameBalanceLoaded()
        {
            ApplyConfiguredMinimum();
        }

        internal void ApplyRuntimeIncrease()
        {
            if (!Enabled)
                return;

            ApplyConfiguredMinimum();
        }

        private void ApplyConfiguredMinimum()
        {
            if (!Enabled || GameBalance.Me?.itemDefs == null)
                return;

            int target = _minimumStackSize.Value;
            int changed = 0;

            foreach (ItemDef definition in GameBalance.Me.itemDefs)
            {
                if (definition == null
                    || definition.stackCount <= 1
                    || definition.stackCount >= target)
                {
                    continue;
                }

                definition.stackCount = target;
                changed++;
            }

            if (changed > 0)
                _log.Debug($"Item Stack Tweak raised {changed} item definition(s) to at least {target}.");
        }
    }
}
