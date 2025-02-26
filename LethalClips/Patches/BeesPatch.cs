using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = RedLocustBees;


[HarmonyPatch(typeof(P), nameof(P.OnCollideWithPlayer))]
internal class BeesPatch {
    private static void Prefix(
        Collider other
    ) {
        var player = other.GetComponent<PlayerControllerB>();
        var death = State<Death>.Of(player);
        death.cause = TranslatedCauseOfDeath.Electrocuted;
        death.source = "Circuit Bees";
    }
}
