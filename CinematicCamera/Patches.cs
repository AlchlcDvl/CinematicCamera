using HarmonyLib;
using UnityEngine;

namespace CinematicCamera;

[HarmonyPatch(typeof(vp_FPCamera))]
internal static class CameraPatches
{
    [HarmonyPatch(nameof(vp_FPCamera.Awake))]
    public static void Postfix(vp_FPCamera __instance) => __instance.gameObject.EnsureComponent<CinematicCamera>();

    private static T EnsureComponent<T>(this GameObject gameObj) where T : Component => gameObj.GetComponent<T>() ?? gameObj.AddComponent<T>();

    [HarmonyPatch(nameof(vp_FPCamera.UpdateInput))]
    public static bool Prefix(vp_FPCamera __instance)
    {
        if (!__instance.TryGetComponent<CinematicCamera>(out var camera))
            return true;

        camera.UpdateInput();
        return false;
    }
}