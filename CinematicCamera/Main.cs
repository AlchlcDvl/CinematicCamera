using SRML;
using SConsole = SRML.Console.Console;

namespace CinematicCamera;

internal sealed class Main : ModEntryPoint
{
    public static SConsole.ConsoleInstance Console;

    public override void PreLoad()
    {
        Console = ConsoleInstance;

        HarmonyInstance.PatchAll(typeof(Main).Assembly);

        SConsole.RegisterCommand(new ToggleHudCommand());
        SConsole.RegisterCommand(new SmoothTimeCommand());
        SConsole.RegisterCommand(new LerpFactorCommand());
        SConsole.RegisterCommand(new CinematicModeCommand());
        SConsole.RegisterCommand(new ToggleCinematicCommand());
    }
}