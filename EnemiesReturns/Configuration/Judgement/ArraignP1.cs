using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration.Judgement
{
    public class ArraignP1 : IConfiguration
    {
        public static ConfigEntry<float> BaseMaxHealth;
        public static ConfigEntry<float> BaseMoveSpeed;
        public static ConfigEntry<float> BaseDamage;
        public static ConfigEntry<float> BaseArmor;
        public static ConfigEntry<float> LevelMaxHealth;
        public static ConfigEntry<float> LevelDamage;
        public static ConfigEntry<float> LevelArmor;
        public static ConfigEntry<float> SprintMultiplier;
        public static ConfigEntry<int> HealthSegments;

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

        public static ConfigEntry<float> LeftRightSwingCooldown;
        public static ConfigEntry<float> LeftRightSwingDamage;
        public static ConfigEntry<float> LeftRightSwingForce;
        public static ConfigEntry<float> LeftRightSwingProcCoefficient;

        public static ConfigEntry<float> SwordBeamCooldown;
        public static ConfigEntry<float> SwordBeamForceFieldPushRadius;
        public static ConfigEntry<float> SwordBeamForceFieldPushStrength;
        public static ConfigEntry<float> SwordBeamDamage;
        public static ConfigEntry<float> SwordBeamProcCoefficient;
        public static ConfigEntry<float> SwordBeamDegreesPerSecond;
        public static ConfigEntry<float> SwordBeamDuration;

        public static ConfigEntry<float> SkyDropCooldown;
        public static ConfigEntry<float> SkyDropExplosionRadius;
        public static ConfigEntry<float> SkyDropExplosionForce;
        public static ConfigEntry<float> SkyDropFirstExplosionDamage;
        public static ConfigEntry<float> SkyDropSecondExplosionDamage;
        public static ConfigEntry<int> SkyDropWavesCount;
        public static ConfigEntry<float> SkyDropWavesDamage;
        public static ConfigEntry<float> SkyDropWavesForce;

        public static ConfigEntry<float> SwordThrowCooldown;
        public static ConfigEntry<float> SwordThrowDamage;
        public static ConfigEntry<float> SwordThrowForce;

        public static ConfigEntry<float> LightningStrikesCooldown;
        public static ConfigEntry<int> LightningStrikesCount;
        public static ConfigEntry<float> LightningStrikesDelayBetweenProjectiles;
        public static ConfigEntry<float> LightningStrikesMinDistance;
        public static ConfigEntry<float> LightningStrikesMaxDistance;
        public static ConfigEntry<float> LightningStrikesDamage;

        public void PopulateConfig(ConfigFile config)
        {
            BaseMaxHealth = config.Bind("Arraign P1 Character Stats", "Base Max Health", 4000f, "Arraign P1' base health.");
            BaseMoveSpeed = config.Bind("Arraign P1 Character Stats", "Base Movement Speed", 15f, "Arraign P1' base movement speed.");
            BaseDamage = config.Bind("Arraign P1 Character Stats", "Base Damage", 16f, "Arraign P1' base damage.");
            BaseArmor = config.Bind("Arraign P1 Character Stats", "Base Armor", 20f, "Arraign P1' base armor.");

            LevelMaxHealth = config.Bind("Arraign P1 Character Stats", "Health per Level", 1000f, "Arraign P1' health increase per level.");
            LevelDamage = config.Bind("Arraign P1 Character Stats", "Damage per Level", 3.2f, "Arraign P1' damage increase per level.");
            LevelArmor = config.Bind("Arraign P1 Character Stats", "Armor per Level", 0f, "Arraign P1' armor increase per level.");

            SprintMultiplier = config.Bind("Arraign P1 Character Stats", "Sprint Multiplier", 3.2f, "Arraign P1' health increase per level.");

            HealthSegments = config.Bind("Arraign P1 Character Stats", "Number of Health Segments", 5, "Arraign P1' number of health segments that you need to break with hammer.");

            ThreeHitComboCooldown = config.Bind("ThreeHitCombo", "ThreeHitCombo Cooldown", 12f, "ThreeHitCombo cooldown.");
            ThreeHitComboFirstSwingDamage = config.Bind("ThreeHitCombo", "ThreeHitCombo First Swing Damage", 3f, "ThreeHitCombo first swing damage.");
            ThreeHitComboFirstSwingForce = config.Bind("ThreeHitCombo", "ThreeHitCombo First Swing Force", 600f, "ThreeHitCombo first swing force.");
            ThreeHitComboFirstSwingProcCoefficient = config.Bind("ThreeHitCombo", "ThreeHitCombo First Swing Proc Coefficient", 1f, "ThreeHitCombo first swing proc coefficient.");

            ThreeHitComboSecondSwingDamage = config.Bind("ThreeHitCombo", "ThreeHitCombo Second Swing Damage", 3f, "ThreeHitCombo second swing damage.");
            ThreeHitComboSecondSwingForce = config.Bind("ThreeHitCombo", "ThreeHitCombo Second Swing Force", 600f, "ThreeHitCombo second swing force.");
            ThreeHitComboSecondSwingProcCoefficient = config.Bind("ThreeHitCombo", "ThreeHitCombo Second Swing Proc Coefficient", 1f, "ThreeHitCombo second swing proc coefficient.");

            ThreeHitComboThirdSwingDamage = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Damage", 3f, "ThreeHitCombo third swing damage.");
            ThreeHitComboThirdSwingForce = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Force", 600f, "ThreeHitCombo third swing force.");
            ThreeHitComboThirdSwingProcCoefficient = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Proc Coefficient", 1f, "ThreeHitCombo third swing proc coefficient.");

            ThreeHitComboExplosionRadius = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Explosion Radius", 20f, "ThreeHitCombo third swing explosion radius.");
            ThreeHitComboExplosionDamage = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Explosion Damage", 5f, "ThreeHitCombo third swing explosion damage.");
            ThreeHitComboExplosionProcCoefficient = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Explosion Proc Coefficient", 1f, "ThreeHitCombo third swing explosion proc coefficient.");
            ThreeHitComboExplosionForce = config.Bind("ThreeHitCombo", "ThreeHitCombo Third Swing Force", 500f, "ThreeHitCombo third swing proc explosion force.");

            LeftRightSwingCooldown = config.Bind("LeftRightSwing", "LeftRighSwingCooldown Cooldown", 0f, "LeftRightSwing cooldown.");
            LeftRightSwingDamage = config.Bind("LeftRightSwing", "LeftRighSwingCooldown Damage", 2f, "LeftRightSwing damage.");
            LeftRightSwingForce = config.Bind("LeftRightSwing", "LeftRighSwingCooldown Force", 600f, "LeftRightSwing force.");
            LeftRightSwingProcCoefficient = config.Bind("LeftRightSwing", "LeftRighSwingCooldown Proc Coefficient", 1f, "LeftRightSwing proc coefficient.");

            SwordBeamCooldown = config.Bind("SwordBeam", "SwordBeam Cooldown", 55f, "SwordBeam cooldown.");
            SwordBeamForceFieldPushRadius = config.Bind("SwordBeam", "SwordBeam Force Field Push Radius", 20f, "SwordBeam force field push radius.");
            SwordBeamForceFieldPushStrength = config.Bind("SwordBeam", "SwordBeam Force Field Strength", 15f, "SwordBeam force field push strength.");
            SwordBeamDamage = config.Bind("SwordBeam", "SwordBeam Damage", 10f, "SwordBeam damage.");
            SwordBeamProcCoefficient = config.Bind("SwordBeam", "SwordBeam Proc Coefficient", 1f, "SwordBeam proc coefficient.");
            SwordBeamDegreesPerSecond = config.Bind("SwordBeam", "SwordBeam Degrees Per Second", 40f, "SwordBeam rotation in degrees per second.");
            SwordBeamDuration = config.Bind("SwordBeam", "SwordBeam Duration", 10f, "SwordBeam duration of rotation portion of skill.");

            SkyDropCooldown = config.Bind("SkyDrop", "SkyDrop Cooldown", 25f, "SkyDrop cooldown.");
            SkyDropExplosionRadius = config.Bind("SkyDrop", "SkyDrop Explosion Radius", 20f, "SkyDrop explosion radius.");
            SkyDropExplosionForce = config.Bind("SkyDrop", "SkyDrop Explosion Force", 1000f, "SkyDrop explosion force.");
            SkyDropFirstExplosionDamage = config.Bind("SkyDrop", "SkyDrop First Explosion Damage", 8f, "SkyDrop first explosion damage.");
            SkyDropSecondExplosionDamage = config.Bind("SkyDrop", "SkyDrop Second Explosion Damage", 6f, "SkyDrop second explosion damage.");
            SkyDropWavesCount = config.Bind("SkyDrop", "SkyDrop Waves Projectiles Count", 8, "SkyDrop waves projectile count.");
            SkyDropWavesDamage = config.Bind("SkyDrop", "SkyDrop Waves Projectiles Damage", 4f, "SkyDrop waves projectile damage.");
            SkyDropWavesForce = config.Bind("SkyDrop", "SkyDrop Waves Projectiles Force", 0f, "SkyDrop waves projectile force.");

            SwordThrowCooldown = config.Bind("SwordThrow", "SwordThrow Cooldown", 10f, "SwordThrow cooldown.");
            SwordThrowDamage = config.Bind("SwordThrow", "SwordThrow Damage", 6f, "SwordThrow damage.");
            SwordThrowForce = config.Bind("SwordThrow", "SwortThrow Force", 0f, "SwordThrow force.");

            LightningStrikesCooldown = config.Bind("LightningStrikes", "LightningStrikes Cooldown", 15f, "LightningStrikes cooldown");
            LightningStrikesCount = config.Bind("LightningStrikes", "LightningStrikes Count", 20, "LightningStrikes projectile count");
            LightningStrikesDelayBetweenProjectiles = config.Bind("LightningStrikes", "LightningStrikes Delay Between Projeciles", 0.1f, "LightningStrikes delay between projectiles.");
            LightningStrikesMinDistance = config.Bind("LightningStrikes", "LightningStrikes Min Projectile Distance", 0f, "LightningStrikes minimum projectile spawn distance from target.");
            LightningStrikesMaxDistance = config.Bind("LightningStrikes", "LightningStrikes Max Projectile Distance", 30f, "LightningStrikes maximum projectile spawn distance from target.");
            LightningStrikesDamage = config.Bind("LightningStrikes", "LightningStrikes Damage", 4f, "LightningStrikes damage.");
        }
    }
}
