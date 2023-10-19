﻿using HarmonyLib;
using UnityEngine;

namespace xale.Subnautica.PressureVessel.Behaviours;

internal class ROVDocking : MapRoomCameraDocking
{
    internal Collider dockingPort { get; set; }

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
        }

        [HarmonyPatch(nameof(MapRoomCameraDocking.UndockCamera)), HarmonyPrefix]
        internal static void UndockCamera_Prefix(MapRoomCameraDocking __instance)
        {
            if (__instance.GetType() != typeof(ROVDocking)) { return; }

            __instance.camera.transform.parent = null;
            __instance.camera.transform.localPosition = Vector3.zero;
            __instance.camera.GetComponent<Collider>().enabled = true;
        }
    }
}
