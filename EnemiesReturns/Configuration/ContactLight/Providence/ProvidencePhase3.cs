using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration.ContactLight.Providence
{
    public class ProvidencePhase3 : IConfiguration
    {
        public static ConfigEntry<float> BaseMaxHealth;
        public static ConfigEntry<float> BaseMoveSpeed;
        public static ConfigEntry<float> BaseDamage;
        public static ConfigEntry<float> BaseArmor;
        public static ConfigEntry<float> LevelMaxHealth;
        public static ConfigEntry<float> LevelDamage;
        public static ConfigEntry<float> LevelArmor;

        public static ConfigEntry<float> P3PrimaryDamage;
        public static ConfigEntry<float> P3PrimaryDuration;
        public static ConfigEntry<float> P3PrimaryProcCoefficient;
        public static ConfigEntry<float> P3PrimaryPushAwayForce;
        public static ConfigEntry<float> P3PrimaryCooldown;

        public static ConfigEntry<float> P3PrimaryCloneSearchDistance;
        public static ConfigEntry<float> P3PrimaryCloneMaxSearchAngle;
        public static ConfigEntry<float> P3PrimaryCloneSpeed;
        public static ConfigEntry<float> P3PrimaryCloneDamage;
        public static ConfigEntry<int> P3PrimaryCloneCount;
        public static ConfigEntry<float> P3PrimaryCloneSpread;

        public static ConfigEntry<float> P3SecondaryCloneDamage;
        public static ConfigEntry<int> P3SecondaryCloneMinSpawns;
        public static ConfigEntry<int> P3SecondaryCloneMaxSpawns;
        public static ConfigEntry<float> P3SecondaryCloneMinHealthThreshold;
        public static ConfigEntry<float> P3SecondaryCloneMaxHealthThreshold;
        public static ConfigEntry<float> P3SecondaryCloneSpeed;

        public static ConfigEntry<float> P3UtilityPredictionTimer;
        public static ConfigEntry<float> P3UtilityDuration;
        public static ConfigEntry<float> P3UtilityEarlyExit;
        public static ConfigEntry<float> P3UtilityDamage;
        public static ConfigEntry<float> P3UtilityRadius;
        public static ConfigEntry<int> P3UtilityOrbCount;
        public static ConfigEntry<float> P3UtilityOrbProjectileDamage;
        public static ConfigEntry<byte> P3UtilityOrbJumps;
        public static ConfigEntry<float> P3UtilityOrbSpeed;
        public static ConfigEntry<float> P3UtilityOrbInitialAngle;
        public static ConfigEntry<float> P3UtilityCooldown;

        public static ConfigEntry<float> P3UtilityCloneDamage;
        public static ConfigEntry<int> P3UtilityCloneMinCount;
        public static ConfigEntry<int> P3UtilityCloneMaxCount;
        public static ConfigEntry<float> P3UtilityCloneDelayBetweenProjectiles;
        public static ConfigEntry<float> P3UtilityCloneMinHealthThreshold;
        public static ConfigEntry<float> P3UtilityCloneMaxHealthThreshold;

        //public static ConfigEntry<int> P3SpecialTimesToFire;
        //public static ConfigEntry<int> P3SpecialRingsToFire;
        //public static ConfigEntry<int> P3SpecialAdditionalRingsMin;
        //public static ConfigEntry<int> P3SpecialAdditionalRingsMax;
        //public static ConfigEntry<int> P3SpecialAdditionalTimesMin;
        //public static ConfigEntry<int> P3SpecialAdditionalTimesMax;
        //public static ConfigEntry<float> P3SpecialDamage;
        //public static ConfigEntry<float> P3SpecialForce;
        //public static ConfigEntry<float> P3SpecialHealthModifiersMax;
        //public static ConfigEntry<float> P3SpecialHealthModifiersMin;
        //public static ConfigEntry<float> P3SpecialCooldown;

        public static ConfigEntry<float> P3SkullsDamage;
        public static ConfigEntry<float> P3SkullsFireFrequency;
        public static ConfigEntry<int> P3SkullsProjectileCount;
        public static ConfigEntry<int> P3SkullsProjectileCountPerPlayer;
        public static ConfigEntry<float> P3SkullsProjectileSpeed;
        public static ConfigEntry<float> P3SkullsMaxDistance;
        public static ConfigEntry<bool> P3SkullsCanBeRed;
        public static ConfigEntry<float> P3SkullsCooldown;

        public static ConfigEntry<float> P3LaserDamage;
        public static ConfigEntry<float> P3LaserRotationSpeed;
        public static ConfigEntry<float> P3LaserLifetime;
        public static ConfigEntry<float> P3LaserCooldown;
        public static ConfigEntry<float> P3LaserOverlapAttackResetInverval;


        public void PopulateConfig(ConfigFile config)
        {
            BaseMaxHealth = config.Bind("Providence P3 Character Stats", "Base Max Health", 4000f, "Providence P3' base health.");
            BaseMoveSpeed = config.Bind("Providence P3 Character Stats", "Base Movement Speed", 10f, "Providence P3' base movement speed.");
            BaseDamage = config.Bind("Providence P3 Character Stats", "Base Damage", 16f, "Providence P3' base damage.");
            BaseArmor = config.Bind("Providence P3 Character Stats", "Base Armor", 20f, "Providence P3' base armor.");

            LevelMaxHealth = config.Bind("Providence P3 Character Stats", "Health per Level", 1200f, "Providence P3' health increase per level.");
            LevelDamage = config.Bind("Providence P3 Character Stats", "Damage per Level", 3.2f, "Providence P3' damage increase per level.");
            LevelArmor = config.Bind("Providence P3 Character Stats", "Armor per Level", 0f, "Providence P3' armor increase per level.");

            P3PrimaryDamage = config.Bind("Phase 3 Primary", "Primary Damage", 2.5f, "Damage of both swings.");
            P3PrimaryDuration = config.Bind("Phase 3 Primary", "Primary Duration", 3f, "Duration of the state, in seconds.");
            P3PrimaryProcCoefficient = config.Bind("Phase 3 Primary", "Primary Proc Coefficient", 1f, "Proc coefficient of swings.");
            P3PrimaryPushAwayForce = config.Bind("Phase 3 Primary", "Primary Push Away Force", 500f, "Push away force of primary swings.");
            P3PrimaryCooldown = config.Bind("Phase 3 Primary", "Primary Cooldown", 5f, "Primary skill cooldown.");

            P3PrimaryCloneSearchDistance = config.Bind("Phase 3 Primary Shadow Clones", "Search Distance", 100f, "How far Providence searches for target when firing shadow clone from primary.");
            P3PrimaryCloneMaxSearchAngle = config.Bind("Phase 3 Primary Shadow Clones", "Max Search Angle", 75f, "Max search angle for shadow clone projectile before firing.");
            P3PrimaryCloneSpeed = config.Bind("Phase 3 Primary Shadow Clones", "Primary Clone Projectile Speed", 45f, "Primary shadow clone projectile speed.");
            P3PrimaryCloneDamage = config.Bind("Phase 3 Primary Shadow Clones", "Primary Clone Projectile Damage", 2.5f, "Primary shadow clone projectile damage.");
            P3PrimaryCloneCount = config.Bind("Phase 3 Primary Shadow Clones", "Phase 3 Primary Shadow Clones Count", 3, "Number of clones spawned from primary.");
            P3PrimaryCloneSpread = config.Bind("Phase 3 Primary Shadow Clones", "Phase 3 Primary Shadow Clones Angle Spread", 45f, "Angle within which shadow clones are spread from primary.");

            P3UtilityPredictionTimer = config.Bind("Phase 3 Utility", "Utility Prediction Timer", 0.75f, "Utility prediction timer, how far into the future prediction position for the target is.");
            P3UtilityDuration = config.Bind("Phase 3 Utility", "Utility Duration", 3.5f, "Utility landing state duration.");
            P3UtilityEarlyExit = config.Bind("Phase 3 Utility", "Utility Early Exit Duration", 1.25f, "Utility early exit into secondary duration. Basically when from the start of the state Providence can cancel landing into dash.");
            P3UtilityDamage = config.Bind("Phase 3 Utility", "Utility Landing Damage", 6f, "Damage of utility's landing.");
            P3UtilityRadius = config.Bind("Phase 3 Utility", "Utility Landing Radius", 8f, "Radius of utility's landing damage. It uses BulletAttack and is infinitely tall.");
            P3UtilityOrbProjectileDamage = config.Bind("Phase 3 Utility", "Utility Orb Damage", 4f, "Damage of utility's orb explosion.");
            P3UtilityOrbCount = config.Bind("Phase 3 Utility", "Utility Orb Count", 4, "Number of orbs.");
            P3UtilityOrbJumps = config.Bind<byte>("Phase 3 Utility", "Utility Orb Jumps", 2, "How many times orbs jump when hitting the ground.");
            P3UtilityOrbSpeed = config.Bind("Phase 3 Utility", "Utility Orb Speed", 20f, "Utility's orb speed.");
            P3UtilityOrbInitialAngle = config.Bind("Phase 3 Utility", "Utility Orb Initial Angle", 45f, "Angle of the initial projectile fired from Providence. Doesn't affect the angle of the projectiles fired after bounce.");
            P3UtilityCooldown = config.Bind("Phase 3 Utility", "Utility Cooldown", 17.5f, "Utility skill cooldown.");

            P3UtilityCloneDamage = config.Bind("Phase 3 Utility Shadow Clones", "Utility Clones Damage", 3f, "Utility clones projectile damage.");
            P3UtilityCloneMinCount = config.Bind("Phase 3 Utility Shadow Clones", "Utility Min Clones Count", 5, "Minimal number of spawned shadow clones. Scales of current HP.");
            P3UtilityCloneMaxCount = config.Bind("Phase 3 Utility Shadow Clones", "Utility Max Clones Count", 5, "Maximum number of spawned shadow clones. Scales of current HP.");
            P3UtilityCloneDelayBetweenProjectiles = config.Bind("Phase 3 Utility Shadow Clones", "Utility Clones Delay Between Projeciles", 0.2f, "Delay between spawning projectiles.");
            P3UtilityCloneMinHealthThreshold = config.Bind("Phase 3 Utility Shadow Clones", "Utility Min Clones Count Health Threshold", 1f, "Minimal health threshold at which numbef of projeciles start scaling. Values before that use minimal count.");
            P3UtilityCloneMaxHealthThreshold = config.Bind("Phase 3 Utility Shadow Clones", "Utility Max Clones Count Health Threshold", 0.25f, "Maximum health threshold at which numbef of projeciles start scaling. Values after that use maximum count.");

            //P3SpecialTimesToFire = config.Bind("Phase 3 Special", "Special Times To Fire", 3, "How many times rings are fired.");
            //P3SpecialRingsToFire = config.Bind("Phase 3 Special", "Special Rings To Fire", 2, "How many rings are fired. Rings themselves are selected from the premade rng table to ensure that attack is dodgable with low movement speed.");
            //P3SpecialAdditionalRingsMin = config.Bind("Phase 3 Special", "Special Minumim Additional Rings", 1, "How many additional rings are added off Providence current HP.");
            //P3SpecialAdditionalRingsMax = config.Bind("Phase 3 Special", "Special Maximum Additional Rings", 2, "How many additional rings are added off Providence current HP.");
            //P3SpecialAdditionalTimesMin = config.Bind("Phase 3 Special", "Special Minimum Additional Times To Fire", 1, "How many additional times rings are fired off Providence current HP.");
            //P3SpecialAdditionalTimesMax = config.Bind("Phase 3 Special", "Special Minimum Additional Times To Fire", 3, "How many additional times rings are fired off Providence current HP.");
            //P3SpecialDamage = config.Bind("Phase 3 Special", "Special Damage", 9f, "Damage of special's overlap attack.");
            //P3SpecialForce = config.Bind("Phase 3 Special", "Special Force", 300f, "Special's up force that is applied if attack landed.");
            //P3SpecialHealthModifiersMax = config.Bind("Phase 3 Special", "Special Modifiers Minimal HP", 0.3f, "Health's proportion at which maximum additional utility modifiers are applied.");
            //P3SpecialHealthModifiersMin = config.Bind("Phase 3 Special", "Special Modifiers Minimal HP", 0.8f, "Health's proportion at which minimum additional utility modifiers are applied.");
            //P3SpecialCooldown = config.Bind("Phase 3 Special", "Special Cooldown", 60f, "Special skill cooldown.");

            P3SkullsDamage = config.Bind("Phase 3 Skulls", "Skulls Damage", 5f, "Damage of normal skull attack.");
            P3SkullsFireFrequency = config.Bind("Phase 3 Skulls", "Skulls Fire Frequency", 0.3f, "How frequently skulls are fired.");
            P3SkullsProjectileCount = config.Bind("Phase 3 Skulls", "Skulls Count", 10, "Base number of skulls spawned by the attack.");
            P3SkullsProjectileCountPerPlayer = config.Bind("Phase 3 Skulls", "Additional Skulls Count Per Player", 5, "Additional skulls spawned by the attack per player (above 1).");
            P3SkullsProjectileSpeed = config.Bind("Phase 3 Skulls", "Skulls Projectile Speed", 50f, "Skulls projectile speed");
            P3SkullsMaxDistance = config.Bind("Phase 3 Skulls", "Skulls Max Distance Spawn From Target", 15f, "How far from the target will projectile target spawn. It will spawn within these limits on all axis.");
            P3SkullsCanBeRed = config.Bind("Phase 3 Skulls", "Skulls Can Be Red", true, "Projectile can be red, red projectiles deal non-lethal 99999 damage.");
            P3SkullsCooldown = config.Bind("Phase 3 Skulls", "Skulls Cooldown", 20f, "Skulls skill cooldown. Skulls are used on health thresholds, so cooldown is only a part of when it is used.");

            P3LaserDamage = config.Bind("Phase 3 Laser", "Rotating Laser Damage", 9f, "Damage of each tick of rotating laser's overlap attack.");
            P3LaserCooldown = config.Bind("Phase 3 Laser", "Rotating Laser Cooldown", 70f, "Cooldown of rotating laser skill.");
            P3LaserLifetime = config.Bind("Phase 3 Laser", "Rotating Laser Lifetime", 40f, "Lifetime of rotating laser projectile.");
            P3LaserRotationSpeed = config.Bind("Phase 3 Laser", "Rotating Laser Rotation Speed", 5f, "Rotation speed, in degrees per second, of laser.");
            P3LaserOverlapAttackResetInverval = config.Bind("Phase 3 Laser", "Rotating Laser Overlap Attack Reset Interval", 0.25f, "Reset timer of rotating laser's overlap attack.");
        }
    }
}
