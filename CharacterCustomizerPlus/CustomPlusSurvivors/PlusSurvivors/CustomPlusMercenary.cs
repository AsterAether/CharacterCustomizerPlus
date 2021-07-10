using System.Collections.Generic;
using BepInEx.Configuration;
using CharacterCustomizer.Util.Config;
using RoR2;

namespace CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors
{
        //
        // public class CustomPlusMercenary : CustomPlusSurvivor
        // {
        //     public FieldConfigWrapper<int> DashMaxCount;
        //
        //     public FieldConfigWrapper<float> DashTimeoutDuration;
        //
        //     public List<IFieldChanger> DashFields;
        //
        //     public override void InitConfigValues()
        //     {
        //         DashMaxCount = new FieldConfigWrapper<int>(BindConfigInt("DashMaxCount",
        //             "Maximum amount of dashes Mercenary can perform."), "maxDashes");
        //
        //         DashTimeoutDuration = new FieldConfigWrapper<float>(BindConfigFloat("DashTimeoutDuration",
        //             "Maximum timeout between dashes, in seconds"), "timeoutDuration");
        //
        //         DashFields = new List<IFieldChanger>
        //         {
        //             DashMaxCount, DashTimeoutDuration
        //         };
        //     }
        //
        //     public CustomPlusMercenary(ConfigFile file) : base(SurvivorIndex.Merc, "Mercenary", file)
        //     {
        //     }
        //
        //
        //     public override void OverrideGameValues()
        //     {
        //         On.RoR2.Skills.MercDashSkillDef.OnExecute += (orig, self, slot) => 
        //         {
        //             DashFields.ForEach(changer => changer.Apply(self));
        //
        //             orig(self, slot);
        //         };
        //     }
        //
        //     public override void WriteNewHooks()
        //     {
        //     }
        // }
        //
}