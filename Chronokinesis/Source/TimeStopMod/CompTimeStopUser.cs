using AbilityUser;
using RimWorld;
using System;
using System.Reflection;
using Verse;
using Verse.Sound;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Verse.AI;
using System.Linq;
using System.Runtime.CompilerServices;
using AbilityUserAI;

//Give a time stop button
namespace ControlTimeMod
{
    [CompilerGenerated]
    [Serializable]
    [StaticConstructorOnStartup]
    public class CompTimeStopUser : CompAbilityUser
    {

        private bool hasAbilities = true;

        private bool IsStandHas
        {
            get
            {
                if (!Pawn.story.traits.HasTrait(TimeStopDefOf.ControlTimeUser)) return false;
                return true;
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            if (Pawn.Spawned != true && !this.Pawn.IsColonist) return;

            if (IsStandHas)
            {
                CheckStandOnTick();
            }
        }

        private void CheckStandOnTick()
        {
            if (Pawn.Spawned != true) return;
            if (Pawn.story == null) return;

            if (hasAbilities)
            {
                this.Initialize();
                this.AddPawnAbility(TimeStopDefOf.TimeStopAbility);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.hasAbilities, "gaveAbilities", true);
        }
    }

}
