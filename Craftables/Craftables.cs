using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xale.Subnautica.PressureVessel.Craftables;

/// <summary>
/// Shortcut for all craftable items added by this mod.
/// </summary>
internal static class Craftables
{
    public static void RegisterAll()
    {
        HabitatBuilderModule.Register();
    }
}
