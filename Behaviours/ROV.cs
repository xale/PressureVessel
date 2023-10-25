using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Extensions;
using System.Collections;
using UnityEngine;
using UWE;

using static xale.Subnautica.PressureVessel.Behaviours.ROV.MapRoomScreenDummyPatches;

namespace xale.Subnautica.PressureVessel.Behaviours;

internal class ROV : MapRoomCamera, IInputHandler
{
    public static TechType rov { get; private set; }

    internal Vehicle mothership;

    internal static void RegisterPrefab()
    {
        PrefabInfo prefabInfo =
            PrefabInfo.WithTechType(
               "ROV", "RemOra Drone", "Remotely-operated utility drone.")
            .WithSizeInInventory(new Vector2int(2, 2))
            .WithIcon(SpriteManager.Get(TechType.MapRoomCamera)); // TODO(xale): custom sprite
        rov = prefabInfo.TechType;

        CloneTemplate template = new CloneTemplate(prefabInfo, TechType.MapRoomCamera);
        template.ModifyPrefab += instance =>
        {
            MapRoomCamera baseBehavior = instance.GetComponent<MapRoomCamera>();
            instance.AddComponent<ROV>().CopyComponent(baseBehavior);
            GameObject.DestroyImmediate(baseBehavior);

            CoroutineHost.StartCoroutine(
                instance.GetComponent<EnergyMixin>()
                    .SpawnDefaultAsync(1.0f, DiscardTaskResult<bool>.Instance));
        };

        // TODO(xale): set custom model

        CustomPrefab rovPrefab = new CustomPrefab(prefabInfo);
        rovPrefab.SetGameObject(template);

        rovPrefab.Register();
    }

    internal void Control(Vehicle mothership)
    {
        base.ControlCamera(/* MapRoomCameraScreen= */ null);
        this.mothership = mothership;
        InputHandlerStack.main.Push(this);
    }

    internal void EndControl()
    {
        InputHandlerStack.main.Pop();
        this.mothership = null;

        base.FreeCamera();
        uGUI_CameraDrone.main.SetCamera(null);
        base.active = false;

        Player.main.SetHeadVisible(false);

        GameInput.ClearInput();

        // Reset the game camera to the player's location and reenable control.
        SNCameraRoot.main.transform.localPosition = Vector3.zero;
        SNCameraRoot.main.transform.localRotation = Quaternion.identity;
        MainCameraControl.main.enabled = true;
    }

    bool IInputHandler.HandleInput()
    {
        if (!base.active) { return false; }

        if (GameInput.GetButtonDown(GameInput.Button.Exit))
        {
            EndControl();
            return true;
        }

        base.HandleInput();
        return true;
    }

    public bool HandleLateInput()
    {
        return true;
    }

    public void OnFocusChanged(InputFocusMode mode)
    {
        // No-op.
    }

    [HarmonyPatch(typeof(MapRoomCamera))]
    internal static class MapRoomCameraPatches
    {
        [HarmonyPatch(nameof(MapRoomCamera.Start)), HarmonyPostfix]
        internal static void Start_Postfix(MapRoomCamera __instance)
        {
            if (__instance.GetType() != typeof(ROV)) { return; }

            // Because the static initialization for MapRoomCamera is performed asynchronously,
            // we need to wait for it to complete before we can revert it.
            CoroutineHost.StartCoroutine(WaitAndRemoveFromScannerCameraList(__instance));
        }

        private static IEnumerator WaitAndRemoveFromScannerCameraList(MapRoomCamera camera)
        {
            // Keep ROVs out of the global list of scanner-room cameras.
            yield return new WaitUntil(() => MapRoomCamera.cameras.Remove(camera));
        }

        [HarmonyPatch(nameof(MapRoomCamera.ExitLockedMode)), HarmonyPrefix]
        internal static void ExitLockedMode_Prefix(MapRoomCamera __instance)
        {
            // Unpatched implementation expects that a MapRoomScreen is associated with the camera,
            // and attempts to invoke a method without a null check. Provide an ephemeral dummy.
            if (__instance.screen == null) { __instance.screen = new DummyMapRoomScreen(); }
        }
    }

    [HarmonyPatch(typeof(MapRoomScreen))]
    internal static class MapRoomScreenDummyPatches
    {
        internal class DummyMapRoomScreen : MapRoomScreen { }

        [HarmonyPatch(nameof(MapRoomScreen.OnCameraFree)), HarmonyPrefix]
        internal static bool OnCameraFree_Prefix(MapRoomScreen __instance)
        {
            // Skip this method on dummy screen instances.
            return (__instance.GetType() != typeof(DummyMapRoomScreen));
        }
    }

}
