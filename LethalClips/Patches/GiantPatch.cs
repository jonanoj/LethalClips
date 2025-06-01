using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;


[HarmonyPatch(typeof(ForestGiantAI))]
public class GiantPatch {
    [HarmonyPatch(nameof(ForestGiantAI.AnimationEventA))]
    [HarmonyPrefix]
    public static void AnimationEventA(ForestGiantAI __instance) {
        // crush players upon whom the giant falls
        RaycastHit[] array = Physics.SphereCastAll(
            __instance.deathFallPosition.position,
            2.7f,
            __instance.deathFallPosition.forward,
            3.9f,
            StartOfRound.Instance.playersMask,
            QueryTriggerInteraction.Ignore
        );
        for(int i = 0; i < array.Length; i++) {
            var player = array[i].transform.GetState<PlayerControllerB, PlayerState>();
            player.Damage(ExtendedCauseOfDeath.Crushed, "Forest Keeper", 30);
        }
    }
}
