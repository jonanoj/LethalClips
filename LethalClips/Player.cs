using GameNetcodeStuff;

namespace LethalClips;


public static class Player {
    public static PlayerControllerB Local => GameNetworkManager.Instance.localPlayerController;

    public static PlayerControllerB FromID(int id) {
        var players = StartOfRound.Instance.allPlayerScripts;
        return 0 <= id && id < players.Length ? players[id] : null;
    }

    public static bool IsLocal(int playerId) {
        return IsLocal(FromID(playerId));
    }

    public static bool IsLocal(PlayerControllerB player) {
        return player == Local;
    }
}
