﻿using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;


[HarmonyPatch(typeof(ShotgunItem))]
public class ShotgunPatch {
    [HarmonyPatch(nameof(ShotgunItem.ShootGun))]
    [HarmonyPrefix]
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
        bool heldByPlayer = __instance.isHeld && __instance.playerHeldBy == Player.Local;
        float dist = Vector3.Distance(Player.Local.transform.position, __instance.shotgunRayPoint.transform.position);
        Vector3 vector = Player.Local.playerCollider.ClosestPoint(shotgunPosition);
        
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
            PlayerState.Local.Damage(ExtendedCauseOfDeath.Shot, shooter, damageNumber);
        }
    }
}
