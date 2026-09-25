using System;
using BepInEx.Configuration;
using GK2.Framework;
using GK2Tweaks.Runtime;
using UnityEngine;

namespace GK2Tweaks.Features.SaveAnywhere
{
    internal sealed class SaveAnywhereFeature
    {
        private readonly Gk2ModLogger _log;

        private ConfigEntry<bool> _enabled;
        private ConfigEntry<KeyboardShortcut> _hotkey;

        public SaveAnywhereFeature(Gk2ModLogger log)
        {
            _log = log;
        }

        public void RegisterSettings(Gk2Settings settings)
        {
            _enabled = settings.AddToggle(
                "Save Anywhere",
                "Enabled",
                true,
                "Enable Save Anywhere",
                "Allow manual saving while normal gameplay is active.",
                order: 0);

            _hotkey = settings.AddKeybind(
                "Save Anywhere",
                "SaveKey",
                new KeyboardShortcut(KeyCode.F5),
                "Save key",
                "Save the current game from anywhere during normal gameplay.",
                order: 10);
        }

        public void Tick()
        {
            if (_enabled == null
                || !_enabled.Value
                || !_hotkey.Value.IsDown())
            {
                return;
            }

            SaveCurrentSession();
        }

        private void SaveCurrentSession()
        {
            if (!ActiveGameSession.TryGetSave(
                    out SaveSlotData slot,
                    out GameSave save))
            {
                _log.Warning("Manual save skipped because no active gameplay save is available.");
                return;
            }

            SaveSystem.Save(
                slot,
                save,
                HandleSaveCompleted,
                HandleSaveFailed,
                true,
                (Action<SaveSlotData, GameSave>)null);
        }

        private void HandleSaveCompleted()
        {
            _log.Info("Manual save completed.");
        }

        private void HandleSaveFailed()
        {
            _log.Error("Manual save failed.");
        }
    }
}
