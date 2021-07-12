using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Configuration;
using BepInEx.Logging;
using CharacterCustomizer.CustomSurvivors;
using CharacterCustomizer.Util.Config;
using CharacterCustomizer.Util.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;
using RoR2;
using RoR2.ContentManagement;

namespace CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors
{
    public class CustomPlusEngineer : CustomPlusSurvivor
    {
        // Turrets

        private ConfigEntryDescriptionWrapper<int> TurretMaxDeployCount { get; set; }
        private ConfigEntryDescriptionWrapper<int> GaussTurretBulletCount { get; set; }

        private CustomBodyDefinition TurretBody { get; set; }

        // Shield

        private ConfigEntryDescriptionWrapper<int> ShieldMaxDeployCount { get; set; }

        private ConfigEntryDescriptionWrapper<bool> ShieldEndlessDuration { get; set; }

        private readonly FieldChangerBag _shieldDeployedFields;

        // Grenade

        private ConfigEntryDescriptionWrapper<bool> GrenadeSetChargeCountToFireAmount { get; set; }

        private readonly FieldChangerBag _chargeGrenadesFields;

        public CustomPlusEngineer(ConfigFile file, ManualLogSource logger) : base("Engi", file, logger)
        {
            _chargeGrenadesFields = new FieldChangerBag(this);
            _shieldDeployedFields = new FieldChangerBag(this);
        }

        protected override void InitConfigValues()
        {
            TurretBody = new CustomBodyDefinition(this, "GaussTurret");

            GaussTurretBulletCount = BindConfig<int>("GaussTurretBulletCount",
                "How many bullets the GaussTurret fires.");

            TurretMaxDeployCount = BindConfig<int>("TurretMaxDeployCount",
                "The maximum number of turrets the Engineer can place.");

            ShieldMaxDeployCount = BindConfig<int>("ShieldMaxDeployCount",
                "The maximum number of shields the Engineer can place.");

            _shieldDeployedFields.AddFieldConfig<float>(
                "ShieldDuration", "The number of seconds the shield is active.", "lifetime", true);

            ShieldEndlessDuration = BindConfig<bool>("ShieldEndlessDuration",
                "If the duration of the shield should be endless.");


            _chargeGrenadesFields.AddFieldConfig<int>("GrenadeMaxFireAmount",
                "The maximum number of grenades the Engineer can fire.", "maxGrenadeCount", true);

            _chargeGrenadesFields.AddFieldConfig<int>("GrenadeMinFireAmount",
                "The minimum number of grenades the Engineer fires.", "minGrenadeCount", true);


            _chargeGrenadesFields.AddFieldConfig<float>("GrenadeMaxChargeTime",
                "Maximum charge time (animation) for grenades, in seconds.", "baseMaxChargeTime", true);

            _chargeGrenadesFields.AddFieldConfig<float>("GrenadeTotalChargeDuration",
                "Maximum charge duration (logic) for grenades, in seconds.", "baseTotalDuration", true);

            GrenadeSetChargeCountToFireAmount = BindConfig<bool>("GrenadeSetChargeCountToFireAmount",
                "Set the number of \"clicks\" you hear in the charging animation to the maximum grenade count.");
        }

        protected override void OverrideGameValues()
        {
            var gaussTurretBody = BodyCatalog.FindBodyPrefab("EngiTurretBody")?.GetComponent<CharacterBody>();
            TurretBody.Apply(gaussTurretBody);

            IL.EntityStates.EngiTurret.EngiTurretWeapon.FireGauss.OnEnter += il =>
            {
                var c = new ILCursor(il);
                c.GotoNext(i => i.Match(OpCodes.Ldc_I4_1));
                c.Remove();
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<BulletAttack, uint>>(attack =>
                {
                    if (GaussTurretBulletCount.IsNotDefault())
                    {
                        return (uint) GaussTurretBulletCount.Value;
                    }

                    return 1u;
                });
            };
            
            GaussTurretBulletCount.UpdateDescription(1);
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
            if (_chargeGrenadesFields.GetValueByFieldName<int>("minGrenadeCount") >= 8 ||
                _chargeGrenadesFields.GetValueByFieldName<int>("maxGrenadeCount") >= 8)
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

            _shieldDeployedFields.Apply(shieldDeployed);
            
            if (ShieldEndlessDuration.Value)
            {
                EntityStates.Engi.EngiBubbleShield.Deployed.lifetime = float.PositiveInfinity;
            }

            var chargeGrenades = SurvivorDef.GetType().Assembly
                .GetClass("EntityStates.Engi.EngiWeapon", "ChargeGrenades");

            _chargeGrenadesFields.Apply(chargeGrenades);

            if (GrenadeSetChargeCountToFireAmount.Value &&
                _chargeGrenadesFields.GetWrapperByFieldName<int>("maxGrenadeCount").IsNotDefault())
            {
                chargeGrenades.SetFieldValue("maxCharges",
                    _chargeGrenadesFields.GetValueByFieldName<int>("maxGrenadeCount"));
            }
        }

        protected override void WriteNewHooks()
        {
            
        }
    }
}