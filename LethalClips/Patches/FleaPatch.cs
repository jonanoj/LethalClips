using HarmonyLib;

namespace LethalClips.Patches;


[HarmonyPatch(typeof(CentipedeAI))]
public class FleaPatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CentipedeAI.DamagePlayerOnIntervals))]
    public static void DamagePlayerOnIntervals(CentipedeAI __instance, float ___damagePlayerInterval, bool ___inDroppingOffPlayerAnim, bool ___singlePlayerSecondChanceGiven) {
        // simulate a damage tick
        var player = KillState.Player;
        if(___damagePlayerInterval <= 0f && !___inDroppingOffPlayerAnim) {
            if(__instance.stunNormalizedTimer <= 0f && (StartOfRound.Instance.connectedPlayersAmount > 0 || player.Instance.health > 15 || ___singlePlayerSecondChanceGiven)) {
                player.Damage(ExtendedCauseOfDeath.Suffocated, "Snare Flea", 10);
            }
        }
    }
}
