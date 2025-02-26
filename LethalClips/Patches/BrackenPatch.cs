using HarmonyLib;

namespace LethalClips.Patches;

using P = FlowermanAI;


[HarmonyPatch(typeof(P), "killAnimation")]
internal class BrackenPatch {
    private static void Prefix(
        P __instance
    ) {
        var death = State<Death>.Of(__instance.inSpecialAnimationWithPlayer);
        death.cause = TranslatedCauseOfDeath.Strangled;
        death.source = "Bracken";
    }
}
