using HarmonyLib;

namespace LethalClips.Patches;


[HarmonyPatch(typeof(Turret))]
public static class TurretPatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Turret.Update))]
    public static void Update(Turret __instance) {
        // run through all the checks to see if player got shot
        if(
            __instance.turretActive
            && (__instance.turretMode == TurretMode.Firing || __instance.turretMode == TurretMode.Berserk)
            && !(__instance.turretMode == TurretMode.Berserk && __instance.enteringBerserkMode)
            && __instance.turretInterval >= 0.21f
            && __instance.CheckForPlayersInLineOfSight(3f) == Player.Local
         ) {
            PlayerState.Local.Damage(ExtendedCauseOfDeath.Shot, "Turret", 50);
        }
    }
}
