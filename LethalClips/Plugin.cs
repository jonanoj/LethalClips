using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace LethalClips;


[BepInPlugin(GUID, NAME, VERSION)]
public class Plugin : BaseUnityPlugin {
    const string GUID = "com.lalabuff.lethal.lethalclips";
    const string NAME = "LethalClips";
    const string VERSION = "0.0.4";

    internal static ManualLogSource Log;


    internal void Awake()
    {
        Log = Logger;
        LethalClips.Config.Initialize(Config);

        if (!LethalClips.Config.General.Enabled.Value)
        {
            Log.LogWarning($"Mod is disabled. No patching will occur. To enable the mod, edit the configuration file and restart the game.");
            return;
        }

        Harmony harmony = new(GUID);
        harmony.PatchAll();
        foreach(var item in harmony.GetPatchedMethods()) {
            Log.LogInfo($"Patched method {item}.");
        }
        Log.LogInfo($"Successfully loaded {NAME} ({GUID}) v{VERSION}!");
    }
}
