using BepInEx.Configuration;
using RoR2;

namespace CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors
{
    public class CustomPlusTreebot : CustomPlusSurvivor
    {
        public CustomPlusTreebot(ConfigFile file) : base(SurvivorIndex.Treebot, "REX",
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