using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using AbilityUser;
using UnityEngine;

namespace ControlTimeMod
{
    public class ControlTimeJobDriver_CastAbilityVerb : JobDriver_CastAbilityVerb
    {
        private int duration;
        public AbilityContext context => job.count == 1 ? AbilityContext.Player : AbilityContext.AI;
        public Verb_UseAbility verb = new Verb_UseAbility(); // = this.pawn.CurJob.verbToUse as Verb_UseAbility;
        private bool wildCheck = false;

        //public override bool TryMakePreToilReservations(bool errorOnFailed)
        //{
        //    if(TargetA.Thing != null)
        //    {
        //        return true;
        //    }
        //    if (pawn.Reserve(TargetA, this.job, 1, 1, null, errorOnFailed))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Misc.ThrowColonistAttackingMote(TargetIndex.A);
            this.verb = this.pawn.CurJob.verbToUse as Verb_UseAbility;
            if (base.TargetA.HasThing && base.TargetA.Thing is Pawn && (!pawn.Position.InHorDistOf(base.TargetA.Cell, pawn.CurJob.verbToUse.verbProps.range) || !Verb.UseAbilityProps.canCastInMelee))
            {
                //if (!base.GetActor().IsFighting() ? true : !verb.UseAbilityProps.canCastInMelee && !this.job.endIfCantShootTargetFromCurPos)
                //{
                Toil toil = Toils_Combat.GotoCastPosition(TargetIndex.A);
                yield return toil;
                //toil = null;
                //}
            }
            if (this.Context == AbilityContext.Player)
            {
                Find.Targeter.targetingSource = this.verb;
            }
            Pawn targetPawn = null;
            if (this.TargetThingA != null)
            {
                targetPawn = TargetThingA as Pawn;
            }

            if (targetPawn != null)
            {
                //yield return Toils_Combat.CastVerb(TargetIndex.A, false);
                Toil combatToil = new Toil();
                //combatToil.FailOnDestroyedOrNull(TargetIndex.A);
                //combatToil.FailOnDespawnedOrNull(TargetIndex.A);
                //combatToil.FailOnDowned(TargetIndex.A);
                //CompAbilityUserMagic comp = this.pawn.GetComp<CompAbilityUserMagic>();                
                //JobDriver curDriver = this.pawn.jobs.curDriver;
                combatToil.initAction = delegate
                {
                    this.verb = combatToil.actor.jobs.curJob.verbToUse as Verb_UseAbility;
                    if (verb != null && verb.verbProps != null)
                    {
                        this.duration = (int)((this.verb.verbProps.warmupTime * 60) * this.pawn.GetStatValue(StatDefOf.AimingDelayFactor, false));

                        if (this.pawn.RaceProps.Humanlike)
                        {
                            //if (this.pawn.story.traits.HasTrait(TorannMagicDefOf.Faceless))
                            //{
                            //    RemoveMimicAbility(verb);
                            //}


                            if (verb.Ability?.CooldownTicksLeft != -1)
                            {
                                this.EndJobWith(JobCondition.Incompletable);
                            }
                        }


                        LocalTargetInfo target = combatToil.actor.jobs.curJob.GetTarget(TargetIndex.A);
                        if (target != null)
                        {
                            verb.TryStartCastOn(target, false, true);
                        }
                        using (IEnumerator<Hediff> enumerator = this.pawn.health.hediffSet.GetHediffs<Hediff>().GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                Hediff rec = enumerator.Current;
                            }
                        }
                    }
                    else
                    {
                        this.EndJobWith(JobCondition.Errored);
                    }
                };
                combatToil.tickAction = delegate
                {
                    if (this.pawn.Downed)
                    {
                        EndJobWith(JobCondition.InterruptForced);
                    }
                    if (Find.TickManager.TicksGame % 12 == 0)
                    {

                    }

                    this.duration--;
                    if (!wildCheck && this.duration <= 6)
                    {
                        wildCheck = true;

                    }
                };
                combatToil.AddFinishAction(delegate
                {
                    if (this.duration <= 5 && !this.pawn.DestroyedOrNull() && !this.pawn.Dead && !this.pawn.Downed)
                    {
                        //ShootLine shootLine;
                        //bool validTarg = verb.TryFindShootLineFromTo(pawn.Position, TargetLocA, out shootLine);
                        //bool inRange = (pawn.Position - TargetLocA).LengthHorizontal < verb.verbProps.range;
                        //if (inRange && validTarg)
                        //{
                        /*TMAbilityDef tmad = (TMAbilityDef)(verb.Ability.Def);
                        if (tmad != null && tmad.relationsAdjustment != 0 && targetPawn.Faction != null && targetPawn.Faction != this.pawn.Faction && !targetPawn.Faction.HostileTo(this.pawn.Faction))
                        {
                            targetPawn.Faction.TryAffectGoodwillWith(this.pawn.Faction, tmad.relationsAdjustment, true, false, null, null);
                        }*/
                        verb.Ability.PostAbilityAttempt();
                        this.pawn.ClearReservationsForJob(this.job);
                        //}
                    }
                });
                //if (combatToil.actor.CurJob != this.job)
                //{
                //    curDriver.ReadyForNextToil();
                //}
                combatToil.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
                this.pawn.ClearReservationsForJob(this.job);
                yield return combatToil;
                //Toil toil2 = new Toil()
                //{
                //    initAction = () =>
                //    {
                //        if (curJob.UseAbilityProps.isViolent)
                //        {
                //            JobDriver_CastAbilityVerb.CheckForAutoAttack(this.pawn);
                //        }
                //    },
                //    defaultCompleteMode = ToilCompleteMode.Instant
                //};
                //yield return toil2;
                //Toil toil1 = new Toil()
                //{
                //    initAction = () => curJob.Ability.PostAbilityAttempt(),
                //    defaultCompleteMode = ToilCompleteMode.Instant
                //};
                //yield return toil1;
            }
            else
            {
                if (verb != null && verb.verbProps != null && (pawn.Position - TargetLocA).LengthHorizontal < verb.verbProps.range)
                {
                    if (TargetLocA.IsValid && TargetLocA.InBounds(pawn.Map) && !TargetLocA.Fogged(pawn.Map))  //&& TargetLocA.Walkable(pawn.Map)
                    {
                        ShootLine shootLine;
                        bool validTarg = verb.TryFindShootLineFromTo(pawn.Position, TargetLocA, out shootLine);
                        if (validTarg)
                        {
                            //yield return Toils_Combat.CastVerb(TargetIndex.A, false);
                            //Toil toil2 = new Toil()
                            //{
                            //    initAction = () =>
                            //    {
                            //        if (curJob.UseAbilityProps.isViolent)
                            //        {
                            //            JobDriver_CastAbilityVerb.CheckForAutoAttack(this.pawn);
                            //        }

                            //    },
                            //    defaultCompleteMode = ToilCompleteMode.Instant
                            //};
                            //yield return toil2;
                            this.duration = (int)((verb.verbProps.warmupTime * 60) * this.pawn.GetStatValue(StatDefOf.AimingDelayFactor, false));
                            Toil toil = new Toil();
                            toil.initAction = delegate
                            {
                                this.verb = toil.actor.jobs.curJob.verbToUse as Verb_UseAbility;
                                if (this.pawn.RaceProps.Humanlike)
                                {
                                    //if (this.pawn.story.traits.HasTrait(TorannMagicDefOf.Faceless))
                                    //{
                                    //    RemoveMimicAbility(verb);                                        
                                    //}


                                    if (verb.Ability?.CooldownTicksLeft != -1)
                                    {
                                        this.EndJobWith(JobCondition.Incompletable);
                                    }

                                }
                                LocalTargetInfo target = toil.actor.jobs.curJob.GetTarget(TargetIndex.A); //TargetLocA;  //
                                bool canFreeIntercept2 = false;
                                if (target != null)
                                {
                                    verb.TryStartCastOn(target, false, canFreeIntercept2);
                                }
                                using (IEnumerator<Hediff> enumerator = this.pawn.health.hediffSet.GetHediffs<Hediff>().GetEnumerator())
                                {
                                    while (enumerator.MoveNext())
                                    {
                                        Hediff rec = enumerator.Current;
                                    }
                                }
                            };
                            toil.tickAction = delegate
                            {
                                if (Find.TickManager.TicksGame % 12 == 0)
                                {

                                }
                                this.duration--;
                                if (!wildCheck && this.duration <= 6)
                                {
                                    wildCheck = true;

                                }
                            };
                            toil.AddFinishAction(delegate
                            {
                                if (this.duration <= 5 && !this.pawn.DestroyedOrNull() && !this.pawn.Dead && !this.pawn.Downed)
                                {
                                    verb.Ability.PostAbilityAttempt();
                                }
                                this.pawn.ClearReservationsForJob(this.job);
                            });
                            toil.defaultCompleteMode = ToilCompleteMode.FinishedBusy;
                            yield return toil;

                            //Toil toil1 = new Toil()
                            //{
                            //    initAction = () => curJob.Ability.PostAbilityAttempt(),
                            //    defaultCompleteMode = ToilCompleteMode.Instant
                            //};
                            //yield return toil1;
                        }
                        else
                        {
                            //No LoS
                            if (pawn.IsColonist)
                            {
                                Messages.Message("TM_OutOfLOS".Translate(
                                    pawn.LabelShort
                                ), MessageTypeDefOf.RejectInput);
                            }
                            pawn.ClearAllReservations(false);
                        }
                    }
                    else
                    {
                        pawn.ClearAllReservations(false);
                    }
                }
                else
                {
                    if (pawn.IsColonist)
                    {
                        //out of range
                        Messages.Message("TM_OutOfRange".Translate(), MessageTypeDefOf.RejectInput);
                    }
                }
            }
        }

    }
}
