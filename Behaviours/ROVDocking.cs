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
            // Run the normal startup routine only if this is *not* a modded docking system.
            return (__instance.GetType() != typeof(ROVDocking));
        }

        [HarmonyPatch(nameof(MapRoomCameraDocking.DockCamera)), HarmonyPostfix]
        internal static void DockCamera_Postfix(
            MapRoomCameraDocking __instance, MapRoomCamera camera)
        {
            // TODO(xale): do not allow regular camera drones to dock

            // Do not interfere with normal scanner-room camera docking.
            if (__instance.GetType() != typeof(ROVDocking)) { return; }

            ROV rov = (camera as ROV);
            if (rov == null) { return; } // (Unlikely, but just in case.)

            // Attach the drone to the underside of the vehicle.
            // TODO(xale): use the rear for the Prawn
            rov.transform.parent = __instance.dockingTransform;
            rov.transform.localPosition = new Vector3(0, -1.5f, 0);
            rov.GetComponent<Collider>().enabled = false;

            // Exit ROV control, if active.
            if (rov.active) { rov.EndControl(); }

            ErrorMessage.AddMessage("Remora docked.");
        }

        [HarmonyPatch(nameof(MapRoomCameraDocking.UndockCamera)), HarmonyPrefix]
        internal static void UndockCamera_Prefix(MapRoomCameraDocking __instance)
        {
            ROVDocking dockingSystem = (__instance as ROVDocking);
            if (dockingSystem == null) { return; }

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
