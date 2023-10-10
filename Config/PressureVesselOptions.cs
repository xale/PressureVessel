using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;

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
}
