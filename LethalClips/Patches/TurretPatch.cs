using HarmonyLib;

namespace LethalClips.Patches;

using P = Turret;


[HarmonyPatch(typeof(P), "Update")]
internal static class TurretPatch {
    internal static void Prefix(
        P __instance,
        float ___turretInterval,
        bool ___enteringBerserkMode
    ) {
        // run through all the checks to see if player got shot
        if(
            !__instance.turretActive
            && (__instance.turretMode == TurretMode.Firing || __instance.turretMode == TurretMode.Berserk)
            && !(__instance.turretMode == TurretMode.Berserk && ___enteringBerserkMode)
            && ___turretInterval >= 0.21f
            && __instance.CheckForPlayersInLineOfSight(3f) == KillPatch.Player
         ) {
            KillPatch.Damage(TranslatedCauseOfDeath.Shot, "Turret", 50);
        }
    }
}
