using HarmonyLib;

namespace LethalClips.Patches;

using P = MaskedPlayerEnemy;


[HarmonyPatch(typeof(P), "killAnimation")]
internal class MaskedPatch {
    private static void Prefix(
        P __instance
    ) {
        var death = State<Death>.Of(__instance.inSpecialAnimationWithPlayer);
        death.cause = TranslatedCauseOfDeath.Infected;
        death.source = "Masked";
    }
}
