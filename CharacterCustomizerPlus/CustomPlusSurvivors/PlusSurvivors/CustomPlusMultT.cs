using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using CharacterCustomizer.Util.Config;
using EntityStates;
using RoR2;

namespace CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors
{
 
        public class CustomPlusMulT : CustomPlusSurvivor
        {
            public FieldConfigWrapper<float> NailgunSpreadYaw;

            public FieldConfigWrapper<float> NailgunSpreadPitch;

            public List<IFieldChanger> NailgunFields;

            public CustomPlusMulT(ConfigFile file) : base(SurvivorIndex.Toolbot, "MultT",
                file)
            {
            }

            public override void InitConfigValues()
            {
                NailgunSpreadYaw =
                    new FieldConfigWrapper<float>(
                        BindConfigFloat("NailgunSpreadYaw", "Yaw spread of the nailgun, in percent"), "spreadYawScale",
                        true);

                NailgunSpreadPitch =
                    new FieldConfigWrapper<float>(
                        BindConfigFloat("NailgunSpreadPitch", "Pitch spread of the nailgun, in percent"),
                        "spreadPitchScale", true);

                NailgunFields = new List<IFieldChanger>
                {
                    NailgunSpreadYaw, NailgunSpreadPitch
                };
            }

            public override void OverrideGameValues()
            {
                On.RoR2.RoR2Application.Start += (orig, self) =>
                {
                    orig(self);

                    Type fireNailgun = typeof(FireNailgun);

                    NailgunFields.ForEach(changer => changer.Apply(fireNailgun));
                };
            }

            public override void WriteNewHooks()
            {
            }
        }
    
}