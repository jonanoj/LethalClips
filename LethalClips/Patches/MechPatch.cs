using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;


[HarmonyPatch(typeof(RadMechAI))]
public class MechPatch {
    [HarmonyPatch(nameof(RadMechAI.Stomp))]
    [HarmonyPrefix]
    public static void Stomp(Transform stompTransform, float radius) {
        // check if the player is within the stomp radius
        double num = Vector3.Distance(Player.Local.transform.position, stompTransform.position);
        if(num < radius) {
            if(num < radius * 0.175) {
                PlayerState.Local.Damage(ExtendedCauseOfDeath.Crushed, "Old Bird", 70);
            } else if(num < radius * 0.5f) {
                PlayerState.Local.Damage(ExtendedCauseOfDeath.Crushed, "Old Bird", 30);
            }
        }
    }

    [HarmonyPatch(nameof(RadMechAI.SetExplosion))]
    [HarmonyPrefix]
    public static void SetExplosion(Vector3 explosionPosition, Vector3 forwardRotation) {
        // steal explosion code from landmines, since this is what old birds do
        LandmineState.SpawnExplosion(explosionPosition - forwardRotation * 0.1f, 1f, 7f, 30, "Old Bird");
    }
}