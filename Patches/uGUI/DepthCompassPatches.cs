using HarmonyLib;
using UnityEngine;
using xale.Subnautica.PressureVessel.Behaviours;
using static uGUI_DepthCompass;

namespace xale.Subnautica.PressureVessel.Patches.uGUI;

[HarmonyPatch(typeof(uGUI_DepthCompass))]
internal static class DepthCompassPatches
{
    [HarmonyPatch(nameof(uGUI_DepthCompass.GetDepthInfo)), HarmonyPostfix]
    internal static DepthMode GetDepthInfo_Postfix(
        DepthMode originalResult, ref int depth, ref int crushDepth)
    {
        // If the player is not inside a vehicle, no work is needed.
        if (originalResult != DepthMode.Submersible) { return originalResult; }

        // Check whether player is controlling an ROV from within the vehicle.
        ROV rov = (uGUI_CameraDrone.main.GetCamera() as ROV);
        if (rov == null) { return originalResult; }

        // If so, use the ROV's depth measurement rather than the submersible's.
        depth = Mathf.FloorToInt(rov.GetDepth());
        crushDepth = 0;
        return DepthMode.MapRoomCamera;
    }
}
