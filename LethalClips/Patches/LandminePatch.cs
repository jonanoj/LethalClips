using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = Landmine;


public class PlayerState : State<P, PlayerState> {
    public PlayerControllerB player;
}

[HarmonyPatch(typeof(P), "OnTriggerExit")]
internal class LandminePatch_OnTriggerExit {
    private static void Prefix(
        P __instance,
        Collider other,
        bool ___mineActivated
    ) {
        if(__instance.hasExploded || !___mineActivated) {
            return;
        }

        // rough search to find the attached player
        var t = other.transform;
        while(t) {
            if(t.TryGetComponent<PlayerControllerB>(out var player)) {
                // blame the player for death
                PlayerState.Of(__instance).player = player;
                break;
            }
            t = t.parent;
        }
    }
}   


[HarmonyPatch(typeof(P), nameof(P.Detonate))]
internal class LandminePatch_Detonate {

    internal static void SpawnExplosion(Vector3 explosionPosition, float killRange, float damageRange, int nonLethalDamage, string source) {
        // simulate an explosion to see if we need to trigger a kill
        Collider[] array = Physics.OverlapSphere(explosionPosition, damageRange, 2621448, QueryTriggerInteraction.Collide);
        for(int i = 0; i < array.Length; i++) {
            var obj = array[i].gameObject;
            float dist = Vector3.Distance(explosionPosition, obj.transform.position);
            if(Physics.Linecast(explosionPosition, obj.transform.position + Vector3.up * 0.3f, out RaycastHit hitInfo, 1073742080, QueryTriggerInteraction.Ignore) && ((hitInfo.collider.gameObject.layer == 30) || dist > 4f)) {
                continue;
            }

            if(obj.layer == 3 && obj.GetComponent<PlayerControllerB>() == KillPatch.Player) {
                // set the cause of death
                if(dist < killRange) {
                    KillPatch.Kill(ExtendedCauseOfDeath.Exploded, source);
                } else if(dist < damageRange) {
                    KillPatch.Damage(ExtendedCauseOfDeath.Exploded, source, nonLethalDamage);
                }
                break;
            }
        }
    }

    private static void Prefix(
        P __instance
    ) {
        SpawnExplosion(__instance.transform.position + Vector3.up, 5.7f, 6f, 50, Blame(__instance));
    }
    
    private static string Blame(P __instance) {
        var blame = PlayerState.Of(__instance).player;
        return blame == null || blame == KillPatch.Player ? "Landmine" : blame.playerUsername;
    }
}
