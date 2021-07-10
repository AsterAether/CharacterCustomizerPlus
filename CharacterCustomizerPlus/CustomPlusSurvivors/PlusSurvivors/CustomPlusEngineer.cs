using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;
using BepInEx.Logging;
using CharacterCustomizer.Util.Config;
using CharacterCustomizer.Util.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;
using RoR2;

namespace CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors
{
    public class CustomPlusEngineer : CustomPlusSurvivor
    {
        public CustomPlusEngineer(ConfigFile file, ManualLogSource logger) : base("Engi", file, logger)
        {
        }

        public ConfigEntryDescriptionWrapper<int> TurretMaxDeployCount;

        public ConfigEntryDescriptionWrapper<int> ShieldMaxDeployCount;

        public FieldConfigWrapper<float> ShieldDuration;

        public ConfigEntryDescriptionWrapper<bool> ShieldEndlessDuration;

        public List<IFieldChanger> ShieldDeployedFields;

        public FieldConfigWrapper<int> GrenadeMaxFireAmount;
        public FieldConfigWrapper<int> GrenadeMinFireAmount;
        public ConfigEntryDescriptionWrapper<bool> GrenadeSetChargeCountToFireAmount;
        public FieldConfigWrapper<float> GrenadeMaxChargeTime;
        public FieldConfigWrapper<float> GrenadeTotalChargeDuration;

        public List<IFieldChanger> ChargeGrenadesFields;


        protected override void InitConfigValues()
        {
            TurretMaxDeployCount = BindConfigInt("TurretMaxDeployCount",
                "The maximum number of turrets the Engineer can place.");

            ShieldMaxDeployCount = BindConfigInt("ShieldMaxDeployCount",
                "The maximum number of shields the Engineer can place.");

            ShieldDuration = new FieldConfigWrapper<float>(
                BindConfigFloat("ShieldDuration", "The number of seconds the shield is active."), "lifetime", true);

            ShieldDeployedFields = new List<IFieldChanger> {ShieldDuration};

            ShieldEndlessDuration = BindConfigBool("ShieldEndlessDuration",
                "If the duration of the shield should be endless.");


            GrenadeMaxFireAmount = new FieldConfigWrapper<int>(BindConfigInt("GrenadeMaxFireAmount",
                "The maximum number of grenades the Engineer can fire."), "maxGrenadeCount", true);


            GrenadeMinFireAmount = new FieldConfigWrapper<int>(BindConfigInt("GrenadeMinFireAmount",
                "The minimum number of grenades the Engineer fires."), "minGrenadeCount", true);


            GrenadeSetChargeCountToFireAmount = BindConfigBool("GrenadeSetChargeCountToFireAmount",
                "Set the number of \"clicks\" you hear in the charging animation to the maximum grenade count.");


            GrenadeMaxChargeTime =
                new FieldConfigWrapper<float>(BindConfigFloat("GrenadeMaxChargeTime",
                    "Maximum charge time (animation) for grenades, in seconds."), "baseMaxChargeTime", true);


            GrenadeTotalChargeDuration =
                new FieldConfigWrapper<float>(BindConfigFloat("GrenadeTotalChargeDuration",
                    "Maximum charge duration (logic) for grenades, in seconds."), "baseTotalDuration", true);

            ChargeGrenadesFields = new List<IFieldChanger>
            {
                GrenadeMaxChargeTime, GrenadeMaxFireAmount, GrenadeMinFireAmount, GrenadeTotalChargeDuration
            };
        }

        protected override void OverrideGameValues()
        {
            TurretMaxDeployCount.UpdateDescription(2);
            ShieldMaxDeployCount.UpdateDescription(1);

            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += (orig, self, slot) =>
            {
                if (TurretMaxDeployCount.IsNotDefault() && slot == DeployableSlot.EngiTurret)
                    return TurretMaxDeployCount.Value;
                if (ShieldMaxDeployCount.IsNotDefault() && slot == DeployableSlot.EngiBubbleShield)
                    return ShieldMaxDeployCount.Value;
                return orig(self, slot);
            };


            // Workaround for more than 8 max grenades
            if (GrenadeMinFireAmount.ConfigEntryDescriptionWrapper.Value >= 8 ||
                GrenadeMaxFireAmount.ConfigEntryDescriptionWrapper.Value >= 8)
            {
                On.EntityStates.Engi.EngiWeapon.FireGrenades.OnEnter += (orig, self) =>
                {
                    var fireGrenades =
                        self.GetType().Assembly.GetClass("EntityStates.Engi.EngiWeapon", "FireGrenades");

                    orig(self);
                    self.SetFieldValue("duration",
                        fireGrenades.GetFieldValue<float>("baseDuration")
                        * self.GetFieldValue<int>("grenadeCountMax") / 8f
                                                                     / self.GetFieldValue<float>("attackSpeedStat")
                    );
                };
            }


            var shieldDeployed = typeof(EntityStates.Engi.EngiBubbleShield.Deployed);

            ShieldDeployedFields.ForEach(changer => changer.Apply(shieldDeployed));
            if (ShieldEndlessDuration.Value)
            {
                EntityStates.Engi.EngiBubbleShield.Deployed.lifetime = float.PositiveInfinity;
            }

            var chargeGrenades = SurvivorDef.GetType().Assembly
                .GetClass("EntityStates.Engi.EngiWeapon", "ChargeGrenades");

            ChargeGrenadesFields.ForEach(changer => changer.Apply(chargeGrenades));

            if (GrenadeSetChargeCountToFireAmount.Value &&
                GrenadeMaxFireAmount.ConfigEntryDescriptionWrapper.IsNotDefault())
            {
                chargeGrenades.SetFieldValue("maxCharges",
                    GrenadeMaxFireAmount.ConfigEntryDescriptionWrapper.Value);
            }
        }

        protected override void WriteNewHooks()
        {
        }
    }
}