using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xale.Subnautica.PressureVessel.Config;

namespace xale.Subnautica.PressureVessel;

internal class DebugMessages
{
    internal static void Show(string message)
    {
        if (PressureVesselConfig.EnableDebugMessages.Value)
        {
            ErrorMessage.AddDebug(message);
        }
    }
}
