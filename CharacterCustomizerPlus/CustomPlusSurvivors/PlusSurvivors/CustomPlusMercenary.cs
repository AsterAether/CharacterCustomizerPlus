using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;
using CharacterCustomizer.Util.Config;
using RoR2;

namespace CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors
{
    public class CustomPlusMercenary : CustomPlusSurvivor
    {
        private readonly FieldChangerBag _dashFields;

        public CustomPlusMercenary(ConfigFile file, ManualLogSource logger) : base("Mercenary", file, logger)
        {
            _dashFields = new FieldChangerBag(this);
        }

        protected override void InitConfigValues()
        {
            _dashFields.AddFieldConfig<int>("DashMaxCount",
                "Maximum amount of dashes Mercenary can perform.", "maxDashes");

            _dashFields.AddFieldConfig<float>("DashTimeoutDuration",
                "Maximum timeout between dashes, in seconds", "timeoutDuration");
        }

        protected override void OverrideGameValues()
        {
            On.RoR2.Skills.MercDashSkillDef.OnExecute += (orig, self, slot) =>
            {
                _dashFields.Apply(self);
                orig(self, slot);
            };
        }

        protected override void WriteNewHooks()
        {
        }
    }
}