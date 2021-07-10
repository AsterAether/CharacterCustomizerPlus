using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;
using BepInEx.Logging;
using CharacterCustomizer.Util.Config;
using CharacterCustomizer.Util.Reflection;
using EntityStates.Commando.CommandoWeapon;
using MonoMod.Cil;
using R2API.Utils;
using RoR2;
using UnityEngine;
using Logger = UnityEngine.Logger;

namespace CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors
{
    
        public class CustomPlusCommando : CustomPlusSurvivor
        {
            public CustomPlusCommando(ConfigFile file, ManualLogSource logger) : base("Commando", file, logger)
            {
            }

            public ConfigEntryDescriptionWrapper<bool> DoubleTapHitLowerSpecialCooldown;
        
            public ConfigEntryDescriptionWrapper<float> DoubleTapHitLowerSpecialCooldownPercent;
        
            public FieldConfigWrapper<float> DoubleTapDamageCoefficient;
        
            public FieldConfigWrapper<float> DoubleTapBaseDuration;
        
            public List<IFieldChanger> DoubleTapFields;

            protected override void InitConfigValues()
            {
                // Pistol
                DoubleTapDamageCoefficient = new FieldConfigWrapper<float>(BindConfigFloat("DoubleTapDamageCoefficient",
                    "Damage coefficient for the Double Tap, in percent."), "damageCoefficient", true);
        
                DoubleTapBaseDuration =
                    new FieldConfigWrapper<float>(BindConfigFloat("DoubleTapBaseDuration",
                        "Base duration for the Double Tap shot, in percent. (Attack Speed)"), "baseDuration", true);
        
                DoubleTapFields = new List<IFieldChanger>
                {
                    DoubleTapBaseDuration,
                    DoubleTapDamageCoefficient
                };
        
                DoubleTapHitLowerSpecialCooldownPercent = BindConfigFloat("DoubleTapHitLowerSpecialCooldownPercent",
                    "The amount in percent that the current cooldown of the Barrage Skill should be lowered by. Needs to have DoubleTapHitLowerSpecialCooldown set.");
        
        
                DoubleTapHitLowerSpecialCooldown =
                    BindConfigBool("DoubleTapHitLowerSpecialCooldown",
                        "If the pistol hit should lower the Special Skill cooldown. Needs to have DoubleTapHitLowerSpecialCooldownPercent set to work");
            }
        
            protected override void OverrideGameValues()
            {
                var assembly = SurvivorDef.GetType().Assembly;
                
                var firePistol = assembly.GetClass("EntityStates.Commando.CommandoWeapon", "FirePistol2");
                DoubleTapFields.ForEach(changer => changer.Apply(firePistol));
            }
        
            protected override void WriteNewHooks()
            {
                if (DoubleTapHitLowerSpecialCooldown.Value && DoubleTapHitLowerSpecialCooldownPercent.IsNotDefault())
                {
                    Type gsType = typeof(GenericSkill);
                    FieldInfo finalRechargeInterval = gsType.GetField("finalRechargeInterval",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                
                    IL.EntityStates.Commando.CommandoWeapon.FirePistol2.FireBullet += il =>
                    {
                        ILCursor c = new ILCursor(il);
                
                        c.GotoNext(x => x.MatchCallvirt(typeof(RoR2.BulletAttack).FullName, "Fire"));
                        c.EmitDelegate<Func<BulletAttack, BulletAttack>>((BulletAttack ba) =>
                        {
                            ba.hitCallback = (ref BulletAttack.BulletHit info) =>
                            {
                                bool result = ba.DefaultHitCallback(ref info);
                                if (info.entityObject?.GetComponent<HealthComponent>())
                                {
                                    SkillLocator skillLocator = ba.owner.GetComponent<SkillLocator>();
                                    GenericSkill special = skillLocator.special;
                                    if (special.IsReady()) return result;
                                    special.rechargeStopwatch = special.rechargeStopwatch +
                                        (float) finalRechargeInterval.GetValue(special) *
                                        DoubleTapHitLowerSpecialCooldownPercent.Value;
                                }
                                
                                return result;
                            };
                            return ba;
                        });
                    };
                }
            }
        }
}