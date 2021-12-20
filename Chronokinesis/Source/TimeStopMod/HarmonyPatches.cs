using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
using Verse;
using Verse.Sound;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
using RimWorld.Planet;
using AbilityUser;


namespace ControlTimeMod
{
    [StaticConstructorOnStartup]
    public static class MyMod
    {
        static MyMod() //our constructor
        {
            var harmonyInstance = new Harmony("rimworld.timestopmod");

            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("Hello World!"); //Outputs "Hello World!" to the dev console.

        }
    }

    //Update tick
    [HarmonyPatch(typeof(TickManager), "TickManagerUpdate", new Type[]
    {

    })]
    public static class AllTickUpdate
    {
        public static bool Prefix(TickManager __instance)
        {
            //Traverse traverse = Traverse.Create(__instance);

            bool result;
            bool stopper = false;

            List<Map> allMaps = Find.Maps;

            /*if (Find.TickManager.CurTimeSpeed == TimeSpeed.Fast)
            {
                CheckStopTime.curTimeSpeed = isTimeStop.Paused;
            }
            else
            {
                CheckStopTime.curTimeSpeed = isTimeStop.Normal;
            }*/

            //Check if the world pawn has a time stop buff
            if (allMaps != null && allMaps.Count > 0)
            {
                for (int i = 0; i < allMaps.Count; i++)
                {
                    List<Pawn> mapPawns = allMaps[i].mapPawns.AllPawnsSpawned;
                    for (int j = 0; j < mapPawns.Count; j++)
                    {
                        bool flag4 = mapPawns[j].health != null && mapPawns[j].health.hediffSet != null && mapPawns[j].health.hediffSet.HasHediff(TimeStopDefOf.TimeStopAbilityHD, false);
                        if (flag4)
                        {
                            stopper = true;
                            break;
                        }
                    }

                    if (stopper)
                    {
                        break;
                    }
                }
            }

            if (stopper)
            {
                CheckStopTime.curTimeSpeed = isTimeStop.Paused;
            }
            else
            {
                CheckStopTime.curTimeSpeed = isTimeStop.Normal;
            }

            result = true;
            return result;
        }
    }

    //Update Pawn tick
    [HarmonyPatch(typeof(Pawn), "Tick", new Type[]
    {

    })]
    public static class PawnTickUpdate
    {
        public static bool Prefix(Pawn __instance)
        {

            Traverse traverse = Traverse.Create(__instance);

        /*  Pawn_PathFollower thing = traverse.Field("pather").GetValue<Pawn_PathFollower>();
            bool flag = thing != null;  */

            bool result = true;

            if (CheckStopTime.curTimeSpeed == isTimeStop.Paused)
            {
                if(__instance != null)
                {
                    if (__instance.health != null && __instance.health.hediffSet != null && !(__instance.health.hediffSet.HasHediff(TimeStopDefOf.TimeStopAbilityHD, false)))
                    {
                        result = false;
                    }
                }

                //result = false;
                return result;
            }

            return result;
        }
    }



    //Update Projectile tick
    [HarmonyPatch(typeof(Projectile), "Tick", new Type[]
    {

    })]
    public static class ProjectileTickUpdate
    {

        public static bool Prefix(Projectile __instance)
        {

            Traverse traverse = Traverse.Create(__instance);

            bool result = true;

            if (__instance.def == ThingDef.Named("Projectile_OraPunch"))
            {
                return result;
            }
            //int value = traverse.Field("ticksToImpact").GetValue<int>();

            //bool flag = thing != null;

            if (CheckStopTime.curTimeSpeed == isTimeStop.Paused)
            {
                result = false;
                return result;
            }

            return result;
        }
    }

    [HarmonyPatch(typeof(Projectile_Explosive), "Tick", new Type[]
       {

    })]
    public static class Projectile_ExplosiveTickUpdate
    {

        public static bool Prefix(Projectile __instance)
        {

            Traverse traverse = Traverse.Create(__instance);

            //int value = traverse.Field("ticksToImpact").GetValue<int>();

            //bool flag = thing != null;

            bool result = true;

            if (CheckStopTime.curTimeSpeed == isTimeStop.Paused)
            {
                result = false;
                return result;
            }

            return result;
        }
    }

    [HarmonyPatch(typeof(Sustainer), "End", new Type[]
   {

    })]
    public static class StopSustainerEnd
    {

        public static bool Prefix(Projectile __instance)
        {

            Traverse traverse = Traverse.Create(__instance);

            //int value = traverse.Field("ticksToImpact").GetValue<int>();

            //bool flag = thing != null;

            bool result = true;

            if (CheckStopTime.curTimeSpeed == isTimeStop.Paused)
            {
                result = false;
                return result;
            }

            return result;
        }
    }

    [HarmonyPatch(typeof(Map), "MapPreTick", new Type[]
    {

    })]
    public static class StopMapObject
    {

        public static bool Prefix(Projectile __instance)
        {

            Traverse traverse = Traverse.Create(__instance);

            //int value = traverse.Field("ticksToImpact").GetValue<int>();

            //bool flag = thing != null;

            bool result = true;

            if (CheckStopTime.curTimeSpeed == isTimeStop.Paused)
            {
                result = false;
                return result;
            }

            return result;
        }
    }

    [HarmonyPatch(typeof(AbilityUser.AbilityDef), "GetJob", null)]
    public static class AbilityDef_Patch
    {
        private static bool Prefix(AbilityUser.AbilityDef __instance, AbilityTargetCategory cat, LocalTargetInfo target, ref Job __result)
        {
            bool flag = __instance.abilityClass.FullName == "AbilityUser.PawnAbility";

            if (flag)
            {
                Job result;

                result = new Job(TimeStopDefOf.ControlTimeCastAbilityVerb, target);

                __result = result;
                return false;
            }


            return true;
        }
    }
}
