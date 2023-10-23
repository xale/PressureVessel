using HarmonyLib;
using UnityEngine;

namespace xale.Subnautica.PressureVessel.Behaviours;

internal class ROVDocking : MapRoomCameraDocking
{
    internal Collider dockingPort { get; set; }

    internal GameObject releasePoint { get; set; }

    internal ROV lastDocked = null;

    // (Wrapper to clarify naming.)
    internal void DockROV(ROV rov) { base.DockCamera(rov); }

    // (Wrapper to clarify naming.)
    internal void UndockROV() { base.UndockCamera(); }

    [HarmonyPatch(typeof(MapRoomCameraDocking))]
    internal static class MapRoomCameraDockingPatches
    {
        [HarmonyPatch(nameof(MapRoomCameraDocking.Start)), HarmonyPrefix]
        internal static bool Start_Prefix(MapRoomCameraDocking __instance)
        {
            DebugMessages.Show("ROVDocking_Start_Prefix");
            return (__instance.GetType() != typeof(ROVDocking));
        }

        [HarmonyPatch(nameof(MapRoomCameraDocking.DockCamera)), HarmonyPostfix]
        internal static void DockCamera_Postfix(
            MapRoomCameraDocking __instance, MapRoomCamera camera)
        {
            // TODO(xale): do not allow regular camera drones to dock
            if (__instance.GetType() != typeof(ROVDocking)) { return; }

            DebugMessages.Show("ROVDocking_DockCamera");

            // Attach the drone to the underside of the vehicle.
            // TODO(xale): use the rear for the Prawn
            camera.transform.parent = __instance.dockingTransform;
            camera.transform.localPosition = new Vector3(0, -1.5f, 0);
            camera.GetComponent<Collider>().enabled = false;

            // TODO(xale): remove player from ROV interface, if active

            ErrorMessage.AddMessage("RemOra docked.");
        }

        [HarmonyPatch(nameof(MapRoomCameraDocking.UndockCamera)), HarmonyPrefix]
        internal static void UndockCamera_Prefix(MapRoomCameraDocking __instance)
        {
            ROVDocking dockingSystem = (__instance as ROVDocking);
            if (dockingSystem == null) { return; }

            DebugMessages.Show("ROVDocking_UndockCamera");

            dockingSystem.camera.transform.parent = null;
            dockingSystem.camera.GetComponent<Collider>().enabled = true;

            ROV undocked = (dockingSystem.camera as ROV);
            if (undocked == null) {
                // Shouldn't happen, but just in case...
                return;
            }
            dockingSystem.lastDocked = undocked;
        }
    }
}
