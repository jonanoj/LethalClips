using GameNetcodeStuff;
using HarmonyLib;

namespace LethalClips.Patches;

using P = SandWormAI;


[HarmonyPatch(typeof(P), nameof(P.EatPlayer))]
internal class WormPatch {
    private static void Prefix(
        PlayerControllerB playerScript
    ) {
        var death = State<Death>.Of(playerScript);
        death.cause = TranslatedCauseOfDeath.Devoured;
        death.source = "Earth Leviathan";
    }
}
