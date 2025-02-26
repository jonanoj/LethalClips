using HarmonyLib;

namespace LethalClips.Patches;

using P = FlowermanAI;


[HarmonyPatch(typeof(P), "killAnimation")]
internal class BrackenPatch {
    private static void Prefix(
        P __instance,
        ref Death __state
    ) {
        var death = State<Death>.Of(__instance.inSpecialAnimationWithPlayer);
        __state = new() {
            cause = death.cause,
            source = death.source
        };
        death.cause = TranslatedCauseOfDeath.Strangled;
        death.source = "Bracken";
    }

    private static void Postfix(
        P __instance,
        Death __state
    ) {
        // restore the previous cause of death, in case no kill happened
        var death = State<Death>.Of(__instance.inSpecialAnimationWithPlayer);
        death.cause = __state.cause;
        death.source = __state.source;
    }
}
