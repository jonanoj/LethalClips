using HarmonyLib;

namespace LethalClips.Patches;


[HarmonyPatch(typeof(LungProp))]
public class ApparatusPatch {
    [HarmonyPatch(nameof(LungProp.DisconnectFromMachinery))]
    [HarmonyPostfix]
    public static void DisconnectFromMachinery() {
        if(Config.Clips.Apparatus.Value) {
            // TODO: identify who took the apparatus?
            Steam.AddEvent("WARNING!", "Someone took the apparatus", Steam.Icon.Caution);
        }
    }
}
