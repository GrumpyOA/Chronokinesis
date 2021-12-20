using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using AbilityUser;
using Verse;
using UnityEngine;


namespace ControlTimeMod
{
    public class Verb_TimeStop : Verb_UseAbility
    {

        //Effect when the time stop ability is activated
        protected override bool TryCastShot()
        {
            Pawn pawn = this.currentTarget.Thing as Pawn;

            bool flag = pawn != null && !pawn.Dead;
            if (flag)
            {

                //CheckStopTime.curTimeSpeed = isTimeStop.Paused;
                HealthUtility.AdjustSeverity(pawn, TimeStopDefOf.TimeStopAbilityHD, .5f);
                HealthUtility.AdjustSeverity(pawn, TimeStopDefOf.TimeStopShieldHD, .5f);
                //Log.Message("TimeStop!");
            }

            return true;
        }
    }
}
