using System;
using RimWorld;
using Verse;
using AbilityUser;

//Set def
namespace ControlTimeMod
{
	[DefOf]
	public static class TimeStopDefOf
	{
		public static AbilityUser.AbilityDef TimeStopAbility;

		public static HediffDef TimeStopAbilityHD;
		public static HediffDef TimeStopShieldHD;

		public static ThingDef TimeStopEffect;

		public static TraitDef ControlTimeUser;

		public static JobDef ControlTimeCastAbilityVerb;
	}
}
