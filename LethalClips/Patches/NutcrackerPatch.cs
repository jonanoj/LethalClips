using HarmonyLib;

namespace LethalClips.Patches;

using P = NutcrackerEnemyAI;


[HarmonyPatch(typeof(P), "LegKickPlayer")]
internal class NutcrackerPatch {
    private static void Prefix(
        P __instance
    ) {
        var death = State<Death>.Of(__instance.inSpecialAnimationWithPlayer);
        death.cause = TranslatedCauseOfDeath.Kicked;
        death.source = "Nutcracker";
    }
}

