using SRML.Console;
using UObject = UnityEngine.Object;

namespace CinematicCamera;

internal abstract class SceneSpecificCommand : ConsoleCommand
{
    public abstract void OnSceneLoaded(SceneContext context);
}

internal sealed class ToggleCinematicCommand : SceneSpecificCommand
{
    public override string ID => "togglecinematic";
    public override string Usage => "togglecinematic";
    public override string Description => "Toggles cinematic mode for camera movement.";

    public override bool Execute(string[] args)
    {
        CinematicCamera.CinematicEnabled = !CinematicCamera.CinematicEnabled;

        foreach (var cam in UObject.FindObjectsOfType<CinematicCamera>())
            cam.ResetCinematic();

        Main.Console.LogSuccess($"Toggled Cinematic Mode: {(CinematicCamera.CinematicEnabled ? "On" : "Off")}");
        return true;
    }

    public override void OnSceneLoaded(SceneContext _) => CinematicCamera.CinematicEnabled = false;
}

internal sealed class SmoothTimeCommand : ConsoleCommand
{
    public override string ID => "smoothtime";
    public override string Usage => "smoothtime [value]";
    public override string Description => "Sets the smooth time for the SmoothDamp mode in seconds. Lower = snappier.";

    public override bool Execute(string[] args)
    {
        if (!float.TryParse(args[0], out var value))
        {
            Main.Console.LogError($"Cannot parse {args[0]} to float.");
            return false;
        }

        CinematicCamera.SmoothTime = value;
        Main.Console.LogSuccess($"Set SmoothTime: {value}");
        return true;
    }
}

internal sealed class LerpFactorCommand : ConsoleCommand
{
    public override string ID => "lerpfactor";
    public override string Usage => "lerpfactor [value]";
    public override string Description => "Sets the interpolation factor used when using Lerp mode (between 0 to 1).";

    public override bool Execute(string[] args)
    {
        if (!float.TryParse(args[0], out var value))
        {
            Main.Console.LogError($"Cannot parse {args[0]} to float.");
            return false;
        }

        if (value is < 0f or > 1f)
        {
            Main.Console.LogError($"{value} is not within the range of 0 to 1");
            return false;
        }

        CinematicCamera.LerpFactor = value;
        Main.Console.LogSuccess($"Set LerpFactor: {value}");
        return true;
    }
}

internal sealed class CinematicModeCommand : ConsoleCommand
{
    public override string ID => "cinmode";
    public override string Usage => "cinmode [value]";
    public override string Description => "Sets the mode of the cinematic camera.";

    public override bool Execute(string[] args)
    {
        if (!Enum.TryParse<CinematicMode>(args[0], out var mode))
        {
            Main.Console.LogError($"Invalid mode name: {args[0]}");
            return false;
        }

        CinematicCamera.ActiveMode = mode;
        Main.Console.LogSuccess($"Set Cinematic Mode: {mode}");
        return true;
    }

    public override List<string> GetAutoComplete(int argIndex, string argText)
    {
        if (argIndex == 0)
            return [.. Enum.GetNames(typeof(CinematicMode))];

        return base.GetAutoComplete(argIndex, argText);
    }
}

internal sealed class ToggleHudCommand : SceneSpecificCommand
{
    public override string ID => "togglehud";
    public override string Usage => "togglehud";
    public override string Description => "Toggles whether the HUD is visible.";

    private static bool HudActive = true;

    public override bool Execute(string[] args)
    {
        HudActive = !HudActive;
        HudUI.Instance.gameObject.SetActive(HudActive);
        Main.Console.LogSuccess($"Toggled Hud: {(HudActive ? "On" : "Off")}");
        return true;
    }

    public override void OnSceneLoaded(SceneContext _) => HudActive = true;
}

internal sealed class ToggleVacPackCommand : SceneSpecificCommand
{
    public override string ID => "togglevac";
    public override string Usage => "togglevac";
    public override string Description => "Toggles whether the vac pack is visible.";

    private static bool VacActive = true;

    public override bool Execute(string[] args)
    {
        VacActive = !VacActive;
        SceneContext.Instance.player.FindChildWithPartialName("VacuumTransform").SetActive(VacActive);

        Main.Console.LogSuccess($"Toggled Vacuum: {(VacActive ? "On" : "Off")}");
        return true;
    }

    public override void OnSceneLoaded(SceneContext _) => VacActive = true;
}