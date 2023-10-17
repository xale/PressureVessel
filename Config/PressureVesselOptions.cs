using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace xale.Subnautica.PressureVessel.Config;

/// <summary>
/// User-editable mod settings.
/// </summary>
[Menu("Pressure Vessel")]
internal class PressureVesselOptions : ConfigFile
{
    private static PressureVesselOptions INSTANCE;

    public static PressureVesselOptions get()
    {
        INSTANCE ??= OptionsPanelHandler.RegisterModOptions<PressureVesselOptions>();
        return INSTANCE;
    }

    [Toggle(
        "Lock hatches",
        Tooltip = "If checked, you cannot leave a vehicle or habitat below safe diving depth")]
    public bool lockHatches = true;

    [Keybind(
        "Open vehicle inventory",
        Tooltip = "Opens the inventory of the vehicle you are currently piloting")]
    public KeyCode openVehicleInventoryKey = KeyCode.LeftBracket;

    [Keybind(
        "Open vehicle modules",
        Tooltip = "Open the module upgrade panel for the vehicle you are currently piloting")]
    public KeyCode openVehicleModulesKey = KeyCode.RightBracket;
}
