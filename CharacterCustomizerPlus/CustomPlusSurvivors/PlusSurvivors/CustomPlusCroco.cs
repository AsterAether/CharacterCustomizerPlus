using BepInEx.Configuration;
using RoR2;

namespace CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors
{
    public class CustomPlusCroco : CustomPlusSurvivor
    {
        public CustomPlusCroco(ConfigFile file) : base(SurvivorIndex.Croco, "Acrid",
            file)
        {
        }

        public override void InitConfigValues()
        {
        }

        public override void OverrideGameValues()
        {
        }

        public override void WriteNewHooks()
        {
        }
    }
}