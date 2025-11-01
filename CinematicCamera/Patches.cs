using HarmonyLib;

namespace CinematicCamera;

[HarmonyPatch(typeof(vp_FPCamera))]
internal static class CameraPatches
{
    [HarmonyPatch(nameof(vp_FPCamera.Awake))]
    public static void Postfix(vp_FPCamera __instance)
    {
        if (!__instance.GetComponent<CinematicCamera>())
            __instance.gameObject.AddComponent<CinematicCamera>();
    }

    [HarmonyPatch(nameof(vp_FPCamera.UpdateInput))]
    public static bool Prefix() => !CinematicCamera.CinematicEnabled;
}