using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;


[HarmonyPatch(typeof(ShotgunItem))]
public class ShotgunPatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ShotgunItem.ShootGun))]
    public static void ShootGun(ShotgunItem __instance, Vector3 shotgunPosition, Vector3 shotgunForward) {
        string shooter;
        if(__instance.isHeldByEnemy) {
            shooter = "Nutcracker";
        } else if(__instance.isHeld) {
            shooter = __instance.playerHeldBy?.playerUsername ?? "Player";
        } else {
            shooter = "Accident";
        }

        // simulate a shot to see if we need to trigger a kill
        var localPlayer = KillState.Player.Instance;
        bool heldByPlayer = __instance.isHeld && __instance.playerHeldBy == localPlayer;
        float dist = Vector3.Distance(localPlayer.transform.position, __instance.shotgunRayPoint.transform.position);
        Vector3 vector = localPlayer.playerCollider.ClosestPoint(shotgunPosition);
        
        bool hit = !heldByPlayer;
        hit &= !Physics.Linecast(shotgunPosition, vector, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore);
        hit &= Vector3.Angle(shotgunForward, vector - shotgunPosition) < 30f;

        int damageNumber = 0;
        if(dist < 15f) {
            damageNumber = 100;
        } else if(dist < 23f) {
            damageNumber = 40;
        } else if(dist < 30f) {
            damageNumber = 20;
        }

        if(hit) {
            KillState.Player.Damage(ExtendedCauseOfDeath.Shot, shooter, damageNumber);
        }
    }
}
