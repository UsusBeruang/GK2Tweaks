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

        private void Awake()
        {
            FrameworkApi.RegisterMod(new GK2TweaksMod(), Config);
        }
    }
}