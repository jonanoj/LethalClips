using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = ClaySurgeonAI;


[HarmonyPatch(typeof(P), nameof(P.OnCollideWithPlayer))]
internal class BarberPatch {
    private static void Prefix(
        Collider other
    ) {
        var player = other.GetComponent<PlayerControllerB>();
        var death = State<Death>.Of(player);
        death.cause = TranslatedCauseOfDeath.Snipped;
        death.source = "Barber";
    }
}
