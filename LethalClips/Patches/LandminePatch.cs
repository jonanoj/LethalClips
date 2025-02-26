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

        var t = other.transform;
        while(t) {
            if(t.TryGetComponent<PlayerControllerB>(out var player)) {
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
        var player = GameNetworkManager.Instance.localPlayerController;
        var death = State<Death>.Of(player);

        var blame = State<Player>.Of(__instance).player;
        death.cause = TranslatedCauseOfDeath.Exploded;
        death.source = blame == null || blame == player ? "Landmine" : blame.playerUsername;
    }
}
