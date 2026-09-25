using System;
using LazyBearTechnology;

namespace GK2Tweaks.Compatibility
{
    internal static class FeatureContractVerifier
    {
        public static void VerifySaveAnywhere()
        {
            ReflectionContract.RequireProperty(
                typeof(MainGame),
                "Instance",
                typeof(MainGame),
                isStatic: true);

            ReflectionContract.RequireProperty(
                typeof(MainGame),
                "gameState",
                typeof(MainGame.GameState),
                isStatic: false);

            ReflectionContract.RequireProperty(
                typeof(MainGame),
                "GameSave",
                typeof(GameSave),
                isStatic: false);

            ReflectionContract.RequireProperty(
                typeof(MainGame),
                "SaveSlotData",
                typeof(SaveSlotData),
                isStatic: false);

            ReflectionContract.RequireMethod(
                typeof(SaveSystem),
                "Save",
                typeof(void),
                isStatic: true,
                typeof(SaveSlotData),
                typeof(GameSave),
                typeof(Action),
                typeof(Action),
                typeof(bool),
                typeof(Action<SaveSlotData, GameSave>));
        }

        public static void VerifyPlayerMovementTweak()
        {
            ReflectionContract.RequireProperty(
                typeof(MainGame),
                "PlayerController",
                typeof(PlayerController),
                isStatic: true);

            var physicalBody = ReflectionContract.RequireProperty(
                typeof(PlayerController),
                "PhysicalBody",
                isStatic: false);

            ReflectionContract.RequireWritableProperty(
                physicalBody.PropertyType,
                "SpeedMultiplier",
                typeof(float),
                isStatic: false);
        }

        public static void VerifyInventoryStorageTweak()
        {
            ReflectionContract.RequireProperty(
                typeof(GameBalance),
                "Me",
                typeof(GameBalance),
                isStatic: true);

            ReflectionContract.RequireField(
                typeof(GameBalance),
                "itemDefs",
                isStatic: false);

            ReflectionContract.RequireField(
                typeof(ItemDef),
                "stackCount",
                typeof(int),
                isStatic: false);

            ReflectionContract.RequireMethodNamed(
                typeof(GameBalance),
                "LoadGameBalance");

            ReflectionContract.RequireProperty(
                typeof(MainGame),
                "PlayerData",
                typeof(PlayerData),
                isStatic: true);

            ReflectionContract.RequireProperty(
                typeof(PlayerData),
                "Inventory",
                typeof(Inventory),
                isStatic: false);

            ReflectionContract.RequireProperty(
                typeof(Inventory),
                "Data",
                typeof(Item),
                isStatic: false);

            ReflectionContract.RequireWritableProperty(
                typeof(Item),
                "InventorySize",
                typeof(int),
                isStatic: false);

            ReflectionContract.RequireProperty(
                typeof(Item),
                "InventoryFillSize",
                typeof(int),
                isStatic: false);

            ReflectionContract.RequireMethodNamed(
                typeof(PlayerData),
                "PrepareForGame");

            ReflectionContract.RequireField(
                typeof(GameSave),
                "playerData",
                typeof(PlayerData),
                isStatic: false);

            ReflectionContract.RequireField(
                typeof(GameSave),
                "perkSystemData",
                typeof(PerkSystemData),
                isStatic: false);

            ReflectionContract.RequireMethod(
                typeof(PerkSystemData),
                "HasPerk",
                typeof(bool),
                isStatic: false,
                typeof(string));

            ReflectionContract.RequireMethodNamed(
                typeof(GameSave),
                "PrepareToSave");

            ReflectionContract.RequireProperty(
                typeof(PlayerData),
                "OverheadStackLimit",
                typeof(int),
                isStatic: false);

            ReflectionContract.RequireProperty(
                typeof(PlayerData),
                "OverheadCount",
                typeof(int),
                isStatic: false);

            ReflectionContract.RequireProperty(
                typeof(PlayerData),
                "HasOverheadItem",
                typeof(bool),
                isStatic: false);

            ReflectionContract.RequireMethodNamed(
                typeof(PlayerData),
                "DropOverheadItem");

            ReflectionContract.RequireMethodNamed(
                typeof(PlayerData),
                "AddOverheadItem");

            ReflectionContract.RequireMethodNamed(
                typeof(DropData),
                "TryAddDropItemPartial");

            ReflectionContract.RequireField(
                typeof(DropView),
                "mergeDelayTime",
                typeof(float),
                isStatic: false);

            ReflectionContract.RequireMethodNamed(
                typeof(DropView),
                "MergeDelayCoroutine");

            ReflectionContract.RequireMethodNamed(
                typeof(DropView),
                "ShouldAbsorbFrom");

            ReflectionContract.RequireMethodNamed(
                typeof(DropView),
                "UpdateTextSprite");

            ReflectionContract.RequireMethodNamed(
                typeof(DropView),
                "OnDestroy");

            ReflectionContract.RequireField(
                typeof(BigDropInteractionHandler),
                "drop",
                typeof(DropView),
                isStatic: false);

            ReflectionContract.RequireMethodNamed(
                typeof(BigDropInteractionHandler),
                "Interact");

            ReflectionContract.RequireField(
                typeof(WGODef),
                "inventorySize",
                typeof(int),
                isStatic: false);

            ReflectionContract.RequireProperty(
                typeof(WgoData),
                "Inventory",
                typeof(Inventory),
                isStatic: false);

            ReflectionContract.RequireProperty(
                typeof(Wgo),
                "Data",
                typeof(WgoData),
                isStatic: false);

            ReflectionContract.RequireMethodNamed(
                typeof(WgoData),
                "SetDataFromDefinition");

            ReflectionContract.RequireMethodNamed(
                typeof(WgoData),
                "PrepareForGame");

            ReflectionContract.RequireMethodNamed(
                typeof(WgoData),
                "MakeDrop");

            ReflectionContract.RequireEvent(
                typeof(Inventory),
                "OnItemsAdd");

            ReflectionContract.RequireEvent(
                typeof(Inventory),
                "OnItemsRemove");

            ReflectionContract.RequireMethodNamed(
                typeof(Wgo),
                "GetWidgetData");

            ReflectionContract.RequireMethodNamed(
                typeof(Wgo),
                "InitDataBindings");

            ReflectionContract.RequireMethodNamed(
                typeof(Wgo),
                "DeInit");

            ReflectionContract.RequireMethodNamed(
                typeof(Wgo),
                "DrawWidgets");

            ReflectionContract.RequireField(
                typeof(UITooltip),
                "instance",
                typeof(UITooltip),
                isStatic: true);

            ReflectionContract.RequireField(
                typeof(UITooltip),
                "headerTextStyle",
                typeof(TextStyle),
                isStatic: false);

            ReflectionContract.RequireField(
                typeof(UITooltip),
                "headerBoldTextStyleGold",
                typeof(TextStyle),
                isStatic: false);
        }

        public static void VerifyUnstuck()
        {
            ReflectionContract.RequireProperty(
                typeof(MainGame),
                "PlayerController",
                typeof(PlayerController),
                isStatic: true);

            ReflectionContract.RequireField(
                typeof(GameSave),
                "worldData",
                typeof(WorldData),
                isStatic: false);

            ReflectionContract.RequireField(
                typeof(WorldData),
                "gdPointsData",
                typeof(GdPointsData),
                isStatic: false);

            ReflectionContract.RequireMethod(
                typeof(GdPointsData),
                "GetGDPointDataByInstanceId",
                typeof(GDPointData),
                isStatic: false,
                typeof(int));

            ReflectionContract.RequireConstructor(
                typeof(GDPointTeleportData),
                typeof(GDPointData),
                typeof(string),
                typeof(string),
                typeof(Action),
                typeof(bool),
                typeof(float));

            ReflectionContract.RequireMethod(
                typeof(PlayerController),
                "Teleport",
                typeof(bool),
                isStatic: true,
                typeof(TeleportDataBase));
        }
    }
}
