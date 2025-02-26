using GameNetcodeStuff;
using HarmonyLib;

namespace LethalClips.Patches;

using P = SandWormAI;


[HarmonyPatch(typeof(P), nameof(P.EatPlayer))]
internal class WormPatch {
    private static void Prefix(
        PlayerControllerB playerScript,
        ref Death __state
    ) {
        var death = State<Death>.Of(playerScript);
        __state = new() {
            cause = death.cause,
            source = death.source
        };
        death.cause = TranslatedCauseOfDeath.Devoured;
        death.source = "Earth Leviathan";
    }

    private static void Postfix(
        PlayerControllerB playerScript,
        Death __state
    ) {
        // restore the previous cause of death, in case no kill happened
        var death = State<Death>.Of(playerScript);
        death.cause = __state.cause;
        death.source = __state.source;
    }
}
