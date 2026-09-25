using System;

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
