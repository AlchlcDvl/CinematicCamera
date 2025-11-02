using UnityEngine;

namespace CinematicCamera;

internal enum Mode
{
    SmoothDamp,
    Lerp
}

internal sealed class CinematicCamera : MonoBehaviour
{
    public static bool CinematicEnabled = false;
    public static Mode Mode = Mode.SmoothDamp;
    public static float SmoothTime = 0.12f;
    public static float LerpFactor = 0.15f;

    private vp_FPCamera Cam;
    private float YawVelocity;
    private float PitchVelocity;
    private float TargetYaw;
    private float TargetPitch;

    public void Awake()
    {
        Cam = GetComponent<vp_FPCamera>();

        TargetYaw = Cam.Yaw;
        TargetPitch = Cam.Pitch;
    }

    public void Update()
    {
        if (!CinematicEnabled)
            return;

        var input = Vector2.zero;

        try
        {
            if (Cam.Player)
                input = Cam.Player.InputSmoothLook.Get();
        }
        catch { }

        TargetYaw += input.x;
        TargetPitch += input.y;

        if (TargetYaw < -360f)
            TargetYaw += 360f;

        if (TargetYaw > 360f)
            TargetYaw -= 360f;

        TargetYaw = Mathf.Clamp(TargetYaw, Cam.RotationYawLimit.x, Cam.RotationYawLimit.y);

        if (TargetPitch < -360f)
            TargetPitch += 360f;

        if (TargetPitch > 360f)
            TargetPitch -= 360f;

        TargetPitch = Mathf.Clamp(TargetPitch, 0f - Cam.RotationPitchLimit.x, 0f - Cam.RotationPitchLimit.y);

        float newYaw;
        float newPitch;

        if (Mode == Mode.SmoothDamp)
        {
            newYaw = Mathf.SmoothDampAngle(Cam.Yaw, TargetYaw, ref YawVelocity, SmoothTime, Mathf.Infinity, Time.deltaTime);
            newPitch = Mathf.SmoothDampAngle(Cam.Pitch, TargetPitch, ref PitchVelocity, SmoothTime, Mathf.Infinity, Time.deltaTime);
        }
        else
        {
            var t = 1f - Mathf.Exp(-LerpFactor * Time.deltaTime);
            newYaw = Mathf.LerpAngle(Cam.Yaw, TargetYaw, t);
            newPitch = Mathf.LerpAngle(Cam.Pitch, TargetPitch, t);
        }

        Cam.Yaw = newYaw;
        Cam.Pitch = newPitch;
    }

    public void SetCinematic()
    {
        TargetYaw = Cam.Yaw;
        TargetPitch = Cam.Pitch;
        YawVelocity = PitchVelocity = 0f;
    }
}
