using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration.ContactLight.Providence
{
    public class ProvidencePhase2 : IConfiguration
    {
        public static ConfigEntry<float> BaseMaxHealth;
        public static ConfigEntry<float> BaseMoveSpeed;
        public static ConfigEntry<float> BaseDamage;
        public static ConfigEntry<float> BaseArmor;
        public static ConfigEntry<float> LevelMaxHealth;
        public static ConfigEntry<float> LevelDamage;
        public static ConfigEntry<float> LevelArmor;

        public static ConfigEntry<float> P2PrimaryDamage;
        public static ConfigEntry<float> P2PrimaryDuration;
        public static ConfigEntry<float> P2PrimaryProcCoefficient;
        public static ConfigEntry<float> P2PrimaryPushAwayForce;
        public static ConfigEntry<float> P2PrimaryEarlyExit;
        public static ConfigEntry<float> P2PrimaryCooldown;

        public static ConfigEntry<float> P2PrimaryCloneSearchDistance;
        public static ConfigEntry<float> P2PrimaryCloneMaxSearchAngle;
        public static ConfigEntry<float> P2PrimaryCloneSpeed;
        public static ConfigEntry<float> P2PrimaryCloneDamage;
        public static ConfigEntry<float> P2PrimaryCloneHealthThreshold;

        public static ConfigEntry<float> P2OrbsDamage;
        public static ConfigEntry<float> P2OrbsDuration;
        public static ConfigEntry<float> P2OrbsProjectileSpawnTime;
        public static ConfigEntry<int> P2OrbsProjectileCount;
        public static ConfigEntry<float> P2OrbsDelayBetween;

        public static ConfigEntry<float> P2SecondaryStats;
        public static ConfigEntry<float> P2SecondaryCooldown;

        public static ConfigEntry<float> P2SecondaryCloneDamage;
        public static ConfigEntry<int> P2SecondaryCloneMinSpawns;
        public static ConfigEntry<int> P2SecondaryCloneMaxSpawns;
        public static ConfigEntry<float> P2SecondaryCloneMinHealthThreshold;
        public static ConfigEntry<float> P2SecondaryCloneMaxHealthThreshold;
        public static ConfigEntry<float> P2SecondaryCloneSpeed;


        public static ConfigEntry<float> P2UtilityPredictionTimer;
        public static ConfigEntry<float> P2UtilityDuration;
        public static ConfigEntry<float> P2UtilityEarlyExit;
        public static ConfigEntry<float> P2UtilityDamage;
        public static ConfigEntry<float> P2UtilityRadius;
        public static ConfigEntry<int> P2UtilityOrbCount;
        public static ConfigEntry<float> P2UtilityOrbProjectileDamage;
        public static ConfigEntry<byte> P2UtilityOrbJumps;
        public static ConfigEntry<float> P2UtilityOrbSpeed;
        public static ConfigEntry<float> P2UtilityOrbInitialAngle;
        public static ConfigEntry<float> P2UtilityCooldown;

        public static ConfigEntry<float> P2UtilityCloneDamage;
        public static ConfigEntry<int> P2UtilityCloneMinCount;
        public static ConfigEntry<int> P2UtilityCloneMaxCount;
        public static ConfigEntry<float> P2UtilityCloneDelayBetweenProjectiles;
        public static ConfigEntry<float> P2UtilityCloneMinHealthThreshold;
        public static ConfigEntry<float> P2UtilityCloneMaxHealthThreshold;

        public static ConfigEntry<int> P2SpecialTimesToFire;
        public static ConfigEntry<int> P2SpecialRingsToFire;
        public static ConfigEntry<int> P2SpecialAdditionalRingsMin;
        public static ConfigEntry<int> P2SpecialAdditionalRingsMax;
        public static ConfigEntry<int> P2SpecialAdditionalTimesMin;
        public static ConfigEntry<int> P2SpecialAdditionalTimesMax;
        public static ConfigEntry<float> P2SpecialDamage;
        public static ConfigEntry<float> P2SpecialForce;
        public static ConfigEntry<float> P2SpecialHealthModifiersMax;
        public static ConfigEntry<float> P2SpecialHealthModifiersMin;
        public static ConfigEntry<float> P2SpecialCooldown;

        public static ConfigEntry<float> P2SkullsDamage;
        public static ConfigEntry<float> P2SkullsFireFrequency;
        public static ConfigEntry<int> P2SkullsProjectileCount;
        public static ConfigEntry<int> P2SkullsProjectileCountPerPlayer;
        public static ConfigEntry<float> P2SkullsProjectileSpeed;
        public static ConfigEntry<float> P2SkullsMaxDistance;
        public static ConfigEntry<bool> P2SkullsCanBeRed;
        public static ConfigEntry<float> P2SkullsCooldown;

        public void PopulateConfig(ConfigFile config)
        {
            BaseMaxHealth = config.Bind("Providence P2 Character Stats", "Base Max Health", 4000f, "Providence P2' base health.");
            BaseMoveSpeed = config.Bind("Providence P2 Character Stats", "Base Movement Speed", 10f, "Providence P2' base movement speed.");
            BaseDamage = config.Bind("Providence P2 Character Stats", "Base Damage", 16f, "Providence P2' base damage.");
            BaseArmor = config.Bind("Providence P2 Character Stats", "Base Armor", 20f, "Providence P2' base armor.");

            LevelMaxHealth = config.Bind("Providence P2 Character Stats", "Health per Level", 1200f, "Providence P2' health increase per level.");
            LevelDamage = config.Bind("Providence P2 Character Stats", "Damage per Level", 3.2f, "Providence P2' damage increase per level.");
            LevelArmor = config.Bind("Providence P2 Character Stats", "Armor per Level", 0f, "Providence P2' armor increase per level.");

            P2PrimaryDamage = config.Bind("Phase 2 Primary", "Primary Damage", 2.5f, "Damage of both swings.");
            P2PrimaryDuration = config.Bind("Phase 2 Primary", "Primary Duration", 3f, "Duration of the state, in seconds.");
            P2PrimaryProcCoefficient = config.Bind("Phase 2 Primary", "Primary Proc Coefficient", 1f, "Proc coefficient of swings.");
            P2PrimaryPushAwayForce = config.Bind("Phase 2 Primary", "Primary Push Away Force", 500f, "Push away force of primary swings.");
            P2PrimaryEarlyExit = config.Bind("Phase 2 Primary", "Early Exit Duration", 2.5f, "Early exit from swings into firing orbs if no target was hit with the swing.");
            P2PrimaryCooldown = config.Bind("Phase 2 Primary", "Primary Cooldown", 5f, "Primary skill cooldown.");

            P2PrimaryCloneSearchDistance = config.Bind("Phase 2 Primary Shadow Clones", "Search Distance", 250f, "How far Providence searches for target when firing shadow clone from primary.");
            P2PrimaryCloneMaxSearchAngle = config.Bind("Phase 2 Primary Shadow Clones", "Max Search Angle", 75f, "Max search angle for shadow clone projectile before firing.");
            P2PrimaryCloneSpeed = config.Bind("Phase 2 Primary Shadow Clones", "Primary Clone Projectile Speed", 50f, "Primary shadow clone projectile speed.");
            P2PrimaryCloneDamage = config.Bind("Phase 2 Primary Shadow Clones", "Primary Clone Projectile Damage", 2.5f, "Primary shadow clone projectile damage.");
            P2PrimaryCloneHealthThreshold = config.Bind("Phase 2 Primary Shadow Clones", "Primary Clone Firing Health Threshold", 1f, "From what HP threshold Providence starts firing shadow clones from primary.");

            P2OrbsDamage = config.Bind("Phase 2 Primary Orbs", "Orbs Damage", 2f, "Damage of orbs that are fired after primary swings.");
            P2OrbsDuration = config.Bind("Phase 2 Primary Orbs", "Orbs Duration", 2.2f, "Duration of orbs firing state. State will stop even if not all projectiles are fired so adjust accordingly.");
            P2OrbsProjectileSpawnTime = config.Bind("Phase 2 Primary Orbs", "Orbs Initial Delay", 1f, "Duration of when orbs are fired after state is entered.");
            P2OrbsProjectileCount = config.Bind("Phase 2 Primary Orbs", "Orbs Count", 5, "Number of orbs fired. If above 3 then spawn position will be rotated from the start. Increasing count without increasing state duration can result in not all orbs being fired.");
            P2OrbsDelayBetween = config.Bind("Phase 2 Primary Orbs", "Orbs Delay Between Spawn", 0.2f, "Delay between projectiles being fired.");

            P2SecondaryStats = config.Bind("Phase 2 Secondary", "Phase 2 Stats", 0f, "Dash shares the same values between phases (except for skill cooldown), so edit values in P1 config.");
            P2SecondaryCooldown = config.Bind("Phase 2 Secondary", "Secondary Cooldown", 10f, "Secondary skill cooldown.");

            P2SecondaryCloneDamage = config.Bind("Phase 2 Secondary Shadow Clones", "Secondary Clone Projecile Damage", 1.5f, "Damage of shadow clone fired from secondary.");
            P2SecondaryCloneMinSpawns = config.Bind("Phase 2 Secondary Shadow Clones", "Min Number of Clones", 1, "Minimal number of clones spawned from secondary. Scales of current HP.");
            P2SecondaryCloneMaxSpawns = config.Bind("Phase 2 Secondary Shadow Clones", "Max Number of Clones", 3, "Maximum number of clones spawned from secondary. Scales of current HP.");
            P2SecondaryCloneMinHealthThreshold = config.Bind("Phase 2 Secondary Shadow Clones", "Min Number of Clones Health Threshold", 1f, "From what health threshold number of clones starts scaling. Before that minimal value is always used.");
            P2SecondaryCloneMaxHealthThreshold = config.Bind("Phase 2 Secondary Shadow Clones", "Max Number of Clones Health Threshold", 0.25f, "To what health threshold number of clones scales. After that maximum value is always used.");
            P2SecondaryCloneSpeed = config.Bind("Phase 2 Secondary Shadow Clones", "Secondary Clone Projectile Speed", 80f, "Projectile speed.");

            P2UtilityPredictionTimer = config.Bind("Phase 2 Utility", "Utility Prediction Timer", 0.75f, "Utility prediction timer, how far into the future prediction position for the target is.");
            P2UtilityDuration = config.Bind("Phase 2 Utility", "Utility Duration", 3.5f, "Utility landing state duration.");
            P2UtilityEarlyExit = config.Bind("Phase 2 Utility", "Utility Early Exit Duration", 1.25f, "Utility early exit into secondary duration. Basically when from the start of the state Providence can cancel landing into dash.");
            P2UtilityDamage = config.Bind("Phase 2 Utility", "Utility Landing Damage", 6f, "Damage of utility's landing.");
            P2UtilityRadius = config.Bind("Phase 2 Utility", "Utility Landing Radius", 8f, "Radius of utility's landing damage. It uses BulletAttack and is infinitely tall.");
            P2UtilityOrbProjectileDamage = config.Bind("Phase 2 Utility", "Utility Orb Damage", 4f, "Damage of utility's orb explosion.");
            P2UtilityOrbCount = config.Bind("Phase 2 Utility", "Utility Orb Count", 8, "Number of orbs.");
            P2UtilityOrbJumps = config.Bind<byte>("Phase 2 Utility", "Utility Orb Jumps", 2, "How many times orbs jump when hitting the ground.");
            P2UtilityOrbSpeed = config.Bind("Phase 2 Utility", "Utility Orb Speed", 20f, "Utility's orb speed.");
            P2UtilityOrbInitialAngle = config.Bind("Phase 2 Utility", "Utility Orb Initial Angle", 45f, "Angle of the initial projectile fired from Providence. Doesn't affect the angle of the projectiles fired after bounce.");
            P2UtilityCooldown = config.Bind("Phase 2 Utility", "Utility Cooldown", 15f, "Utility skill cooldown.");

            P2UtilityCloneDamage = config.Bind("Phase 2 Utility Shadow Clones", "Utility Clones Damage", 3f, "Utility clones projectile damage.");
            P2UtilityCloneMinCount = config.Bind("Phase 2 Utility Shadow Clones", "Utility Min Clones Count", 5, "Minimal number of spawned shadow clones. Scales of current HP.");
            P2UtilityCloneMaxCount = config.Bind("Phase 2 Utility Shadow Clones", "Utility Max Clones Count", 10, "Maximum number of spawned shadow clones. Scales of current HP.");
            P2UtilityCloneDelayBetweenProjectiles = config.Bind("Phase 2 Utility Shadow Clones", "Utility Clones Delay Between Projeciles", 0.2f, "Delay between spawning projectiles.");
            P2UtilityCloneMinHealthThreshold = config.Bind("Phase 2 Utility Shadow Clones", "Utility Min Clones Count Health Threshold", 1f, "Minimal health threshold at which numbef of projeciles start scaling. Values before that use minimal count.");
            P2UtilityCloneMaxHealthThreshold = config.Bind("Phase 2 Utility Shadow Clones", "Utility Max Clones Count Health Threshold", 0.25f, "Maximum health threshold at which numbef of projeciles start scaling. Values after that use maximum count.");

            P2SpecialTimesToFire = config.Bind("Phase 2 Special", "Special Times To Fire", 3, "How many times rings are fired.");
            P2SpecialRingsToFire = config.Bind("Phase 2 Special", "Special Rings To Fire", 2, "How many rings are fired. Rings themselves are selected from the premade rng table to ensure that attack is dodgable with low movement speed.");
            P2SpecialAdditionalRingsMin = config.Bind("Phase 2 Special", "Special Minumim Additional Rings", 1, "How many additional rings are added off Providence current HP.");
            P2SpecialAdditionalRingsMax = config.Bind("Phase 2 Special", "Special Maximum Additional Rings", 2, "How many additional rings are added off Providence current HP.");
            P2SpecialAdditionalTimesMin = config.Bind("Phase 2 Special", "Special Minimum Additional Times To Fire", 1, "How many additional times rings are fired off Providence current HP.");
            P2SpecialAdditionalTimesMax = config.Bind("Phase 2 Special", "Special Maximum Additional Times To Fire", 3, "How many additional times rings are fired off Providence current HP.");
            P2SpecialDamage = config.Bind("Phase 2 Special", "Special Damage", 9f, "Damage of special's overlap attack.");
            P2SpecialForce = config.Bind("Phase 2 Special", "Special Force", 300f, "Special's up force that is applied if attack landed.");
            P2SpecialHealthModifiersMax = config.Bind("Phase 2 Special", "Special Modifiers Minimal HP", 0.3f, "Health's proportion at which maximum additional utility modifiers are applied.");
            P2SpecialHealthModifiersMin = config.Bind("Phase 2 Special", "Special Modifiers Maximum HP", 0.8f, "Health's proportion at which minimum additional utility modifiers are applied.");
            P2SpecialCooldown = config.Bind("Phase 2 Special", "Special Cooldown", 60f, "Special skill cooldown.");

            P2SkullsDamage = config.Bind("Phase 2 Skulls", "Skulls Damage", 5f, "Damage of normal skull attack.");
            P2SkullsFireFrequency = config.Bind("Phase 2 Skulls", "Skulls Fire Frequency", 0.3f, "How frequently skulls are fired.");
            P2SkullsProjectileCount = config.Bind("Phase 2 Skulls", "Skulls Count", 25, "Base number of skulls spawned by the attack.");
            P2SkullsProjectileCountPerPlayer = config.Bind("Phase 2 Skulls", "Additional Skulls Count Per Player", 15, "Additional skulls spawned by the attack per player (above 1).");
            P2SkullsProjectileSpeed = config.Bind("Phase 2 Skulls", "Skulls Projectile Speed", 50f, "Skulls projectile speed");
            P2SkullsMaxDistance = config.Bind("Phase 2 Skulls", "Skulls Max Distance Spawn From Target", 15f, "How far from the target will projectile target spawn. It will spawn within these limits on all axis.");
            P2SkullsCanBeRed = config.Bind("Phase 2 Skulls", "Skulls Can Be Red", true, "Projectile can be red, red projectiles deal non-lethal 99999 damage.");
            P2SkullsCooldown = config.Bind("Phase 2 Skulls", "Skulls Cooldown", 45f, "Skulls skill cooldown. Skulls are used on health thresholds, so cooldown is only a part of when it is used.");
        }
    }
}
