using System;
using BepInEx.Configuration;
using GK2.Framework;
using UnityEngine;

namespace GK2Tweaks.Features.SaveAnywhere
{
    internal sealed class SaveAnywhereFeature
    {
        private const int GameplayStateValue = 1;
        private const int SafePointInstanceId = 2143638924;

        private readonly Gk2ModLogger _log;

        private ConfigEntry<bool> _saveAnywhereEnabled;
        private ConfigEntry<KeyboardShortcut> _saveKey;
        private ConfigEntry<bool> _emergencyTeleportEnabled;
        private ConfigEntry<KeyboardShortcut> _emergencyTeleportKey;

        public SaveAnywhereFeature(Gk2ModLogger log)
        {
            _log = log;
        }

        public void RegisterSettings(Gk2Settings settings)
        {
            _saveAnywhereEnabled = settings.AddToggle(
                "Save Anywhere",
                "Enabled",
                true,
                "Enable Save Anywhere",
                "Allow manual saving while normal gameplay is active.",
                order: 0);

            _saveKey = settings.AddKeybind(
                "Save Anywhere",
                "SaveKey",
                new KeyboardShortcut(KeyCode.F5),
                "Save key",
                "Save the current game from anywhere during normal gameplay.",
                order: 10);

            _emergencyTeleportEnabled = settings.AddToggle(
                "Safety",
                "EmergencyTeleportEnabled",
                true,
                "Enable emergency teleport",
                "Allow the emergency teleport shortcut if the player becomes stuck.",
                order: 0);

            _emergencyTeleportKey = settings.AddKeybind(
                "Safety",
                "EmergencyTeleportKey",
                new KeyboardShortcut(
                    KeyCode.T,
                    KeyCode.LeftControl,
                    KeyCode.LeftAlt),
                "Emergency teleport key",
                "Teleport to the verified safe point. Default: Ctrl+Alt+T.",
                order: 10);
        }

        public void Tick()
        {
            if (_emergencyTeleportEnabled != null
                && _emergencyTeleportEnabled.Value
                && _emergencyTeleportKey.Value.IsDown())
            {
                TeleportToSafety();
            }

            if (_saveAnywhereEnabled == null
                || !_saveAnywhereEnabled.Value
                || !_saveKey.Value.IsDown())
            {
                return;
            }

            TrySaveGame();
        }

        private void TrySaveGame()
        {
            MainGame mainGame = MainGame.Instance;
            if (mainGame == null || (int)mainGame.gameState != GameplayStateValue)
                return;

            GameSave gameSave = mainGame.GameSave;
            SaveSlotData saveSlotData = mainGame.SaveSlotData;

            if (gameSave == null || saveSlotData == null)
            {
                _log.Warning("Unable to save - no active game.");
                return;
            }

            SaveSystem.Save(
                saveSlotData,
                gameSave,
                OnSaveSucceeded,
                OnSaveFailed,
                true,
                (Action<SaveSlotData, GameSave>)null);
        }

        private void TeleportToSafety()
        {
            MainGame mainGame = MainGame.Instance;
            if (mainGame == null || (int)mainGame.gameState != GameplayStateValue)
                return;

            GameSave gameSave = mainGame.GameSave;
            if (gameSave?.worldData?.gdPointsData == null
                || MainGame.PlayerController == null)
            {
                return;
            }

            GDPointData safePoint =
                gameSave.worldData.gdPointsData.GetGDPointDataByInstanceId(
                    SafePointInstanceId);

            if (safePoint == null)
                return;

            var teleportData = new GDPointTeleportData(
                safePoint,
                "outdoor",
                string.Empty,
                (Action)null,
                false,
                0.3f);

            PlayerController.Teleport(teleportData);
        }

        private void OnSaveSucceeded()
        {
            _log.Info("Game saved successfully.");
        }

        private void OnSaveFailed()
        {
            _log.Error("Failed to save game.");
        }
    }
}
