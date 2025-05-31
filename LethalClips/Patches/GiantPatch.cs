using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;


[HarmonyPatch(typeof(ForestGiantAI))]
public class GiantPatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ForestGiantAI.AnimationEventA))]
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
            if(array[i].transform.TryGetComponent(out PlayerControllerB component)) {
                var player = PlayerState.Of(component);
                player.Damage(ExtendedCauseOfDeath.Crushed, "Forest Keeper", 30);
            }
        }
    }
}
