using System;
using System.Collections.Generic;
using HarmonyLib;
using LazyBearTechnology;
using UnityEngine;

namespace GK2Tweaks.Features.InventoryStorage
{
    internal static class InventoryStoragePatchSet
    {
        internal static void Apply(Harmony harmony)
        {
            Type[] patchTypes =
            {
                typeof(ItemStackBalancePatch),
                typeof(PlayerInventoryPreparePatch),
                typeof(PlayerInventorySavePatch),
                typeof(OverheadCarryLimitPatch),
                typeof(GroundStackMergePatch),
                typeof(GroundStackMergeDelayPatch),
                typeof(GroundStackAbsorbPatch),
                typeof(GroundStackVisualPatch),
                typeof(GroundStackDestroyPatch),
                typeof(GroundStackPickupPatch),
                typeof(ContainerDefinitionPatch),
                typeof(ContainerPreparePatch),
                typeof(ContainerWidgetPatch),
                typeof(ContainerBindingsPatch),
                typeof(ContainerDeinitPatch)
            };

            foreach (Type patchType in patchTypes)
                harmony.CreateClassProcessor(patchType).Patch();
        }
    }

    [HarmonyPatch(typeof(GameBalance), "LoadGameBalance")]
    internal static class ItemStackBalancePatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            InventoryStorageRuntime.Current?.ItemStacks.OnGameBalanceLoaded();
        }
    }

    [HarmonyPatch(typeof(PlayerData), "PrepareForGame")]
    internal static class PlayerInventoryPreparePatch
    {
        [HarmonyPostfix]
        private static void Postfix(PlayerData __instance)
        {
            InventoryStorageRuntime.Current?.PlayerInventory.OnPlayerPrepared(__instance);
        }
    }

    [HarmonyPatch(typeof(GameSave), "PrepareToSave")]
    internal static class PlayerInventorySavePatch
    {
        [HarmonyPrefix]
        private static void Prefix(GameSave __instance)
        {
            InventoryStorageRuntime.Current?.PlayerInventory.BeforeGameSave(__instance);
        }
    }

    [HarmonyPatch(typeof(PlayerData), nameof(PlayerData.OverheadStackLimit), MethodType.Getter)]
    internal static class OverheadCarryLimitPatch
    {
        [HarmonyPostfix]
        private static void Postfix(ref int __result)
        {
            BigItemTweak tweak = InventoryStorageRuntime.Current?.BigItems;
            if (tweak?.Enabled == true)
                __result = tweak.CarryLimit;
        }
    }

    [HarmonyPatch(typeof(DropData), "TryAddDropItemPartial")]
    internal static class GroundStackMergePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(DropData __instance, DropData drop, ref bool __result)
        {
            BigItemTweak tweak = InventoryStorageRuntime.Current?.BigItems;
            if (tweak == null
                || !tweak.CanStackOnGround(__instance)
                || !tweak.CanStackOnGround(drop)
                || __instance.Id != drop.Id
                || (int)__instance.DropType == 1
                || (int)drop.DropType == 1)
            {
                return true;
            }

            __instance.Item.Count += drop.Item.Count;
            drop.Item.Count = 0;
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(DropView), "MergeDelayCoroutine")]
    internal static class GroundStackMergeDelayPatch
    {
        [HarmonyPrefix]
        private static void Prefix(DropView __instance)
        {
            InventoryStorageRuntime.Current?.BigItems.SetFastMergeDelay(__instance);
        }
    }

    [HarmonyPatch(typeof(DropView), "ShouldAbsorbFrom")]
    internal static class GroundStackAbsorbPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(DropView __instance, DropView other, ref bool __result)
        {
            BigItemTweak tweak = InventoryStorageRuntime.Current?.BigItems;
            if (tweak == null
                || __instance?.Data == null
                || other?.Data == null
                || !tweak.CanStackOnGround(__instance.Data)
                || !tweak.CanStackOnGround(other.Data)
                || __instance.Data.Id != other.Data.Id
                || (int)__instance.Data.DropType == 1
                || (int)other.Data.DropType == 1)
            {
                return true;
            }

            int countComparison =
                __instance.Data.Count.CompareTo(other.Data.Count);

            if (countComparison != 0)
            {
                __result = countComparison > 0;
                return false;
            }

            __result = __instance.GetInstanceID() < other.GetInstanceID();
            return false;
        }
    }

    [HarmonyPatch(typeof(DropView), "UpdateTextSprite")]
    internal static class GroundStackVisualPatch
    {
        [HarmonyPostfix]
        private static void Postfix(DropView __instance)
        {
            InventoryStorageRuntime.Current?.BigItems.RefreshGroundStackBubble(__instance);
        }
    }

    [HarmonyPatch(typeof(DropView), "OnDestroy")]
    internal static class GroundStackDestroyPatch
    {
        [HarmonyPrefix]
        private static void Prefix(DropView __instance)
        {
            InventoryStorageRuntime.Current?.BigItems.HideGroundStackBubble(__instance);
        }
    }

    [HarmonyPatch(typeof(BigDropInteractionHandler), "Interact")]
    internal static class GroundStackPickupPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(BigDropInteractionHandler __instance, ref bool __result)
        {
            BigItemTweak tweak = InventoryStorageRuntime.Current?.BigItems;
            return tweak == null || tweak.TryHandleGroundStackPickup(__instance, ref __result);
        }
    }

    [HarmonyPatch(typeof(WgoData), "SetDataFromDefinition")]
    internal static class ContainerDefinitionPatch
    {
        [HarmonyPostfix]
        private static void Postfix(WgoData __instance)
        {
            InventoryStorageRuntime.Current?.Containers.OnStoragePrepared(__instance);
        }
    }

    [HarmonyPatch(typeof(WgoData), "PrepareForGame")]
    internal static class ContainerPreparePatch
    {
        [HarmonyPostfix]
        private static void Postfix(WgoData __instance)
        {
            InventoryStorageRuntime.Current?.Containers.OnStoragePrepared(__instance);
        }
    }

    [HarmonyPatch(typeof(Wgo), "GetWidgetData")]
    internal static class ContainerWidgetPatch
    {
        [HarmonyPostfix]
        private static void Postfix(
            Wgo __instance,
            ref List<LazyWidgetDataBase> __result)
        {
            InventoryStorageRuntime.Current?.Containers.AddCapacityWidget(
                __instance,
                ref __result);
        }
    }

    [HarmonyPatch(typeof(Wgo), "InitDataBindings")]
    internal static class ContainerBindingsPatch
    {
        [HarmonyPostfix]
        private static void Postfix(Wgo __instance)
        {
            InventoryStorageRuntime.Current?.Containers.BindWidgetRefresh(__instance);
        }
    }

    [HarmonyPatch(typeof(Wgo), "DeInit")]
    internal static class ContainerDeinitPatch
    {
        [HarmonyPrefix]
        private static void Prefix(Wgo __instance)
        {
            InventoryStorageRuntime.Current?.Containers.UnbindWidgetRefresh(__instance);
        }
    }
}
