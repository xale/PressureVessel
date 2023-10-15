using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using xale.Subnautica.PressureVessel.Config;
using xale.Subnautica.PressureVessel.Craftables;

namespace xale.Subnautica.PressureVessel;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class PressureVessel : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }

    public new static ConfigFile Config { get; private set; }

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

    private void Awake()
    {
        Logger = base.Logger;
        Config = base.Config;

        // Install internal configuration settings.
        PressureVesselConfig.Initialize();

        // Preload settings menu.
        PressureVesselOptions.get();

        // Install all added craftable items.
        Craftables.Craftables.RegisterAll();

        Harmony.CreateAndPatchAll(Assembly, $"{MyPluginInfo.PLUGIN_GUID}");

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}