using HarmonyLib;

namespace LethalClips.Patches;

using P = StartOfRound;


[HarmonyPatch(typeof(P), nameof(P.ReviveDeadPlayers))]
internal class RevivePatch {

    private static void Postfix(
        P __instance
    ) {
        foreach(var player in __instance.allPlayerScripts) {
            var death = State<Death>.Of(player);
            death.cause = TranslatedCauseOfDeath.Killed;
            death.source = null;
        }
    }
}
