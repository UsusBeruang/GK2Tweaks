using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using GK2.Framework;
using HarmonyLib;
using LazyBearTechnology;
using UnityEngine;

namespace GK2Tweaks.Features.InventoryStorage
{
    internal sealed class BigItemTweak
    {
        private static readonly HashSet<string> StackableGroundItemIds =
            new HashSet<string>
            {
                "wood",
                "box_fabric",
                "box_iron_ingot",
                "box_clothes_1"
            };

        private readonly Gk2ModLogger _log;
        private readonly Dictionary<DropView, GroundStackCounterBubble> _bubbles =
            new Dictionary<DropView, GroundStackCounterBubble>();

        private ConfigEntry<bool> _enabled;
        private ConfigEntry<int> _carryLimit;
        private ConfigEntry<KeyboardShortcut> _dropAllKey;

        internal BigItemTweak(Gk2ModLogger log)
        {
            _log = log;
        }

        internal bool Enabled => _enabled != null && _enabled.Value;
        internal int CarryLimit => _carryLimit?.Value ?? 1;

        public void RegisterSettings(Gk2Settings settings)
        {
            _enabled = settings.AddToggle(
                "Inventory & Storage - Big Items",
                "Enabled",
                true,
                "Enable Big Item Stacking Tweak",
                "Allow supported Big Items to stack on the ground and increase the overhead carry limit.",
                order: 0);

            _carryLimit = settings.AddIntSlider(
                "Inventory & Storage - Big Items",
                "CarryLimit",
                10,
                1,
                100,
                "Big Item carry limit",
                "Maximum number of Big Items the player can carry overhead.",
                step: 1,
                order: 10);

            _dropAllKey = settings.AddKeybind(
                "Inventory & Storage - Big Items",
                "DropAllKey",
                new KeyboardShortcut(KeyCode.F8),
                "Drop all key",
                "Drop all currently carried Big Items.",
                order: 20);
        }

        internal void Tick()
        {
            if (!Enabled || _dropAllKey == null || !IsShortcutDown(_dropAllKey.Value))
                return;

            PlayerData player = MainGame.PlayerData;
            if (player == null)
                return;

            while (player.HasOverheadItem)
                player.DropOverheadItem();
        }

        internal bool CanStackOnGround(DropData drop)
        {
            if (!Enabled || drop == null || (int)drop.Size != 2)
                return false;

            Item item = drop.Item;
            if (item == null)
                return false;

            ItemDef definition =
                ((ObjectLinkedToDefinition<ItemDef>)(object)item).Definition;

            if (definition == null)
                return false;

            string id = ((BalanceBaseObject)definition).id;
            return StackableGroundItemIds.Contains(id);
        }

        internal bool TryHandleGroundStackPickup(
            BigDropInteractionHandler handler,
            ref bool result)
        {
            if (!Enabled || handler == null)
                return true;

            DropView view = Traverse.Create(handler)
                .Field("drop")
                .GetValue<DropView>();

            if (view == null
                || view.Data == null
                || !CanStackOnGround(view.Data)
                || view.Data.Count <= 1)
            {
                return true;
            }

            PlayerData player = MainGame.PlayerData;
            if (player == null)
                return true;

            int free = player.OverheadStackLimit - player.OverheadCount;
            if (free <= 0)
            {
                result = true;
                return false;
            }

            Item stackedItem = view.Data.Item;
            int pickupCount = Mathf.Min(stackedItem.Count, free);

            for (int i = 0; i < pickupCount; i++)
                player.AddOverheadItem(stackedItem.Split(1, false));

            if (stackedItem.Count <= 0)
            {
                MainGame.Instance?.dropSystem?.RemoveDrop(
                    view.Data,
                    view.Data.WorldId);
            }
            else
            {
                Traverse.Create(view)
                    .Method("UpdateTextSprite", Array.Empty<object>())
                    .GetValue();
            }

            result = true;
            return false;
        }

        internal void SetFastMergeDelay(DropView view)
        {
            if (view?.Data == null || !CanStackOnGround(view.Data))
                return;

            Traverse.Create(view)
                .Field("mergeDelayTime")
                .SetValue(0.1f);
        }

        internal void RefreshGroundStackBubble(DropView view)
        {
            if (view?.Data == null || !CanStackOnGround(view.Data) || view.Data.Count <= 1)
            {
                HideGroundStackBubble(view);
                return;
            }

            if (!_bubbles.TryGetValue(view, out GroundStackCounterBubble bubble))
            {
                bubble = new GroundStackCounterBubble(view);
                _bubbles.Add(view, bubble);
            }

            bubble.Refresh();
            UIObjectBubbleManager.Instance?.RequestDisplay(bubble);
        }

        internal void HideGroundStackBubble(DropView view)
        {
            if (view == null || !_bubbles.TryGetValue(view, out GroundStackCounterBubble bubble))
                return;

            UIObjectBubbleManager.Instance?.Hide(bubble);
            _bubbles.Remove(view);
        }

        internal void ResetSession()
        {
            UIObjectBubbleManager manager = UIObjectBubbleManager.Instance;
            if (manager != null)
            {
                foreach (GroundStackCounterBubble bubble in _bubbles.Values)
                    manager.Hide(bubble);
            }

            _bubbles.Clear();
        }

        private static bool IsShortcutDown(KeyboardShortcut shortcut)
        {
            KeyCode mainKey = shortcut.MainKey;
            if (mainKey == KeyCode.None || !Input.GetKeyDown(mainKey))
                return false;

            foreach (KeyCode modifier in shortcut.Modifiers)
            {
                if (!Input.GetKey(modifier))
                    return false;
            }

            return true;
        }
    }
}
