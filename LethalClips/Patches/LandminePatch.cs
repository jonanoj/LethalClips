using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = Landmine;


internal class Player {
    internal PlayerControllerB player;
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
                State<Player>.Of(__instance).player = player;
                break;
            }
            t = t.parent;
        }
    }
}   


[HarmonyPatch(typeof(P), nameof(P.Detonate))]
internal class LandminePatch_Detonate {
    private static void Prefix(
        P __instance
    ) {
        // simulate an explosion to see if we need to trigger a kill
        var explosionPosition = __instance.transform.position + Vector3.up;
        var killRange = 5.7f;
        var damageRange = 6f;
        var nonLethalDamage = 50;
        Collider[] array = Physics.OverlapSphere(explosionPosition, damageRange, 2621448, QueryTriggerInteraction.Collide);
        for(int i = 0; i < array.Length; i++) {
            var obj = array[i].gameObject;
            float dist = Vector3.Distance(explosionPosition, obj.transform.position);
            if(Physics.Linecast(explosionPosition, obj.transform.position + Vector3.up * 0.3f, out RaycastHit hitInfo, 1073742080, QueryTriggerInteraction.Ignore) && ((hitInfo.collider.gameObject.layer == 30) || dist > 4f)) {
                continue;
            }

            if(obj.layer == 3 && obj.GetComponent<PlayerControllerB>() == KillPatch.Player) {
                if(dist < killRange || dist < damageRange && KillPatch.Player.health <= nonLethalDamage) {
                    // set the death cause
                    Kill(__instance);
                    break;
                }
            }
        }
    }
    
    private static void Kill(P __instance) {
        var blame = State<Player>.Of(__instance).player;
        KillPatch.Kill(
            TranslatedCauseOfDeath.Exploded,
            blame == null || blame == KillPatch.Player ? "Landmine" : blame.playerUsername
        );
    }
}
