using HarmonyLib;

namespace LethalClips.Patches;

using P = MouthDogAI;


[HarmonyPatch(typeof(P), "KillPlayer")]
internal class DogPatch {
    private static void Prefix(
        int playerId
    ) {
        var player = StartOfRound.Instance.allPlayerScripts[playerId];
        var death = State<Death>.Of(player);
        death.cause = ExtendedCauseOfDeath.EyelessDog;
    }
}
