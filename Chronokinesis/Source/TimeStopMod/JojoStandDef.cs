using AbilityUser;
using System;
using System.Text;

namespace ControlTimeMod
{
	public class JojoStandDef : AbilityUser.AbilityDef
    {
        public float manaCost = 0f;
        public float staminaCost = 0f;
        public float bloodCost = 0f;
        public float chiCost = 0f;
        public bool consumeEnergy = true;
        public int abilityPoints = 1;
        public float learnChance = 1f;
        public float efficiencyReductionPercent = 0f;
        public float upkeepEnergyCost = 0f;
        public float upkeepRegenCost = 0f;
        public float upkeepEfficiencyPercent = 0f;
        public bool shouldInitialize = true;
        public float weaponDamageFactor = 1f;
        public bool canCopy = false;

        public string GetPointDesc()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(this.GetDescription());
            return stringBuilder.ToString();
        }
    }
}
