using HarmonyLib;

namespace LethalClips.Patches;


[HarmonyPatch(typeof(CentipedeAI))]
public class FleaPatch {
    [HarmonyPatch(nameof(CentipedeAI.DamagePlayerOnIntervals))]
    [HarmonyPrefix]
    public static void DamagePlayerOnIntervals(CentipedeAI __instance) {
        // simulate a damage tick
        if(
            __instance.damagePlayerInterval <= 0f
            && !__instance.inDroppingOffPlayerAnim
            && __instance.stunNormalizedTimer <= 0f
            && (StartOfRound.Instance.connectedPlayersAmount > 0 || Player.Local.health > 15 || __instance.singlePlayerSecondChanceGiven)
        ) {
            PlayerState.Local.Damage(ExtendedCauseOfDeath.Suffocated, "Snare Flea", 10);
        }
    }
}
