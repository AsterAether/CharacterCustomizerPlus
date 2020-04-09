using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;
using CharacterCustomizer.Util.Config;
using CharacterCustomizer.Util.Reflection;
using MonoMod.Cil;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors
{
    
        public class CustomPlusCommando : CustomPlusSurvivor
        {
            public CustomPlusCommando(ConfigFile file) : base(SurvivorIndex.Commando, "Commando", file)
            {
            }

            public ConfigEntryDescriptionWrapper<bool> DoubleTapHitLowerSpecialCooldown;

            public ConfigEntryDescriptionWrapper<float> DoubleTapHitLowerSpecialCooldownPercent;

            public FieldConfigWrapper<float> DoubleTapDamageCoefficient;

            public FieldConfigWrapper<float> DoubleTapBaseDuration;

            public List<IFieldChanger> DoubleTapFields;

            public FieldConfigWrapper<float> PhaseRoundDamageCoefficient;

            public List<IFieldChanger> PhaseRoundFields;
            
            public FieldConfigWrapper<float> SuppressiveFireBaseDurationBetweenShots;

            public FieldConfigWrapper<int> SuppressiveFireBaseShotAmount;

            public List<IFieldChanger> SuppressiveFireFields;

            public override void InitConfigValues()
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

                // Laser

                PhaseRoundDamageCoefficient = new FieldConfigWrapper<float>(BindConfigFloat("PhaseRoundDamageCoefficient",
                    "Damage coefficient for the secondary phase round, in percent."), "damageCoefficient");

                PhaseRoundFields = new List<IFieldChanger>
                {
                    PhaseRoundDamageCoefficient
                };
                
                // Barrage

                SuppressiveFireBaseShotAmount =
                    new FieldConfigWrapper<int>(
                        BindConfigInt("SuppressiveFireBaseShotAmount", "How many shots the Suppressive Fire skill should when ATKSP = 1"),
                        "baseBulletCount", true);


                SuppressiveFireBaseDurationBetweenShots =
                    new FieldConfigWrapper<float>(BindConfigFloat("SuppressiveFireBaseDurationBetweenShots",
                        "Base duration between shots in the Suppressive Fire skill."), "baseDurationBetweenShots", true);

                SuppressiveFireFields = new List<IFieldChanger> {SuppressiveFireBaseShotAmount, SuppressiveFireBaseDurationBetweenShots};
            }

            public override void OverrideGameValues()
            {
                On.RoR2.RoR2Application.Start += (orig, self) =>
                {
                    orig(self);
                    Assembly assembly = self.GetType().Assembly;
                
                    Type firePistol = assembly.GetClass("EntityStates.Commando.CommandoWeapon", "FirePistol2");
                
                    DoubleTapFields.ForEach(changer => changer.Apply(firePistol));
                    
                    Type fireBarr = assembly.GetClass("EntityStates.Commando.CommandoWeapon", "FireBarrage");
                
                    SuppressiveFireFields.ForEach(changer => changer.Apply(fireBarr));
                };
            }

            public override void WriteNewHooks()
            {
                On.EntityStates.Commando.CommandoWeapon.FireFMJ.OnEnter += (orig, self) =>
                {
                    PhaseRoundFields.ForEach(changer => changer.Apply(self));
                    orig(self);
                };
                
                // On.EntityStates.Commando.DodgeState.OnEnter += (orig, self) =>
                // {
                //     orig(self);
                //
                //     if (DashInvulnerability.Value)
                //     {
                //         if (DashInvulnerabilityTimer.IsDefault())
                //         {
                //             Transform transform = self.InvokeMethod<Transform>("GetModelTransform");
                //
                //             HurtBoxGroup hurtBoxGroup = transform.GetComponent<HurtBoxGroup>();
                //             ++hurtBoxGroup.hurtBoxesDeactivatorCounter;
                //         }
                //         else
                //         {
                //             self.outer.commonComponents.characterBody.AddTimedBuff(BuffIndex.HiddenInvincibility,
                //                 DashInvulnerabilityTimer.Value);
                //         }
                //     }
                // };
                
                // On.EntityStates.Commando.DodgeState.OnExit += (orig, self) =>
                // {
                //     if (DashInvulnerability.Value && DashInvulnerabilityTimer.IsDefault())
                //     {
                //         Transform transform = self.InvokeMethod<Transform>("GetModelTransform");
                //
                //         HurtBoxGroup hurtBoxGroup = transform.GetComponent<HurtBoxGroup>();
                //         --hurtBoxGroup.hurtBoxesDeactivatorCounter;
                //     }
                //
                //     orig(self);
                // };
                
                
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