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
        private ConfigEntryDescriptionWrapper<bool> DoubleTapHitLowerSpecialCooldown { get; set; }

        private ConfigEntryDescriptionWrapper<float> DoubleTapHitLowerSpecialCooldownPercent { get; set; }

        private readonly FieldChangerBag _doubleTapFields;


        public CustomPlusCommando(ConfigFile file, ManualLogSource logger) : base("Commando", file, logger)
        {
            _doubleTapFields = new FieldChangerBag(this);
        }


        protected override void InitConfigValues()
        {
            // Pistol

            _doubleTapFields.AddFieldConfig<float>("DoubleTapDamageCoefficient",
                "Damage coefficient for the Double Tap, in percent.",
                "damageCoefficient",
                true);

            _doubleTapFields.AddFieldConfig<float>("DoubleTapBaseDuration",
                "Base duration for the Double Tap shot, in percent. (Attack Speed)",
                "baseDuration",
                true);

            DoubleTapHitLowerSpecialCooldownPercent = BindConfig<float>("DoubleTapHitLowerSpecialCooldownPercent",
                "The amount in percent that the current cooldown of the Barrage Skill should be lowered by. Needs to have DoubleTapHitLowerSpecialCooldown set.");


            DoubleTapHitLowerSpecialCooldown =
                BindConfig<bool>("DoubleTapHitLowerSpecialCooldown",
                    "If the pistol hit should lower the Special Skill cooldown. Needs to have DoubleTapHitLowerSpecialCooldownPercent set to work");
        }

        protected override void OverrideGameValues()
        {
            var assembly = SurvivorDef.GetType().Assembly;

            var firePistol = assembly.GetClass("EntityStates.Commando.CommandoWeapon", "FirePistol2");
            _doubleTapFields.Apply(firePistol);
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