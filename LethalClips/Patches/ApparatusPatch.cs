using System;
using HarmonyLib;
using LethalClips;
using Steamworks;


[HarmonyPatch(typeof(LungProp))]
public class ApparatusPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(LungProp.DisconnectFromMachinery))]
    public static void DisconnectFromMachinery() {
        if(!Config.Clips.Apparatus.Value) {
            return;
        }

        try {
            // TODO: abstract this
            var timelineEvent = SteamTimeline.AddInstantaneousTimelineEvent(
                "WARNING!",
                "Someone took the apparatus.",
                "steam_caution",
                0,
                0,
                TimelineEventClipPriority.Standard
            );
            Plugin.Log.LogInfo($"Added timeline event {timelineEvent}.");
        } catch(Exception e) {
            Plugin.Log.LogError($"Failed to add timeline event: {e}");
        }
    }
}
