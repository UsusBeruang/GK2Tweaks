using BepInEx;
using GK2.Framework;

namespace GK2Tweaks
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(
        FrameworkPlugin.PluginGuid,
        BepInDependency.DependencyFlags.HardDependency)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.ususberuang.gk2.tweaks";
        public const string PluginName = "GK2 Tweaks";
        public const string PluginVersion = "0.1.0";

        private GK2TweaksMod _mod;

        private void Awake()
        {
            _mod = new GK2TweaksMod();
            FrameworkApi.RegisterMod(_mod, Config);
        }

        private void Update()
        {
            _mod?.Tick();
        }
    }
}
