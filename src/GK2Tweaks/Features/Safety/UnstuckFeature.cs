using System;
using BepInEx.Configuration;
using GK2.Framework;
using GK2Tweaks.Runtime;
using UnityEngine;

namespace GK2Tweaks.Features.Safety
{
    internal sealed class UnstuckFeature
    {
        private const int UnstuckPointInstanceId = 2143638924;

        private ConfigEntry<bool> _enabled;
        private ConfigEntry<KeyboardShortcut> _hotkey;

        public void RegisterSettings(Gk2Settings settings)
        {
            _enabled = settings.AddToggle(
                "Safety",
                "UnstuckEnabled",
                true,
                "Enable Unstuck",
                "Allow a shortcut that moves the player to a known safe point when stuck.",
                order: 0);

            _hotkey = settings.AddKeybind(
                "Safety",
                "UnstuckKey",
                new KeyboardShortcut(
                    KeyCode.T,
                    KeyCode.LeftControl,
                    KeyCode.LeftAlt),
                "Unstuck key",
                "Move the player to the known safe point. Default: Ctrl+Alt+T.",
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

            MovePlayerToSafePoint();
        }

        private static void MovePlayerToSafePoint()
        {
            if (!ActiveGameSession.TryGet(out MainGame game))
                return;

            GdPointsData points = game.GameSave?.worldData?.gdPointsData;
            if (points == null || MainGame.PlayerController == null)
                return;

            GDPointData destination =
                points.GetGDPointDataByInstanceId(UnstuckPointInstanceId);

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
