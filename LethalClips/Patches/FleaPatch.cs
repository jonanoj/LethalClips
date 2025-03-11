using HarmonyLib;

namespace LethalClips.Patches;

using P = CentipedeAI;


[HarmonyPatch(typeof(P), "DamagePlayerOnIntervals")]
internal class FleaPatch {
    private static void Prefix(
        P __instance,
        float ___damagePlayerInterval,
        bool ___inDroppingOffPlayerAnim,
        bool ___singlePlayerSecondChanceGiven
    ) {
        // simulate a damage tick
        if(___damagePlayerInterval <= 0f && !___inDroppingOffPlayerAnim) {
            if(__instance.stunNormalizedTimer <= 0f && (StartOfRound.Instance.connectedPlayersAmount > 0 || KillPatch.Player.health > 15 || ___singlePlayerSecondChanceGiven)) {
                KillPatch.Damage(TranslatedCauseOfDeath.Suffocated, "Snare Flea", 10);
            }
        }
    }
}
