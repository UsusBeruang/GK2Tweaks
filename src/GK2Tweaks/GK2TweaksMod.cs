using System.Collections.Generic;
using GK2.Framework;

namespace GK2Tweaks
{
    internal sealed class GK2TweaksMod : Gk2ModBase
    {
        private readonly Gk2ModMetadata _metadata =
            new Gk2ModMetadata(
                Plugin.PluginGuid,
                Plugin.PluginName,
                "UsusBeruang",
                Plugin.PluginVersion,
                "Configurable quality-of-life tweaks for Graveyard Keeper 2.",
                supportsRuntimeToggle: false,
                requiresKnownBuild: false);

        private readonly IReadOnlyList<Gk2ModDependency> _dependencies =
            new[]
            {
                new Gk2ModDependency(
                    FrameworkPlugin.PluginGuid,
                    "0.1.12",
                    "0.2.0")
            };

        private Gk2ModLogger _log;

        public override Gk2ModMetadata Metadata => _metadata;

        public override IReadOnlyList<Gk2ModDependency> Dependencies =>
            _dependencies;

        public override void OnRegister(Gk2ModContext context)
        {
            _log = context.Log;

            _log.Info("GK2_TWEAKS_REGISTERED");
        }

        public override void OnEnable()
        {
            _log.Info("GK2_TWEAKS_ENABLED");
        }

        public override void OnDisable()
        {
            _log.Info("GK2_TWEAKS_DISABLED");
        }

        public override void OnGameStarted()
        {
        }

        public override void OnReturnedToMainMenu()
        {
        }
    }
}