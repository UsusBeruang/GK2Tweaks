using System;
using BepInEx.Configuration;
using GK2.Framework;
using GK2Tweaks.Runtime;
using UnityEngine;

namespace GK2Tweaks.Features.Safety
{
    internal sealed class EmergencyRecoveryFeature
    {
        private const int RecoveryPointInstanceId = 2143638924;

        private ConfigEntry<bool> _enabled;
        private ConfigEntry<KeyboardShortcut> _hotkey;

        public void RegisterSettings(Gk2Settings settings)
        {
            _enabled = settings.AddToggle(
                "Safety",
                "EmergencyTeleportEnabled",
                true,
                "Enable emergency teleport",
                "Allow a recovery shortcut that moves the player to a known safe point.",
                order: 0);

            _hotkey = settings.AddKeybind(
                "Safety",
                "EmergencyTeleportKey",
                new KeyboardShortcut(
                    KeyCode.T,
                    KeyCode.LeftControl,
                    KeyCode.LeftAlt),
                "Emergency teleport key",
                "Move the player to the known recovery point. Default: Ctrl+Alt+T.",
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

            RecoverPlayer();
        }

        private static void RecoverPlayer()
        {
            if (!ActiveGameSession.TryGet(out MainGame game))
                return;

            GdPointsData points = game.GameSave?.worldData?.gdPointsData;
            if (points == null || MainGame.PlayerController == null)
                return;

            GDPointData destination =
                points.GetGDPointDataByInstanceId(RecoveryPointInstanceId);

            if (destination == null)
                return;

            var request = new GDPointTeleportData(
                destination,
                "outdoor",
                string.Empty,
                (Action)null,
                false,
                0.3f);

            PlayerController.Teleport(request);
        }
    }
}
