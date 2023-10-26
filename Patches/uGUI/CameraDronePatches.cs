using HarmonyLib;
using xale.Subnautica.PressureVessel.Behaviours;

namespace xale.Subnautica.PressureVessel.Patches.uGUI;

[HarmonyPatch(typeof(uGUI_CameraDrone))]
internal static class CameraDronePatches
{
    [HarmonyPatch(nameof(uGUI_CameraDrone.UpdateCameraTitle)), HarmonyPostfix]
    internal static void UpdateCameraTitle_Postfix(uGUI_CameraDrone __instance)
    {
        ROV rov = (__instance.activeCamera as ROV);
        if (rov == null) return;

        // TODO(xale): update mothership name when necessary
        string vehicleName = rov.mothership?.subName?.GetName() ?? "Unknown Vehicle";
        __instance.textTitle.text = $"Remora ({vehicleName})";
    }
}
