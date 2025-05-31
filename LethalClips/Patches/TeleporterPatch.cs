using System;
using GameNetcodeStuff;
using HarmonyLib;
using LethalClips;
using Steamworks;
using P = ShipTeleporter;

internal enum TeleporterType
{
    Unknown = 0,
    Normal = 1,
    Inverse = 2,
}


[HarmonyPatch(typeof(P), "SetPlayerTeleporterId")]
internal class TeleporterPatch
{
    internal static PlayerControllerB Player => GameNetworkManager.Instance.localPlayerController;

    internal static int LastTeleporterId { get; private set; } = -1;
    internal static DateTime LastTeleportEventTime { get; private set; } = DateTime.MinValue;

    private static void Prefix(
       P __instance,
       PlayerControllerB playerScript,
       int teleporterId)
    {
        if (Player != playerScript)
        {
            // Another player was teleported
            return;
        }

        if (teleporterId == LastTeleporterId)
        {
            // Either:
            // 1. Player missed the inverse teleporter, ignore
            // 2. Inverse teleport is loading (it constantly fires calls with ID 2 until teleported)
            return;
        }

        if (teleporterId == -1 && LastTeleporterId != -1)
        {
            // Player teleported!
            TeleporterType teleporterType = (TeleporterType)LastTeleporterId;
            if (!Enum.IsDefined(typeof(TeleporterType), teleporterType))
            {
                teleporterType = TeleporterType.Unknown;
            }

            CreateTeleportEvent(teleporterType);
        }

        // Save the last used teleporter ID
        LastTeleporterId = teleporterId;
    }

    private static void CreateTeleportEvent(TeleporterType teleporterType)
    {
        if (DateTime.UtcNow - LastTeleportEventTime < TimeSpan.FromSeconds(1))
        {
            // Inverse teleporter fires 2 events
            Plugin.Log.LogDebug("Duplicate teleport event, ignoring");
            return;
        }

        LastTeleportEventTime = DateTime.UtcNow;

        var description = $"Teleported by {teleporterType} teleporter";
        Plugin.Log.LogDebug(description);

        if (!Plugin.ClipConfig.ClipTeleporter.Value)
        {
            return;
        }

        try
        {
            var timelineEvent = SteamTimeline.AddInstantaneousTimelineEvent(
                GetTimelineMessage(teleporterType),
                description,
                "steam_transfer",
                0,
                0,
                TimelineEventClipPriority.Standard
            );
            Plugin.Log.LogDebug($"Added timeline event {timelineEvent}.");
        }
        catch (Exception e)
        {
            Plugin.Log.LogError($"Failed to add timeline event: {e}");
        }
    }

    private static string GetTimelineMessage(TeleporterType teleporterType)
    {
        return teleporterType switch
        {
            TeleporterType.Normal => Player.isPlayerDead ? "Saved" : "Yoinked",
            TeleporterType.Inverse => "Inversed",
            _ => "Teleported" // Unknown teleporter type
        };
    }
}