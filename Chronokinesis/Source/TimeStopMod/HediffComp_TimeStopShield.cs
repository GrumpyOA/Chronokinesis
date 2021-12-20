using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ControlTimeMod
{
    [StaticConstructorOnStartup]
    public class HediffComp_TimeStopShield : HediffComp
    {
        private float energy;

        private bool initializing = true;
        public string labelCap
        {
            get
            {
                return base.Def.LabelCap;
            }
        }

        public string label
        {
            get
            {
                return base.Def.label;
            }
        }

        private float EnergyLossPerTick
        {
            get
            {
                return 0.000166667f;
            }
        }
        private void Initialize()
        {
            this.energy = 0.35f;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            bool flag = base.Pawn != null;
            if (flag)
            {
                if (initializing)
                {
                    initializing = false;
                    this.Initialize();
                }
            }

            Pawn pawn = base.Pawn;

            this.energy -= this.EnergyLossPerTick;
            bool flag5 = this.energy <= 0.2;
            if (flag5)
            {
                //CheckStopTime.curTimeSpeed = isTimeStop.Normal;
                severityAdjustment = -10f;
            }

            TickProtection();

            base.Pawn.SetPositionDirect(base.Pawn.Position);
        }

        public void TickProtection()
        {

            if (CellsToProtect == null)
            {
                ReCalibrateCells();
            }

            foreach (IntVec3 square in this.CellsToProtect)
            {
                ProtectSquare(square);
            }

        }

        private List<IntVec3> CellsToProtect;

        public void ReCalibrateCells()
        {

            this.CellsToProtect = new List<IntVec3>();

            Pawn pawn = base.Pawn;

            IEnumerable<IntVec3> _AllSquares = GenRadial.RadialCellsAround(pawn.Position, 2f, false);

            foreach (IntVec3 _Square in _AllSquares)
            {
                //if (Vectors.VectorSize(_Square) >= (float)this.m_Field_Radius - 1.5f)
                if (Vectors.EuclDist(_Square, pawn.Position) >= 2f - 1.5f)
                {
                    this.CellsToProtect.Add(_Square);

                }
            }
        }

        virtual public void ProtectSquare(IntVec3 square)
        {
            Pawn pawn = base.Pawn;

            Map Maps = pawn.Map;

            /*if (!square.InBounds(Maps))
            {
                return;
            }*/

            List<Thing> things = Maps.thingGrid.ThingsListAt(square);
            List<Thing> thingsToDestroy = new List<Thing>();

            for (int i = 0, l = things.Count(); i < l; i++)
            {
                if (things[i] != null && things[i] is Projectile)
                {
                    Projectile pr = (Projectile)things[i];
                    if (!pr.Destroyed && !pr.def.projectile.flyOverhead)
                    {
                        bool wantToIntercept = true; //弾丸を止める

                        if (wantToIntercept)
                        {

                            if (pr.def != ThingDef.Named("Projectile_OraPunch"))
                            {
                                //Detect proper collision using angles
                                Quaternion targetAngle = pr.ExactRotation;

                                Vector3 projectilePosition2D = pr.ExactPosition;
                                projectilePosition2D.y = 0;

                                Vector3 shieldPosition2D = Vectors.IntVecToVec(pawn.Position);
                                shieldPosition2D.y = 0;

                                Quaternion shieldProjAng = Quaternion.LookRotation(projectilePosition2D - shieldPosition2D);

                                if ((Quaternion.Angle(targetAngle, shieldProjAng) > 90))
                                {
                                    //On hit effects
                                    //MoteMaker.ThrowLightningGlow(pr.ExactPosition, Maps, 0.5f);

                                    //On hit sound
                                    //HitSoundDef.PlayOneShot((SoundInfo)new TargetInfo(this.Position, this.Map, false));

                                    //add projectile to the list of things to be destroyed
                                    thingsToDestroy.Add(pr);
                                }
                            }
                        }
                    }
                }
            }

            foreach (Thing currentThing in thingsToDestroy)
            {
                currentThing.Destroy();
            }

        }
    }
}

