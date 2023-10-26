using System;
using HarmonyLib;
using xale.Subnautica.PressureVessel.Behaviours;

namespace xale.Subnautica.PressureVessel.Patches.uGUI;

[HarmonyPatch(typeof(uGUI_QuickSlots))]
internal static class QuickSlotsPatches
{
    [HarmonyPatch(nameof(uGUI_QuickSlots.GetTarget)), HarmonyPostfix]
    internal static IQuickSlots GetTarget_Postfix(IQuickSlots originalResult)
    {
        if (originalResult != null) return originalResult;

        // If the player is controlling an ROV, allow control of its quick slots.
        ROV rov = (uGUI_CameraDrone.main?.GetCamera() as ROV);
        return rov?.GetComponent<ROVQuickSlots>();
    }
}
