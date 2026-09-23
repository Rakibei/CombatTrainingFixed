using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace KriilMod_CD
{
    [StaticConstructorOnStartup]
    public static class CombatDummySoundPatch
    {
        static CombatDummySoundPatch()
        {
            new Harmony("Rakibei.CombatTrainingFixed.SilentDummy")
                .PatchAll();
        }
    }

    // Ranged/projectile impact sounds
    [HarmonyPatch]
    public static class Patch_ImpactSoundUtility
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return typeof(ImpactSoundUtility)
                .GetMethods(AccessTools.all)
                .Where(method => method.Name == "PlayImpactSound");
        }

        static bool Prefix(object[] __args)
        {
            return !TargetsCombatDummy(__args);
        }

        private static bool TargetsCombatDummy(object[] args)
        {
            foreach (object arg in args)
            {
                if (arg is Building_CombatDummy)
                    return true;

                if (arg is LocalTargetInfo target &&
                    target.Thing is Building_CombatDummy)
                    return true;
            }

            return false;
        }
    }

    // Melee weapon impact sounds against buildings
    [HarmonyPatch]
    public static class Patch_MeleeHitBuildingSound
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return typeof(Verb_MeleeAttack)
                .GetMethods(AccessTools.all)
                .Where(method => method.Name == "SoundHitBuilding");
        }

        static bool Prefix(object[] __args)
        {
            foreach (object arg in __args)
            {
                if (arg is Building_CombatDummy)
                    return false;

                if (arg is LocalTargetInfo target &&
                    target.Thing is Building_CombatDummy)
                    return false;
            }

            return true;
        }
    }
}