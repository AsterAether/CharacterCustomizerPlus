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

            public CustomPlusMulT(ConfigFile file) : base(SurvivorIndex.Toolbot, "MultT",
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