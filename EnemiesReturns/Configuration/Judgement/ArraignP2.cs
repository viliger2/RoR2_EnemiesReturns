using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration.Judgement
{
    public class ArraignP2 : IConfiguration
    {
        public static ConfigEntry<float> P2BaseMaxHealth;
        public static ConfigEntry<float> P2BaseMoveSpeed;
        public static ConfigEntry<float> P2BaseDamage;
        public static ConfigEntry<float> P2BaseArmor;
        public static ConfigEntry<float> P2LevelMaxHealth;
        public static ConfigEntry<float> P2LevelDamage;
        public static ConfigEntry<float> P2LevelArmor;
        public static ConfigEntry<float> P2SprintMultiplier;
        public static ConfigEntry<int> P2HealthSegments;

        public static ConfigEntry<float> ThreeHitComboCooldown;
        public static ConfigEntry<float> ThreeHitComboFirstSwingDamage;
        public static ConfigEntry<float> ThreeHitComboFirstSwingForce;
        public static ConfigEntry<float> ThreeHitComboFirstSwingProcCoefficient;
        public static ConfigEntry<float> ThreeHitComboSecondSwingDamage;
        public static ConfigEntry<float> ThreeHitComboSecondSwingForce;
        public static ConfigEntry<float> ThreeHitComboSecondSwingProcCoefficient;
        public static ConfigEntry<float> ThreeHitComboThirdSwingDamage;
        public static ConfigEntry<float> ThreeHitComboThirdSwingForce;
        public static ConfigEntry<float> ThreeHitComboThirdSwingProcCoefficient;
        public static ConfigEntry<float> ThreeHitComboExplosionRadius;
        public static ConfigEntry<float> ThreeHitComboExplosionDamage;
        public static ConfigEntry<float> ThreeHitComboExplosionProcCoefficient;
        public static ConfigEntry<float> ThreeHitComboExplosionForce;
        public static ConfigEntry<float> ThreeHitComboWavesDamage;
        public static ConfigEntry<int> ThreeHitComboWavesCount;
        public static ConfigEntry<float> ThreeHitComboWavesForce;

        public static ConfigEntry<float> LeftRightSwingCooldown;
        public static ConfigEntry<float> LeftRightSwingDamage;
        public static ConfigEntry<float> LeftRightSwingForce;
        public static ConfigEntry<float> LeftRightSwingProcCoefficient;

        public static ConfigEntry<float> DashLeapCooldown;
        public static ConfigEntry<float> DashLeapExplosionDamage;
        public static ConfigEntry<float> DashLeapExplosionForce;
        public static ConfigEntry<float> DashLeapProcCoefficient;
        public static ConfigEntry<float> DashLeapExplosionRadius;
        public static ConfigEntry<float> DashLeapProjectileDamage;
        public static ConfigEntry<float> DashLeapProjectileForce;
        public static ConfigEntry<float> DashLeapProjectileDistanceBetween;

        public static ConfigEntry<float> SpearThrowCooldown;
        public static ConfigEntry<float> SpearThrowDamage;
        public static ConfigEntry<float> SpearThrowForce;

        public static ConfigEntry<float> ClockAttackCooldown;
        public static ConfigEntry<float> ClockAttackDelayBetweenSpawns;
        public static ConfigEntry<float> ClockAttackProjectileDistance;
        public static ConfigEntry<float> ClockAttackProjectileDamage;
        public static ConfigEntry<int> ClockAttackAdditionalPairs;
        public static ConfigEntry<int> ClockAttackDistanceFromStart;

        public static ConfigEntry<float> SkyLaserCooldown;
        public static ConfigEntry<int> SkyLaserCount;
        public static ConfigEntry<float> SkyLaserAdditionalCountPerPlayer;
        public static ConfigEntry<float> SkyLaserLifetime;
        public static ConfigEntry<float> SkyLaserDamageCoefficient;
        public static ConfigEntry<float> SkyLaserMovementSpeed;

        public static ConfigEntry<bool> ThisEntryDoesNothing;

        public void PopulateConfig(ConfigFile config)
        {
            P2BaseMaxHealth = config.Bind("Arraign P2 Character Stats", "Base Max Health", 4000f, "Arraign P2' base health.");
            P2BaseMoveSpeed = config.Bind("Arraign P2 Character Stats", "Base Movement Speed", 15f, "Arraign P2' base movement speed.");
            P2BaseDamage = config.Bind("Arraign P2 Character Stats", "Base Damage", 16f, "Arraign P2' base damage.");
            P2BaseArmor = config.Bind("Arraign P2 Character Stats", "Base Armor", 20f, "Arraign P2' base armor.");

            P2LevelMaxHealth = config.Bind("Arraign P2 Character Stats", "Health per Level", 1200f, "Arraign P2' health increase per level.");
            P2LevelDamage = config.Bind("Arraign P2 Character Stats", "Damage per Level", 3.2f, "Arraign P2' damage increase per level.");
            P2LevelArmor = config.Bind("Arraign P2 Character Stats", "Armor per Level", 0f, "Arraign P2' armor increase per level.");

            P2SprintMultiplier = config.Bind("Arraign P2 Character Stats", "Sprint Multiplier", 3.2f, "Arraign P2' health increase per level.");

            P2HealthSegments = config.Bind("Arraign P2 Character Stats", "Number of Health Segments", 3, "Arraign P2' number of health segments that you need to break with hammer.");

            ThisEntryDoesNothing = config.Bind("Phases Share Beam and Lightning Attacks", "Please Use ArraignP1 Config For Those Attacks", false, "Maybe someday I'll separate them, but probably not.");

            ThreeHitComboCooldown = config.Bind("ThreeHitCombo", "ThreeHitCombo Cooldown", 12f, "ThreeHitCombo cooldown.");
            ThreeHitComboFirstSwingDamage = config.Bind("ThreeHitCombo", "ThreeHitCombo First Swing Damage", 3.5f, "ThreeHitCombo first swing damage.");
            ThreeHitComboFirstSwingForce = config.Bind("ThreeHitCombo", "ThreeHitCombo First Swing Force", 600f, "ThreeHitCombo first swing force.");
            ThreeHitComboFirstSwingProcCoefficient = config.Bind("ThreeHitCombo", "ThreeHitCombo First Swing Proc Coefficient", 1f, "ThreeHitCombo first swing proc coefficient.");

            ThreeHitComboSecondSwingDamage = config.Bind("ThreeHitCombo", "ThreeHitCombo Second Swing Damage", 3.5f, "ThreeHitCombo second swing damage.");
            ThreeHitComboSecondSwingForce = config.Bind("ThreeHitCombo", "ThreeHitCombo Second Swing Force", 600f, "ThreeHitCombo second swing force.");
            ThreeHitComboSecondSwingProcCoefficient = config.Bind("ThreeHitCombo", "ThreeHitCombo Second Swing Proc Coefficient", 1f, "ThreeHitCombo second swing proc coefficient.");

            ThreeHitComboThirdSwingDamage = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Damage", 3.5f, "ThreeHitCombo third swing damage.");
            ThreeHitComboThirdSwingForce = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Force", 600f, "ThreeHitCombo third swing force.");
            ThreeHitComboThirdSwingProcCoefficient = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Proc Coefficient", 1f, "ThreeHitCombo third swing proc coefficient.");

            ThreeHitComboExplosionRadius = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Explosion Radius", 20f, "ThreeHitCombo third swing explosion radius.");
            ThreeHitComboExplosionDamage = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Explosion Damage", 5f, "ThreeHitCombo third swing explosion damage.");
            ThreeHitComboExplosionProcCoefficient = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Explosion Proc Coefficient", 1f, "ThreeHitCombo third swing explosion proc coefficient.");
            ThreeHitComboExplosionForce = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Force", 500f, "ThreeHitCombo third swing proc explosion force.");

            ThreeHitComboWavesDamage = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Waves Damage", 4f, "ThreeHitCombo third swing explosion waves damage.");
            ThreeHitComboWavesCount = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Waves Count", 8, "ThreeHitCombo third swing explosion waves count.");
            ThreeHitComboWavesForce = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Waves Force", 0f, "ThreeHitCombo third swing explosion waves force.");

            LeftRightSwingCooldown = config.Bind("LeftRightSwing", "LeftRighSwingCooldown Cooldown", 0f, "LeftRightSwing cooldown.");
            LeftRightSwingDamage = config.Bind("LeftRightSwing", "LeftRighSwingCooldown Damage", 2f, "LeftRightSwing damage.");
            LeftRightSwingForce = config.Bind("LeftRightSwing", "LeftRighSwingCooldown Force", 600f, "LeftRightSwing force.");
            LeftRightSwingProcCoefficient = config.Bind("LeftRightSwing", "LeftRighSwingCooldown Proc Coefficient", 1f, "LeftRightSwing proc coefficient.");

            DashLeapCooldown = config.Bind("DashLeap", "DashLeap Cooldown", 10f, "DashLeap cooldown.");
            DashLeapExplosionDamage = config.Bind("DashLeap", "DashLeap Explosion Damage", 6f, "DashLeap explosion damage.");
            DashLeapExplosionForce = config.Bind("DashLeap", "DashLeap Explosion Force", 0f, "DashLeap explosion force.");
            DashLeapProcCoefficient = config.Bind("DashLeap", "DashLeap Proc Coefficient", 1f, "DashLeap explosion proc coefficient.");
            DashLeapExplosionRadius = config.Bind("DashLeap", "DashLeap Explosion Radius", 20f, "DashLeap explosion radius.");
            DashLeapCooldown = config.Bind("DashLeap", "DashLeap Cooldown", 10f, "DashLeap cooldown.");

            DashLeapProjectileDamage = config.Bind("DashLeap", "DashLeap Projectile Damage", 4f, "DashLeap projectile damage.");
            DashLeapProjectileForce = config.Bind("DashLeap", "DashLeap Projectile Force", 0f, "DashLeap projectile force.");
            DashLeapProjectileDistanceBetween = config.Bind("DashLeap", "DashLeap Distance Between Projectiles", 13f, "DashLeap distance between projectiles spawned during leap. By default equal to the radius of that projectile .");

            SpearThrowCooldown = config.Bind("SpearThrow", "SpearThrow Cooldown", 10f, "SpearThrow cooldown.");
            SpearThrowDamage = config.Bind("SpearThrow", "SpearThrow Damage", 7f, "SpearThrow damage.");
            SpearThrowForce = config.Bind("SpearThrow", "SpearThrow Force", 0f, "SpearThrow force.");

            ClockAttackCooldown = config.Bind("ClockAttack", "ClockAttack Cooldown", 20f, "ClockAttack cooldown.");
            ClockAttackDelayBetweenSpawns = config.Bind("ClockAttack", "ClockAttack Delay Between Spawns", 0.1f, "ClockAttack delay between projectiles spawns in a single line.");
            ClockAttackProjectileDistance = config.Bind("ClockAttack", "ClockAttack Projectile Spawn Distance", 10f, "ClockAttack distance between projectile spawns in a single line.");
            ClockAttackProjectileDamage = config.Bind("ClockAttack", "ClockAttack Projectile Damage", 4f, "ClockAttack projectile damage.");
            ClockAttackAdditionalPairs = config.Bind("ClockAttack", "ClockAttack Additional Line Pairs", 1, "ClockAttack additional line pairs.");
            ClockAttackDistanceFromStart = config.Bind("ClockAttack", "ClockAttack Distance From Origin", 2, "ClockAttack additional lines distance from original. Imagine a clock and then add or subtract this number of hours, this will be origin point of additional lines.");

            SkyLaserCooldown = config.Bind("SkyLaser", "SkyLaser Cooldown", 40f, "SkyLaser cooldown.");
            SkyLaserLifetime = config.Bind("SkyLaser", "SkyLaser Lifetime", 20f, "SkyLaser lifetime.");
            SkyLaserCount = config.Bind("SkyLaser", "SkyLaser Count", 3, "SkyLaser summon count.");
            SkyLaserAdditionalCountPerPlayer = config.Bind("SkyLaser", "SkyLaser Additional Summon Per Player", 0.5f, "SkyLaser additional summon per player, so with default config you will get 1 additional laser per 2 additional players, so 3 players will get 4 lasers instead of 3.");
            SkyLaserDamageCoefficient = config.Bind("SkyLaser", "SkyLaser Damage", 10f, "SkyLaser damage, does not scale off ArraignP2 body stats and instead scales of its own stat values which are 16(+3.2).");
            SkyLaserMovementSpeed = config.Bind("SkyLaser", "SkyLaser Movement Speed", 12f, "SkyLaser summoned laser movement speed.");
        }
    }
}
