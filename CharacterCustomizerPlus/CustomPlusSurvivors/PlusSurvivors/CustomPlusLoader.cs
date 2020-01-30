using BepInEx.Configuration;
using RoR2;

namespace CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors
{
    public class CustomPlusLoader : CustomPlusSurvivor
    {
        public CustomPlusLoader(ConfigFile file) : base(SurvivorIndex.Loader, "Loader",
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