# Config Values
## General
* **PrintReadme:** Outputs a file called "config_values.md" to the working directory, containing all config values formatted as Markdown. (Only used for development purposes)
# Artificer
* **FlameboltAttackSpeedCooldownScaling:** If the cooldown of the Flamebolt Skill should scale with AttackSpeed. Needs to have FlameboltAttackSpeedCooldownScalingCoefficent set to work.
* **FlameboltAttackSpeedCooldownScalingCoefficient:** Coefficient for cooldown AttackSpeed scaling, in percent. Formula: BaseCooldown * (1 / (1 + (ATKSP-1) * Coeff)) .
* **FlameboltAttackSpeedStockScaling:** If the charge count of the FlameBolt Skill should scale with AttackSpeed. Needs to have FlameboltAttackSpeedStockScalingCoefficent set to work.
* **FlameboltAttackSpeedStockScalingCoefficient:** Coefficient for charge AttackSpeed scaling, in percent. Formula: Stock + Stock * (ATKSP - 1) * Coeff.
* **FlamethrowerDuration:** The duration of the flamethrower
* **FlamethrowerDurationScaleCoefficient:** The coefficient for flame thrower scaling. Formula: Duration - Coeff * (ATKSP - 1) * Duration. Minimum of FlamethrowerMinimalDuration seconds.
* **FlamethrowerDurationScaleDownWithAttackSpeed:** If the flame thrower duration should get shorter with more attack speed. Needs FlamethrowerDurationScaleCoefficient to be set.
* **FlamethrowerIgnitePercentChance:** The change to ignite per proc in percent.
* **FlamethrowerMaxDistance:** The max distance of the Flamethrower
* **FlamethrowerMinimalDuration:** The minimal duration of the flamethrower
* **FlamethrowerProcCoefficientPerTick:** The coefficient for items per proc of the flamethrower.
* **FlamethrowerRadius:** The radius of the Flamethrower
* **FlamethrowerTickFrequency:** The tick frequency of the flamethrower
* **FlamethrowerTickFrequencyScaleCoefficient:** The coefficient for the AttackSpeed scaling of the Flamethrower. Formula: TickFreq + Coeff * (ATKSP - 1) * TickFreq
* **FlamethrowerTickFrequencyScaleWithAttackSpeed:** If the tick frequency should scale with AttackSpeed. Needs FlamethrowerTickFrequencyScaleCoefficient to be set to work.
* **FlamethrowerTotalDamageCoefficient:** The total damage coefficient for the flamethrower
* **NanoBombBaseChargeDuration:** Base max charging duration of the NanoBomb
* **NanoBombMaxDamageCoefficient:** Max damage coefficient of the NanoBomb
# Commando
* **BarrageBaseDurationBetweenShots:** Base duration between shots in the Barrage skill.
* **BarrageBaseShotAmount:** How many shots the Barrage skill should when ATKSP = 1
* **DashInvulnerability:** If Commando should be invulnerable while dashing.
* **DashInvulnerabilityTimer:** How long Commando should be invincible for when dashing. Only active when DashInvulnerability is on. 0 = For the whole dash.
* **DashResetsSecondCooldown:** If the dash should reset the cooldown of the second ability.
* **LaserDamageCoefficient:** Damage coefficient for the secondary laser, in percent.
* **PistolBaseDuration:** Base duration for the pistol shot, in percent. (Attack Speed)
* **PistolDamageCoefficient:** Damage coefficient for the pistol, in percent.
* **PistolHitLowerBarrageCooldown:** If the pistol hit should lower the Barrage Skill cooldown. Needs to have PistolHitLowerBarrageCooldownPercent set to work
* **PistolHitLowerBarrageCooldownPercent:** The amount in percent that the current cooldown of the Barrage Skill should be lowered by. Needs to have PistolHitLowerBarrageCooldownPercent set.
# Acrid
# Engineer
* **GrenadeMaxChargeTime:** Maximum charge time (animation) for grenades, in seconds.
* **GrenadeMaxFireAmount:** The maximum number of grenades the Engineer can fire.
* **GrenadeMinFireAmount:** The minimum number of grenades the Engineer fires.
* **GrenadeSetChargeCountToFireAmount:** Set the number of "clicks" you hear in the charging animation to the maximum grenade count.
* **GrenadeTotalChargeDuration:** Maximum charge duration (logic) for grenades, in seconds.
* **MineMaxDeployCount:** The maximum number of mines the Engineer can place. Vanilla value: 10
* **ShieldDuration:** The number of seconds the shield is active.
* **ShieldEndlessDuration:** If the duration of the shield should be endless.
* **ShieldMaxDeployCount:** The maximum number of shields the Engineer can place. Vanilla value: 1
* **TurretMaxDeployCount:** The maximum number of turrets the Engineer can place. Vanilla value: 2
# Huntress
* **TrackingMaxAngle:** The maximum angle the tracking of the huntress works.
* **TrackingMaxDistance:** The maximum distance the tracking of the huntress works.
# Loader
# Mercenary
* **DashMaxCount:** Maximum amount of dashes Mercenary can perform.
* **DashTimeoutDuration:** Maximum timeout between dashes, in seconds
# MultT
* **NailgunSpreadPitch:** Pitch spread of the nailgun, in percent
* **NailgunSpreadYaw:** Yaw spread of the nailgun, in percent
# REX
