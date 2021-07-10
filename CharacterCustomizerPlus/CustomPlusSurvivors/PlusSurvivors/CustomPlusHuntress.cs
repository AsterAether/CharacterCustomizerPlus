using System.Collections.Generic;
using BepInEx.Configuration;
using CharacterCustomizer.Util.Config;
using RoR2;

namespace CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors
{
    //
    //     public class CustomPlusHuntress : CustomPlusSurvivor
    //     {
    //         public FieldConfigWrapper<float> TrackingMaxDistance;
    //
    //         public FieldConfigWrapper<float> TrackingMaxAngle;
    //
    //         public List<IFieldChanger> TrackingFields;
    //
    //         public CustomPlusHuntress(ConfigFile file) : base(SurvivorIndex.Huntress, "Huntress",file)
    //         {
    //         }
    //
    //         public override void InitConfigValues()
    //         {
    //             TrackingMaxDistance = new FieldConfigWrapper<float>(BindConfigFloat("TrackingMaxDistance",
    //                 "The maximum distance the tracking of the huntress works."), "maxTrackingDistance");
    //
    //
    //             TrackingMaxAngle = new FieldConfigWrapper<float>(BindConfigFloat("TrackingMaxAngle",
    //                 "The maximum angle the tracking of the huntress works."), "maxTrackingAngle");
    //
    //             TrackingFields = new List<IFieldChanger>
    //             {
    //                 TrackingMaxAngle, TrackingMaxDistance
    //             };
    //         }
    //
    //         public override void OverrideGameValues()
    //         {
    //             On.RoR2.HuntressTracker.Awake += (orig, self) =>
    //             {
    //                 orig(self);
    //
    //                 TrackingFields.ForEach(changer => changer.Apply(self));
    //             };
    //         }
    //
    //         public override void WriteNewHooks()
    //         {
    //         }
    //     
    // }
}