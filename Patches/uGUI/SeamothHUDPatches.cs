using HarmonyLib;
using xale.Subnautica.PressureVessel.Behaviours;

namespace xale.Subnautica.PressureVessel.Patches.uGUI;

[HarmonyPatch(typeof(uGUI_SeamothHUD))]
internal static class SeamothHUDPatches
{
    [HarmonyPatch(nameof(uGUI_SeamothHUD.Update)), HarmonyPostfix]
    internal static void Update_Postfix(uGUI_SeamothHUD __instance)
    {
        if (!__instance.root.activeSelf) { return; }

        // Hide vehicle HUDs when piloting an ROV.
        ROV currentRov = Player.main.GetVehicle()?.GetComponent<ROVDocking>()?.lastDocked;
        __instance.root.SetActive((currentRov == null) || !currentRov.active);
    }
}
