using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;
using CharacterCustomizer.Util.Config;
using CharacterCustomizer.Util.Reflection;
using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors
{
    public class CustomPlusArtificer : CustomPlusSurvivor
    {

        public CustomPlusArtificer(ConfigFile file) : base(SurvivorIndex.Mage, "Artificer", file)
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