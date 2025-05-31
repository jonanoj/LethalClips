using HarmonyLib;

namespace LethalClips.Patches;


[HarmonyPatch(typeof(CentipedeAI))]
public class FleaPatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CentipedeAI.DamagePlayerOnIntervals))]
    public static void DamagePlayerOnIntervals(CentipedeAI __instance) {
        // simulate a damage tick
        var player = KillState.Player;
        if(
            __instance.damagePlayerInterval <= 0f
            && !__instance.inDroppingOffPlayerAnim
            && __instance.stunNormalizedTimer <= 0f
            && (StartOfRound.Instance.connectedPlayersAmount > 0 || player.Instance.health > 15 || __instance.singlePlayerSecondChanceGiven)
        ) {
            player.Damage(ExtendedCauseOfDeath.Suffocated, "Snare Flea", 10);
        }
    }
}
