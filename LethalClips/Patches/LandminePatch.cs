using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;


public class LandmineState : State<Landmine, LandmineState> {
    public PlayerControllerB detonator;
    private string DetonatorName => !detonator || detonator == GameNetworkManager.Instance.localPlayerController ? "Landmine" : detonator.playerUsername;

    public void SpawnExplosion(Vector3 explosionPosition, float killRange, float damageRange, int nonLethalDamage) {
        // simulate an explosion to see if we need to trigger a kill
        var array = Physics.OverlapSphere(explosionPosition, damageRange, 2621448, QueryTriggerInteraction.Collide);
        for(int i = 0; i < array.Length; i++) {
            var obj = array[i].gameObject;
            float dist = Vector3.Distance(explosionPosition, obj.transform.position);
            if(Physics.Linecast(explosionPosition, obj.transform.position + Vector3.up * 0.3f, out RaycastHit hitInfo, 1073742080, QueryTriggerInteraction.Ignore) && ((hitInfo.collider.gameObject.layer == 30) || dist > 4f)) {
                continue;
            }

            // TODO: would like a better way to just tryget the state
            if(obj.layer == 3 && obj.TryGetComponent(out PlayerControllerB controller)) {
                var player = KillState.Of(controller);
                // set the cause of death
                if(dist < killRange) {
                    player.Kill(ExtendedCauseOfDeath.Exploded, DetonatorName);
                } else if(dist < damageRange) {
                    player.Damage(ExtendedCauseOfDeath.Exploded, DetonatorName, nonLethalDamage);
                }
                break;
            }
        }
    }
}

[HarmonyPatch(typeof(Landmine))]
public class LandminePatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Landmine.OnTriggerExit))]
    public static void OnTriggerExit(Landmine __instance, Collider other) {
        if(__instance.hasExploded || !__instance.mineActivated) {
            return;
        }

        // rough search to find the attached player
        var t = other.transform;
        while(t) {
            if(t.TryGetComponent<PlayerControllerB>(out var player)) {
                // blame the player for setting off the mine
                LandmineState.Of(__instance).detonator = player;
                break;
            }
            t = t.parent;
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Landmine.Detonate))]
    public static void Detonate(Landmine __instance) {
        var state = LandmineState.Of(__instance);
        state.SpawnExplosion(__instance.transform.position + Vector3.up, 5.7f, 6f, 50);
    }
}
