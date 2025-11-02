using SRML;
using SRML.Console;
using SRML.SR;
using SConsole = SRML.Console.Console;

namespace CinematicCamera;

internal sealed class Main : ModEntryPoint
{
    public static SConsole.ConsoleInstance Console;

    private static readonly ConsoleCommand[] Commands = [new ToggleHudCommand(), new SmoothTimeCommand(), new LerpFactorCommand(), new CinematicModeCommand(), new ToggleCinematicCommand(), new ToggleVacPackCommand()];

    public override void PreLoad()
    {
        Console = ConsoleInstance;

        HarmonyInstance.PatchAll(typeof(Main).Assembly);

        foreach (var command in Commands)
        {
            SConsole.RegisterCommand(command);

            if (command is SceneSpecificCommand sceneCommand)
                SRCallbacks.OnSaveGameLoaded += sceneCommand.OnSceneLoaded;
        }
    }
}