using HarmonyLib;
using UnityEngine;

namespace LethalClips.Patches;

using P = EnemyAI;


[HarmonyPatch(typeof(P), nameof(P.OnCollideWithPlayer))]
internal class DamagePatch {
    private static void Prefix(
        P __instance,
        Collider other
    ) {
        const int KILL = -1;
        var overrideIsInsideFactoryCheck = __instance switch {
            DressGirlAI => true,
            _ => false
        };

        if(!__instance.MeetsStandardPlayerCollisionConditions(other, false, overrideIsInsideFactoryCheck)) {
            return;
        }

        var (cause, source, damage) = __instance switch {
            ClaySurgeonAI => (TranslatedCauseOfDeath.Snipped, "Barber", KILL),
            RedLocustBees => (TranslatedCauseOfDeath.Electrocuted, "Circuit Bees", 10),
            ButlerEnemyAI => (TranslatedCauseOfDeath.Stabbed, "Butler", 10),
            SpringManAI => (TranslatedCauseOfDeath.Springed, "Coil-Head", 90),
            BushWolfEnemy => (TranslatedCauseOfDeath.Mauled, "Kidnapper Fox", KILL),
            DressGirlAI => (TranslatedCauseOfDeath.Died, "", KILL),
            BaboonBirdAI => (TranslatedCauseOfDeath.Stabbed, "Baboon Hawk", 20),
            ButlerBeesEnemyAI => (TranslatedCauseOfDeath.Stabbed, "Mask Hornets", 10),
            PufferAI => (TranslatedCauseOfDeath.Embarrassing, "", 20),
            HoarderBugAI => (TranslatedCauseOfDeath.Mauled, "Hoarding Bug", 30),
            CaveDwellerAI => (TranslatedCauseOfDeath.Mauled, "Maneater", KILL),
            NutcrackerEnemyAI => (TranslatedCauseOfDeath.Kicked, "Nutcracker", KILL),
            BlobAI => (TranslatedCauseOfDeath.Disintegrated, "Hydrogere", 35),
            SandSpiderAI => (TranslatedCauseOfDeath.Mauled, "Bunker Spider", 90),
            CrawlerAI => (TranslatedCauseOfDeath.Mauled, "Thumper", 40),
            SandWormAI => (TranslatedCauseOfDeath.Devoured, "Earth Leviathan", KILL),
            _ => default
        };

        // TODO: get enemy internal timers to check if they can actually damage the player
        if(damage == KILL) {
            KillPatch.Kill(cause, source);
        } else {
            KillPatch.Damage(cause, source, damage);
        }
    }
}
