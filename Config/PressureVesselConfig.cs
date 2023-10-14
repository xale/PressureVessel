using BepInEx.Configuration;
using System;

namespace xale.Subnautica.PressureVessel.Config;

/// <summary>
/// Debug/testing settings for the mod; not displayed in the Subnautica settings menu.
/// </summary>
internal class PressureVesselConfig
{
    internal static ConfigFile Config => PressureVessel.Config;

    internal static void Initialize() { }

    public static ConfigEntry<int> SafeDepth { get; } =
        Config.Bind(
            section: "Default",
            key: "Safe depth",
            defaultValue: 200,
            description: "Maximum unassisted diving depth.");

    public static ConfigEntry<bool> EnableDebugMessages { get; } =
        Config.Bind(
            section: "Default",
            key: "Enable debug messages",
            defaultValue: true,
            description: "Show debugging info to player.");
}
