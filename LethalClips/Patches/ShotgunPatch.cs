using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = ShotgunItem;


[HarmonyPatch(typeof(P), "ShootGun")]
internal class ShotgunPatch {
    private static void Prefix(
        P __instance,
        Vector3 shotgunPosition,
        Vector3 shotgunForward
    ) {
        // determine the shooter
        string blame;
        if(__instance.isHeldByEnemy) {
            blame = "Nutcracker";
        } else if(__instance.isHeld) {
            blame = __instance.playerHeldBy?.playerUsername ?? "Player";
        } else {
            blame = "Accident";
        }

        // simulate a shot to see if we need to trigger a kill
        bool heldByPlayer = __instance.isHeld && __instance.playerHeldBy == KillPatch.Player;
        float dist = Vector3.Distance(KillPatch.Player.transform.position, __instance.shotgunRayPoint.transform.position);
        Vector3 vector = KillPatch.Player.playerCollider.ClosestPoint(shotgunPosition);
        
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
            KillPatch.Damage(TranslatedCauseOfDeath.Shot, blame, damageNumber);
        }
    }
}

