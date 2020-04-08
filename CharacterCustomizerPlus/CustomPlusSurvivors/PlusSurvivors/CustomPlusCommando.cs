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

            public ConfigEntryDescriptionWrapper<bool> PistolHitLowerBarrageCooldown;

            public ConfigEntryDescriptionWrapper<float> PistolHitLowerBarrageCooldownPercent;

            public FieldConfigWrapper<float> PistolDamageCoefficient;

            public FieldConfigWrapper<float> PistolBaseDuration;

            public List<IFieldChanger> PistolFields;

            public FieldConfigWrapper<float> LaserDamageCoefficient;

            public List<IFieldChanger> LaserFields;
            
            public FieldConfigWrapper<float> BarrageBaseDurationBetweenShots;

            public FieldConfigWrapper<int> BarrageBaseShotAmount;

            public List<IFieldChanger> BarrageFields;

            public override void InitConfigValues()
            {
                // Pistol
                PistolDamageCoefficient = new FieldConfigWrapper<float>(BindConfigFloat("PistolDamageCoefficient",
                    "Damage coefficient for the pistol, in percent."), "damageCoefficient", true);

                PistolBaseDuration =
                    new FieldConfigWrapper<float>(BindConfigFloat("PistolBaseDuration",
                        "Base duration for the pistol shot, in percent. (Attack Speed)"), "baseDuration", true);

                PistolFields = new List<IFieldChanger>
                {
                    PistolBaseDuration,
                    PistolDamageCoefficient
                };

                PistolHitLowerBarrageCooldownPercent = BindConfigFloat("PistolHitLowerBarrageCooldownPercent",
                    "The amount in percent that the current cooldown of the Barrage Skill should be lowered by. Needs to have PistolHitLowerBarrageCooldown set.");


                PistolHitLowerBarrageCooldown =
                    BindConfigBool("PistolHitLowerBarrageCooldown",
                        "If the pistol hit should lower the Barrage Skill cooldown. Needs to have PistolHitLowerBarrageCooldownPercent set to work");

                // Laser

                LaserDamageCoefficient = new FieldConfigWrapper<float>(BindConfigFloat("LaserDamageCoefficient",
                    "Damage coefficient for the secondary laser, in percent."), "damageCoefficient");

                LaserFields = new List<IFieldChanger>
                {
                    LaserDamageCoefficient
                };
                
                // Barrage

                BarrageBaseShotAmount =
                    new FieldConfigWrapper<int>(
                        BindConfigInt("BarrageBaseShotAmount", "How many shots the Barrage skill should when ATKSP = 1"),
                        "baseBulletCount", true);


                BarrageBaseDurationBetweenShots =
                    new FieldConfigWrapper<float>(BindConfigFloat("BarrageBaseDurationBetweenShots",
                        "Base duration between shots in the Barrage skill."), "baseDurationBetweenShots", true);

                BarrageFields = new List<IFieldChanger> {BarrageBaseShotAmount, BarrageBaseDurationBetweenShots};
            }

            public override void OverrideGameValues()
            {
                On.RoR2.RoR2Application.Start += (orig, self) =>
                {
                    orig(self);
                    Assembly assembly = self.GetType().Assembly;
                
                    Type firePistol = assembly.GetClass("EntityStates.Commando.CommandoWeapon", "FirePistol2");
                
                    PistolFields.ForEach(changer => changer.Apply(firePistol));
                    
                    Type fireBarr = assembly.GetClass("EntityStates.Commando.CommandoWeapon", "FireBarrage");
                
                    BarrageFields.ForEach(changer => changer.Apply(fireBarr));
                };
            }

            public override void WriteNewHooks()
            {
                On.EntityStates.Commando.CommandoWeapon.FireFMJ.OnEnter += (orig, self) =>
                {
                    LaserFields.ForEach(changer => changer.Apply(self));
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
                
                
                if (PistolHitLowerBarrageCooldown.Value && PistolHitLowerBarrageCooldownPercent.IsNotDefault())
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
                                        PistolHitLowerBarrageCooldownPercent.Value;
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