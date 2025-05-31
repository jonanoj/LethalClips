using System;
using HarmonyLib;
using LethalClips;
using Steamworks;


[HarmonyPatch(typeof(LungProp))]
public class ApparatusPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(LungProp.DisconnectFromMachinery))]
    public static void DisconnectFromMachinery() {
        if(Config.Clips.Apparatus.Value) {
            // TODO: identify who took the apparatus?
            Steam.AddEvent("WARNING!", "Someone took the apparatus", Steam.Icon.Caution);
        }
    }
}
