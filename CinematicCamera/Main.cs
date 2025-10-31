using SRML;
using SRML.SR;

namespace CinematicCamera;

internal sealed class Main : ModEntryPoint
{
    public override void PreLoad() => HarmonyInstance.PatchAll(typeof(Main).Assembly);
}