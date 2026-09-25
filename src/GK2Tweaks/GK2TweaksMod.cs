using System.Collections.Generic;
using GK2.Framework;
using GK2Tweaks.Compatibility;
using GK2Tweaks.Features.Safety;
using GK2Tweaks.Features.SaveAnywhere;

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
                requiresKnownBuild: true);

        private readonly IReadOnlyList<Gk2ModDependency> _dependencies =
            new[]
            {
                new Gk2ModDependency(
                    FrameworkPlugin.PluginGuid,
                    "0.1.12",
                    "0.2.0")
            };

        private Gk2ModLogger _log;
        private SaveAnywhereFeature _saveAnywhere;
        private UnstuckFeature _unstuck;
        private bool _runtimeEnabled;

        public override Gk2ModMetadata Metadata => _metadata;

        public override IReadOnlyList<Gk2ModDependency> Dependencies =>
            _dependencies;

        public override void OnRegister(Gk2ModContext context)
        {
            _log = context.Log;

            FeatureContractVerifier.VerifySaveAnywhere();
            FeatureContractVerifier.VerifyUnstuck();
            context.ConfirmCurrentBuildCompatibility(
                "Manual save and Unstuck contracts are intact.");

            _saveAnywhere = new SaveAnywhereFeature(_log);
            _saveAnywhere.RegisterSettings(context.Settings);

            _unstuck = new UnstuckFeature();
            _unstuck.RegisterSettings(context.Settings);

            _log.Info("GK2_TWEAKS_REGISTERED");
        }

        public override void OnEnable()
        {
            _runtimeEnabled = true;
            _log.Info("GK2_TWEAKS_ENABLED");
        }

        public override void OnDisable()
        {
            _runtimeEnabled = false;
            _log.Info("GK2_TWEAKS_DISABLED");
        }

        public override void OnGameStarted()
        {
        }

        public override void OnReturnedToMainMenu()
        {
        }

        internal void Tick()
        {
            if (!_runtimeEnabled)
                return;

            _saveAnywhere?.Tick();
            _unstuck?.Tick();
        }
    }
}
