using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace LethalClips;


[BepInPlugin(GUID, NAME, VERSION)]
public class Plugin : BaseUnityPlugin {
    const string GUID = "com.lalabuff.lethal.lethalclips";
    const string NAME = "LethalClips";
    const string VERSION = "0.0.2";

    internal static ManualLogSource Log;


    internal void Awake() {
        Log = Logger;

        Harmony harmony = new(GUID);
        harmony.PatchAll();
        foreach(var item in harmony.GetPatchedMethods()) {
            Log.LogInfo($"Patched method {item}.");
        }
        Log.LogInfo($"Successfully loaded {NAME} ({GUID}) v{VERSION}!");
    }
}
