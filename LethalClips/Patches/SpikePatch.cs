using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;


[HarmonyPatch(typeof(SpikeRoofTrap))]
public static class SpikePatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(SpikeRoofTrap.OnTriggerStay))]
    public static void OnTriggerStay(SpikeRoofTrap __instance, Collider other) {
        if(!__instance.trapActive || !__instance.slammingDown || Time.realtimeSinceStartup - __instance.timeSinceMovingUp < 0.75f) {
            return;
        }

        if(other.gameObject.TryGetComponent(out PlayerControllerB component) && !component.isPlayerDead) {
            var state = KillState.Of(component);
            state.Kill(ExtendedCauseOfDeath.Crushed, "Spike Trap");            
        }
    }
}
