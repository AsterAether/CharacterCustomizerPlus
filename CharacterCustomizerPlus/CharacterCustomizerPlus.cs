using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;

namespace CharacterCustomizerPlus
{
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("at.aster.aetherlib")]
    [BepInDependency("at.aster.charactercustomizer")]
    [BepInPlugin("at.aster.charactercustomizerplus", "CharacterCustomizerPlus", "1.0.0")]
    public class CharacterCustomizerPlus : BaseUnityPlugin
    {
	}
}