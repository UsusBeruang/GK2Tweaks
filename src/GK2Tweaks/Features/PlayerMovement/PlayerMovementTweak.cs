using System;
using BepInEx.Configuration;
using GK2.Framework;
using GK2Tweaks.Runtime;
using UnityEngine;

namespace GK2Tweaks.Features.PlayerMovement
{
    internal sealed class PlayerMovementTweak
    {
        private ConfigEntry<bool> _enabled;
        private ConfigEntry<float> _walkMultiplier;
        private ConfigEntry<float> _sprintMultiplier;
        private ConfigEntry<KeyboardShortcut> _sprintKey;
        private ConfigEntry<bool> _toggleSprint;

        private PlayerController _trackedPlayer;
        private float _baselineMultiplier;
        private bool _hasBaseline;
        private bool _sprintLatched;

        public void RegisterSettings(Gk2Settings settings)
        {
            _enabled = settings.AddToggle(
                "Player Movement Tweak",
                "Enabled",
                true,
                "Enable Player Movement Tweak",
                "Apply configurable walk and sprint speed multipliers.",
                order: 0);

            _walkMultiplier = settings.AddFloatSlider(
                "Player Movement Tweak",
                "WalkSpeedMultiplier",
                1.5f,
                1f,
                5f,
                "Walk speed multiplier",
                "Movement speed multiplier while not sprinting.",
                step: 0.1f,
                order: 10);

            _sprintMultiplier = settings.AddFloatSlider(
                "Player Movement Tweak",
                "SprintSpeedMultiplier",
                3f,
                1f,
                10f,
                "Sprint speed multiplier",
                "Movement speed multiplier while sprinting.",
                step: 0.1f,
                order: 20);

            _sprintKey = settings.AddKeybind(
                "Player Movement Tweak",
                "SprintKey",
                new KeyboardShortcut(KeyCode.LeftShift),
                "Sprint key",
                "Key or shortcut used to sprint.",
                order: 30);

            _toggleSprint = settings.AddToggle(
                "Player Movement Tweak",
                "ToggleSprint",
                false,
                "Toggle sprint",
                "Toggle sprint on/off with the sprint key instead of holding it.",
                order: 40);
        }

        public void Tick()
        {
            if (!TryGetPlayer(out PlayerController player))
            {
                ClearSessionState();
                return;
            }

            TrackPlayer(player);

            if (_enabled == null || !_enabled.Value)
            {
                RestoreBaseline(player);
                return;
            }

            bool sprinting = ResolveSprintState();
            player.PhysicalBody.SpeedMultiplier =
                sprinting ? _sprintMultiplier.Value : _walkMultiplier.Value;
        }

        public void ResetSession()
        {
            ClearSessionState();
        }

        private bool ResolveSprintState()
        {
            KeyboardShortcut shortcut = _sprintKey.Value;

            if (!_toggleSprint.Value)
            {
                _sprintLatched = false;
                return shortcut.IsPressed();
            }

            if (shortcut.IsDown())
                _sprintLatched = !_sprintLatched;

            return _sprintLatched;
        }

        private void TrackPlayer(PlayerController player)
        {
            if (ReferenceEquals(_trackedPlayer, player) && _hasBaseline)
                return;

            _trackedPlayer = player;
            _baselineMultiplier = player.PhysicalBody.SpeedMultiplier;
            _hasBaseline = true;
            _sprintLatched = false;
        }

        private void RestoreBaseline(PlayerController player)
        {
            _sprintLatched = false;

            if (_hasBaseline && ReferenceEquals(_trackedPlayer, player))
                player.PhysicalBody.SpeedMultiplier = _baselineMultiplier;
        }

        private void ClearSessionState()
        {
            _trackedPlayer = null;
            _hasBaseline = false;
            _sprintLatched = false;
        }

        private static bool TryGetPlayer(out PlayerController player)
        {
            player = null;

            if (!ActiveGameSession.TryGet(out _))
                return false;

            player = MainGame.PlayerController;
            return player != null && player.PhysicalBody != null;
        }
    }
}
