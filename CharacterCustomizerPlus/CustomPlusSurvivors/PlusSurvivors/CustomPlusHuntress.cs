using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;
using CharacterCustomizer.Util.Config;
using RoR2;

namespace CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors
{
    public class CustomPlusHuntress : CustomPlusSurvivor
    {
        private readonly FieldChangerBag _trackingFields;


        public CustomPlusHuntress(ConfigFile file, ManualLogSource logger) : base("Huntress", file, logger)
        {
            _trackingFields = new FieldChangerBag(this);
        }


        protected override void InitConfigValues()
        {
            _trackingFields.AddFieldConfig<float>("TrackingMaxDistance",
                "The maximum distance the tracking of the huntress works.", "maxTrackingDistance");
            _trackingFields.AddFieldConfig<float>("TrackingMaxAngle",
                "The maximum angle the tracking of the huntress works.", "maxTrackingAngle");
        }

        protected override void OverrideGameValues()
        {
            On.RoR2.HuntressTracker.Awake += (orig, self) =>
            {
                orig(self);

                _trackingFields.Apply(self);
            };
        }

        protected override void WriteNewHooks()
        {
        }
    }
}