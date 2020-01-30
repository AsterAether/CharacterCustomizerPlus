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
        public ConfigEntryDescriptionWrapper<bool> FlameboltAttackSpeedStockScaling;

        public ConfigEntryDescriptionWrapper<float> FlameboltAttackSpeedStockScalingCoefficient;

        public ConfigEntryDescriptionWrapper<bool> FlameboltAttackSpeedCooldownScaling;

        public ConfigEntryDescriptionWrapper<float> FlameboltAttackSpeedCooldownScalingCoefficient;

        public List<IFieldChanger> FlameboltFields;

        public FieldConfigWrapper<float> NanoBombMaxChargeDuration;

        public FieldConfigWrapper<float> NanoBombMaxDamageCoefficient;

        public List<IFieldChanger> ChargeNanoBombFields;

        public FieldConfigWrapper<float> FlamethrowerDuration;

        public FieldConfigWrapper<float> FlamethrowerTickFrequency;

        public ConfigEntryDescriptionWrapper<bool> FlamethrowerTickFrequencyScaleWithAttackSpeed;

        public float VanillaFlamethrowerTickFrequency;

        public ConfigEntryDescriptionWrapper<float> FlamethrowerTickFrequencyScaleCoefficient;

        public FieldConfigWrapper<float> FlamethrowerProcCoefficientPerTick;

        public FieldConfigWrapper<float> FlamethrowerMaxDistance;

        public FieldConfigWrapper<float> FlamethrowerRadius;

        public FieldConfigWrapper<float> FlamethrowerTotalDamageCoefficient;

        public FieldConfigWrapper<float> FlamethrowerIgnitePercentChance;

        public ConfigEntryDescriptionWrapper<bool> FlamethrowerDurationScaleDownWithAttackSpeed;

        public ConfigEntryDescriptionWrapper<float> FlamethrowerDurationScaleCoefficient;

        public ConfigEntryDescriptionWrapper<float> FlamethrowerMinimalDuration;

        public float VanillaFlamethrowerDuration;

        public List<IFieldChanger> FlamethrowerFields;


        public CustomPlusArtificer(ConfigFile file) : base(SurvivorIndex.Mage, "Artificer", file)
        {
        }

        public override void InitConfigValues()
        {
            // Flamebolt
            FlameboltAttackSpeedStockScaling =
                BindConfigBool("FlameboltAttackSpeedStockScaling",
                    "If the charge count of the FlameBolt Skill should scale with AttackSpeed. Needs to have FlameboltAttackSpeedStockScalingCoefficent set to work.");


            FlameboltAttackSpeedStockScalingCoefficient =
                BindConfigFloat("FlameboltAttackSpeedStockScalingCoefficient",
                    "Coefficient for charge AttackSpeed scaling, in percent. Formula: Stock + Stock * (ATKSP - 1) * Coeff.");

            FlameboltAttackSpeedCooldownScaling = BindConfigBool("FlameboltAttackSpeedCooldownScaling",
                "If the cooldown of the Flamebolt Skill should scale with AttackSpeed. Needs to have FlameboltAttackSpeedCooldownScalingCoefficent set to work.");


            FlameboltAttackSpeedCooldownScalingCoefficient = BindConfigFloat(
                "FlameboltAttackSpeedCooldownScalingCoefficient",
                "Coefficient for cooldown AttackSpeed scaling, in percent. Formula: BaseCooldown * (1 / (1 + (ATKSP-1) * Coeff)) .");

            // NanoBomb

            NanoBombMaxChargeDuration = new FieldConfigWrapper<float>(BindConfigFloat("NanoBombBaseChargeDuration",
                "Base max charging duration of the NanoBomb"), "baseChargeDuration");

            NanoBombMaxDamageCoefficient = new FieldConfigWrapper<float>(BindConfigFloat(
                "NanoBombMaxDamageCoefficient",
                "Max damage coefficient of the NanoBomb"), "maxDamageCoefficient");

            ChargeNanoBombFields = new List<IFieldChanger>
            {
                NanoBombMaxChargeDuration,
                NanoBombMaxDamageCoefficient
            };

            // Flamethrower

            FlamethrowerProcCoefficientPerTick = new FieldConfigWrapper<float>(BindConfigFloat(
                "FlamethrowerProcCoefficientPerTick",
                "The coefficient for items per proc of the flamethrower."), "procCoefficientPerTick", true);

            FlamethrowerMaxDistance = new FieldConfigWrapper<float>(BindConfigFloat("FlamethrowerMaxDistance",
                "The max distance of the Flamethrower"), "maxDistance", true);

            FlamethrowerRadius = new FieldConfigWrapper<float>(BindConfigFloat("FlamethrowerRadius",
                "The radius of the Flamethrower"), "radius", true);

            FlamethrowerTotalDamageCoefficient = new FieldConfigWrapper<float>(BindConfigFloat(
                "FlamethrowerTotalDamageCoefficient",
                "The total damage coefficient for the flamethrower"), "totalDamageCoefficient", true);

            FlamethrowerIgnitePercentChance = new FieldConfigWrapper<float>(BindConfigFloat(
                "FlamethrowerIgnitePercentChance",
                "The change to ignite per proc in percent."), "ignitePercentChance", true);

            FlamethrowerFields = new List<IFieldChanger>
            {
                FlamethrowerProcCoefficientPerTick,
                FlamethrowerRadius,
                FlamethrowerTotalDamageCoefficient,
                FlamethrowerIgnitePercentChance
            };

            FlamethrowerDuration = new FieldConfigWrapper<float>(BindConfigFloat("FlamethrowerDuration",
                "The duration of the flamethrower"), "baseFlamethrowerDuration", true);

            FlamethrowerTickFrequency = new FieldConfigWrapper<float>(BindConfigFloat("FlamethrowerTickFrequency",
                "The tick frequency of the flamethrower"), "tickFrequency", true);

            FlamethrowerTickFrequencyScaleWithAttackSpeed = BindConfigBool(
                "FlamethrowerTickFrequencyScaleWithAttackSpeed",
                "If the tick frequency should scale with AttackSpeed. Needs FlamethrowerTickFrequencyScaleCoefficient to be set to work.");

            FlamethrowerTickFrequencyScaleCoefficient = BindConfigFloat("FlamethrowerTickFrequencyScaleCoefficient",
                "The coefficient for the AttackSpeed scaling of the Flamethrower. Formula: TickFreq + Coeff * (ATKSP - 1) * TickFreq");

            FlamethrowerDurationScaleDownWithAttackSpeed =
                BindConfigBool("FlamethrowerDurationScaleDownWithAttackSpeed",
                    "If the flame thrower duration should get shorter with more attack speed. Needs FlamethrowerDurationScaleCoefficient to be set.");

            FlamethrowerDurationScaleCoefficient = BindConfigFloat("FlamethrowerDurationScaleCoefficient",
                "The coefficient for flame thrower scaling. Formula: Duration - Coeff * (ATKSP - 1) * Duration. Minimum of FlamethrowerMinimalDuration seconds.");

            FlamethrowerMinimalDuration = BindConfigFloat("FlamethrowerMinimalDuration",
                "The minimal duration of the flamethrower", 1f);
        }


        public override void OverrideGameValues()
        {
            On.RoR2.RoR2Application.Start += (orig, self) =>
            {
                orig(self);

                Assembly assembly = self.GetType().Assembly;

                Type flamethrower = assembly.GetClass("EntityStates.Mage.Weapon", "Flamethrower");

                FlamethrowerFields.ForEach(changer => changer.Apply(flamethrower));

                VanillaFlamethrowerTickFrequency = FlamethrowerTickFrequency.GetValue<float>(flamethrower);
                VanillaFlamethrowerDuration = FlamethrowerDuration.GetValue<float>(flamethrower);
            };

            On.EntityStates.Mage.Weapon.Flamethrower.OnEnter += (orig, self) =>
            {
                FlamethrowerMaxDistance.Apply(self);
                orig(self);
            };

            On.EntityStates.Mage.Weapon.ChargeNovabomb.OnEnter += (orig, self) =>
            {
                ChargeNanoBombFields.ForEach(changer => changer.Apply(self));
                orig(self);
            };
        }

        public override void WriteNewHooks()
        {
            bool runStockScaling = FlameboltAttackSpeedStockScaling.Value &&
                                   FlameboltAttackSpeedStockScalingCoefficient.IsNotDefault();

            bool runCooldownScaling = FlameboltAttackSpeedCooldownScaling.Value &&
                                      FlameboltAttackSpeedCooldownScalingCoefficient.IsNotDefault();

            float cooldownCoeff = FlameboltAttackSpeedCooldownScalingCoefficient.Value;
            float stockCoeff = FlameboltAttackSpeedStockScalingCoefficient.Value;

            if (runStockScaling || runCooldownScaling)
            {
                IL.RoR2.CharacterBody.RecalculateStats += il =>
                {
                    ILCursor c = new ILCursor(il);
                    c.GotoNext(x =>
                        x.MatchRet()
                    );

                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit<CharacterBody>(OpCodes.Ldfld, "skillLocator");
                    c.Emit(OpCodes.Ldloc_S, (byte) 45);
                    c.Emit(OpCodes.Ldloc_S, (byte) 42);
                    c.EmitDelegate<Action<SkillLocator, float, float>>(
                        (skillLocator, cooldownScale, attackSpeed) =>
                        {
                            GenericSkill primary = skillLocator != null ? skillLocator.primary : null;
                            if (primary != null && primary.skillName == "FireFlamebolt")
                            {
                                if (runCooldownScaling)
                                {
                                    float val = cooldownScale *
                                                (1 / (1 + cooldownCoeff * (attackSpeed - 1)));
                                    primary.cooldownScale = val;
                                }

                                if (runStockScaling)
                                {
                                    primary.SetBonusStockFromBody(
                                        (int) ((attackSpeed - 1) * stockCoeff * primary.skillDef.baseMaxStock));
                                }
                            }
                        });
                };
            }


            On.EntityStates.Mage.Weapon.Flamethrower.OnEnter += (orig, self) =>
            {
                Type flamethrowerType = self.GetType();

                GameObject go = typeof(EntityState)
                    .GetProperty("gameObject", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.GetValue(self) as GameObject;

                CharacterBody body = go.GetComponent<CharacterBody>();

                if (FlamethrowerTickFrequencyScaleWithAttackSpeed.Value &&
                    FlamethrowerTickFrequencyScaleCoefficient.IsNotDefault())
                {
                    float baseVal = FlamethrowerTickFrequency.ConfigEntryDescriptionWrapper.IsNotDefault()
                        ? FlamethrowerTickFrequency.ConfigEntryDescriptionWrapper.Value
                        : VanillaFlamethrowerTickFrequency;

                    float val = baseVal + (body.attackSpeed - 1) *
                                FlamethrowerTickFrequencyScaleCoefficient.Value * baseVal
                        ;

                    flamethrowerType.SetFieldValue("tickFrequency",
                        val
                    );
                }

                if (FlamethrowerDurationScaleDownWithAttackSpeed.Value &&
                    FlamethrowerDurationScaleCoefficient.IsNotDefault())
                {
                    float baseVal = FlamethrowerDuration.ConfigEntryDescriptionWrapper.IsNotDefault()
                        ? FlamethrowerDuration.ConfigEntryDescriptionWrapper.Value
                        : VanillaFlamethrowerDuration;

                    float val = baseVal - (body.attackSpeed - 1) *
                                FlamethrowerDurationScaleCoefficient.Value *
                                baseVal;

                    if (val < FlamethrowerMinimalDuration.Value)
                    {
                        val = FlamethrowerMinimalDuration.Value;
                    }

                    flamethrowerType.SetFieldValue("baseFlamethrowerDuration",
                        val
                    );
                }


                orig(self);
            };
        }
    }
}