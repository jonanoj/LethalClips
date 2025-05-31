using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;


public class LandmineState : State<Landmine, LandmineState> {
    public PlayerControllerB detonator;
    private string DetonatorName => !detonator || detonator == Player.Local ? "Landmine" : detonator.playerUsername;
    
    public static void SpawnExplosion(Vector3 explosionPosition, float killRange, float damageRange, int nonLethalDamage, string source) {
        Plugin.Log.LogWarning($"kabooming");
        // simulate an explosion to see if we need to trigger a kill
        var array = Physics.OverlapSphere(explosionPosition, damageRange, 2621448, QueryTriggerInteraction.Collide);
        for(int i = 0; i < array.Length; i++) {
            Plugin.Log.LogWarning($"hit");
            var obj = array[i].gameObject;
            float dist = Vector3.Distance(explosionPosition, obj.transform.position);
            if(Physics.Linecast(explosionPosition, obj.transform.position + Vector3.up * 0.3f, out RaycastHit hitInfo, 1073742080, QueryTriggerInteraction.Ignore) && ((hitInfo.collider.gameObject.layer == 30) || dist > 4f)) {
                continue;
            }

            if(obj.layer == 3) {
                // set the cause of death
                Plugin.Log.LogWarning($"found object {obj}");
                var player = obj.GetState<PlayerControllerB, PlayerState>();
                Plugin.Log.LogWarning($"player {player}");
                if(dist < killRange) {
                    player.Kill(ExtendedCauseOfDeath.Exploded, source);
                } else if(dist < damageRange) {
                    player.Damage(ExtendedCauseOfDeath.Exploded, source, nonLethalDamage);
                }
                break;
            }
        }
    }

    public void SpawnExplosion(Vector3 explosionPosition, float killRange, float damageRange, int nonLethalDamage) {
        SpawnExplosion(explosionPosition, killRange, damageRange, nonLethalDamage, DetonatorName);
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

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Landmine.Detonate))]
    public static void Detonate(Landmine __instance) {
        var state = LandmineState.Of(__instance);
        state.SpawnExplosion(__instance.transform.position + Vector3.up, 5.7f, 6f, 50);
    }
}
