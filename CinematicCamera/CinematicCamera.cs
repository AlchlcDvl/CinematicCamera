using UnityEngine;

namespace CinematicCamera;

internal enum CinematicMode
{
    SmoothDamp,
    Interpolate
}

[RequireComponent(typeof(vp_FPCamera))]
internal sealed class CinematicCamera : MonoBehaviour
{
    public static bool CinematicEnabled = false;
    public static float SmoothTime = 0.12f;
    public static float LerpFactor = 0.15f;
    public static CinematicMode ActiveMode = CinematicMode.SmoothDamp;

    private vp_FPCamera Cam;

    private float YawVelocity;
    private float PitchVelocity;

    private float TargetYaw;
    private float TargetPitch;

    public void Awake()
    {
        Cam = GetComponent<vp_FPCamera>();

        TargetYaw = Cam.m_Yaw;
        TargetPitch = Cam.m_Pitch;
    }

    public void UpdateInput()
    {
        if (Cam.Player.Dead.Active)
            return;

        var input = Cam.Player.InputSmoothLook.Get();

        if (input == Vector2.zero)
            return;

        TargetYaw = NormalizeTargetAngle(TargetYaw, input.x, Cam.RotationYawLimit.x, Cam.RotationYawLimit.y);
        TargetPitch = NormalizeTargetAngle(TargetPitch, input.y, -Cam.RotationPitchLimit.x, -Cam.RotationPitchLimit.y);

        if (!CinematicEnabled)
        {
            Cam.m_Yaw = TargetYaw;
            Cam.m_Pitch = TargetPitch;
            return;
        }

        float newYaw;
        float newPitch;

        if (ActiveMode == CinematicMode.SmoothDamp)
        {
            newYaw = Mathf.SmoothDampAngle(Cam.m_Yaw, TargetYaw, ref YawVelocity, SmoothTime);
            newPitch = Mathf.SmoothDampAngle(Cam.m_Pitch, TargetPitch, ref PitchVelocity, SmoothTime);
        }
        else
        {
            var t = 1f - Mathf.Exp(-LerpFactor * Time.deltaTime);
            newYaw = Mathf.LerpAngle(Cam.m_Yaw, TargetYaw, t);
            newPitch = Mathf.LerpAngle(Cam.m_Pitch, TargetPitch, t);
        }

        Cam.m_Yaw = newYaw;
        Cam.m_Pitch = newPitch;
    }

    public void ResetCinematic()
    {
        TargetYaw = Cam.m_Yaw;
        TargetPitch = Cam.m_Pitch;
        YawVelocity = PitchVelocity = 0f;
    }

    private static float NormalizeTargetAngle(float angle, float add, float xLim, float yLim)
    {
        angle += add;

        if (angle < -360f)
            angle += 360f;

        if (angle > 360f)
            angle -= 360f;

        return Mathf.Clamp(angle, xLim, yLim);
    }
}
