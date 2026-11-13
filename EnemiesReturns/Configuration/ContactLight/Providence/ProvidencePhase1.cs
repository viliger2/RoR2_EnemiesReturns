using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration.ContactLight.Providence
{
    public class ProvidencePhase1 : IConfiguration
    {
        public static ConfigEntry<float> BaseMaxHealth;
        public static ConfigEntry<float> BaseMoveSpeed;
        public static ConfigEntry<float> BaseDamage;
        public static ConfigEntry<float> BaseArmor;
        public static ConfigEntry<float> LevelMaxHealth;
        public static ConfigEntry<float> LevelDamage;
        public static ConfigEntry<float> LevelArmor;

        public static ConfigEntry<float> P1PrimaryDamage;
        public static ConfigEntry<float> P1PrimaryDuration;
        public static ConfigEntry<float> P1PrimaryProcCoefficient;
        public static ConfigEntry<float> P1PrimaryPushAwayForce;
        public static ConfigEntry<float> P1PrimaryEarlyExit;
        public static ConfigEntry<float> P1PrimaryCooldown;

        public static ConfigEntry<float> P1OrbsDamage;
        public static ConfigEntry<float> P1OrbsDuration;
        public static ConfigEntry<float> P1OrbsProjectileSpawnTime;
        public static ConfigEntry<int> P1OrbsProjectileCount;
        public static ConfigEntry<float> P1OrbsDelayBetween;

        public static ConfigEntry<float> PrimaryOrbsExplosionRadius;
        public static ConfigEntry<float> PrimaryOrbsLifetime;
        public static ConfigEntry<float> PrimaryOrbsRotationSpeed;

        public static ConfigEntry<float> P1SecondaryDuration;
        public static ConfigEntry<float> P1SecondaryDamage;
        public static ConfigEntry<float> P1SecondaryProcCoefficient;
        public static ConfigEntry<float> P1SecondaryPushAwayForce;
        public static ConfigEntry<float> P1SecondaryForwardSpeedCoef;
        public static ConfigEntry<float> P1SecondaryCooldown;

        public static ConfigEntry<float> P1UtilityPredictionTimer;
        public static ConfigEntry<float> P1UtilityDisappearDuration;
        public static ConfigEntry<float> P1UtilityDuration;
        public static ConfigEntry<float> P1UtilityEarlyExit;
        public static ConfigEntry<float> P1UtilityDamage;
        public static ConfigEntry<float> P1UtilityRadius;
        public static ConfigEntry<int> P1UtilityOrbCount;
        public static ConfigEntry<float> P1UtilityOrbProjectileDamage;
        public static ConfigEntry<byte> P1UtilityOrbJumps;
        public static ConfigEntry<float> P1UtilityOrbSpeed;
        public static ConfigEntry<float> P1UtilityOrbInitialAngle;
        public static ConfigEntry<float> P1UtilityCooldown;

        public static ConfigEntry<float> UtilityOrbBlastRadius;
        public static ConfigEntry<float> UtilityOrbOverlapAttackDamage;

        public static ConfigEntry<int> P1SpecialTimesToFire;
        public static ConfigEntry<int> P1SpecialRingsToFire;
        public static ConfigEntry<int> P1SpecialAdditionalRingsMin;
        public static ConfigEntry<int> P1SpecialAdditionalRingsMax;
        public static ConfigEntry<int> P1SpecialAdditionalTimesMin;
        public static ConfigEntry<int> P1SpecialAdditionalTimesMax;
        public static ConfigEntry<float> P1SpecialDamage;
        public static ConfigEntry<float> P1SpecialForce;
        public static ConfigEntry<float> P1SpecialHealthModifiersMax;
        public static ConfigEntry<float> P1SpecialHealthModifiersMin;
        public static ConfigEntry<float> P1SpecialCooldown;

        public static ConfigEntry<float> P1SkullsDamage;
        public static ConfigEntry<float> P1SkullsFireFrequency;
        public static ConfigEntry<int> P1SkullsProjectileCount;
        public static ConfigEntry<int> P1SkullsProjectileCountPerPlayer;
        public static ConfigEntry<float> P1SkullsProjectileSpeed;
        public static ConfigEntry<float> P1SkullsMaxDistance;
        public static ConfigEntry<bool> P1SkullsCanBeRed;
        public static ConfigEntry<float> P1SkullsCooldown;

        public void PopulateConfig(ConfigFile config)
        {
            BaseMaxHealth = config.Bind("Providence P1 Character Stats", "Base Max Health", 4000f, "Providence P1' base health.");
            BaseMoveSpeed = config.Bind("Providence P1 Character Stats", "Base Movement Speed", 10f, "Providence P1' base movement speed.");
            BaseDamage = config.Bind("Providence P1 Character Stats", "Base Damage", 16f, "Providence P1' base damage.");
            BaseArmor = config.Bind("Providence P1 Character Stats", "Base Armor", 20f, "Providence P1' base armor.");

            LevelMaxHealth = config.Bind("Providence P1 Character Stats", "Health per Level", 1200f, "Providence P1' health increase per level.");
            LevelDamage = config.Bind("Providence P1 Character Stats", "Damage per Level", 3.2f, "Providence P1' damage increase per level.");
            LevelArmor = config.Bind("Providence P1 Character Stats", "Armor per Level", 0f, "Providence P1' armor increase per level.");

            P1PrimaryDamage = config.Bind("Phase 1 Primary", "Primary Damage", 2.5f, "Damage of both swings.");
            P1PrimaryDuration = config.Bind("Phase 1 Primary", "Primary Duration", 3f, "Duration of the state, in seconds.");
            P1PrimaryProcCoefficient = config.Bind("Phase 1 Primary", "Primary Proc Coefficient", 1f, "Proc coefficient of swings.");
            P1PrimaryPushAwayForce = config.Bind("Phase 1 Primary", "Primary Push Away Force", 500f, "Push away force of primary swings.");
            P1PrimaryEarlyExit = config.Bind("Phase 1 Primary", "Early Exit Duration", 2f, "Early exit from swings into firing orbs if no target was hit with the swing.");
            P1PrimaryCooldown = config.Bind("Phase 1 Primary", "Primary Cooldown", 3.5f, "Primary skill cooldown.");

            P1OrbsDamage = config.Bind("Phase 1 Primary Orbs", "Orbs Damage", 2f, "Damage of orbs that are fired after primary swings.");
            P1OrbsDuration = config.Bind("Phase 1 Primary Orbs", "Orbs Duration", 2.2f, "Duration of orbs firing state. State will stop even if not all projectiles are fired so adjust accordingly.");
            P1OrbsProjectileSpawnTime = config.Bind("Phase 1 Primary Orbs", "Orbs Initial Delay", 1f, "Duration of when orbs are fired after state is entered.");
            P1OrbsProjectileCount = config.Bind("Phase 1 Primary Orbs", "Orbs Count", 3, "Number of orbs fired. If above 3 then spawn position will be rotated from the start. Increasing count without increasing state duration can result in not all orbs being fired.");
            P1OrbsDelayBetween = config.Bind("Phase 1 Primary Orbs", "Orbs Delay Between Spawn", 0.25f, "Delay between projectiles being fired.");

            PrimaryOrbsExplosionRadius = config.Bind("All Phases Primary Orbs", "Orbs Blast Explosion Radius", 6f, "Orb explosion radius.");
            PrimaryOrbsLifetime = config.Bind("All Phases Primary Orbs", "Orbs Lifetime", 12f, "Orb lifetime");
            PrimaryOrbsRotationSpeed = config.Bind("All Phases Primary Orbs", "Orbs Homing Rotation Speed", 30f, "Orb homing rotation speed towards the target.");

            P1SecondaryDamage = config.Bind("Phase 1 Secondary", "Secondary Damage", 3f, "Damage of secondary dash.");
            P1SecondaryDuration = config.Bind("Phase 1 Secondary", "Secondary Duration", 0.75f, "Duration of the secondary dash.");
            P1SecondaryProcCoefficient = config.Bind("Phase 1 Secondary", "Secondary Proc Coefficient", 1f, "Proc coefficient of secondary dash.");
            P1SecondaryPushAwayForce = config.Bind("Phase 1 Secondary", "Secondary Push Away Force", 1000f, "Push away force of secondary dash.");
            P1SecondaryForwardSpeedCoef = config.Bind("Phase 1 Secondary", "Secondary Forward Speed Coefficient", 1.5f, "Forward speed coefficient of secondary dash. Scales off base speed.");
            P1SecondaryCooldown = config.Bind("Phase 1 Seconadry", "Seconadry Cooldown", 10f, "Seconadry skill cooldown.");

            P1UtilityPredictionTimer = config.Bind("Phase 1 Utility", "Utility Prediction Timer", 0.75f, "Utility prediction timer, how far into the future prediction position for the target is.");
            P1UtilityDisappearDuration = config.Bind("Phase 1 Utility", "Utility Invisibility Duration", 1f, "Utility invisibility duration, for how long Providence remain hidden before the attack.");
            P1UtilityDuration = config.Bind("Phase 1 Utility", "Utility Duration", 3.5f, "Utility landing state duration.");
            P1UtilityEarlyExit = config.Bind("Phase 1 Utility", "Utility Early Exit Duration", 1.25f, "Utility early exit into secondary duration. Basically when from the start of the state Providence can cancel landing into dash.");
            P1UtilityDamage = config.Bind("Phase 1 Utility", "Utility Landing Damage", 6f, "Damage of utility's landing.");
            P1UtilityRadius = config.Bind("Phase 1 Utility", "Utility Landing Radius", 8f, "Radius of utility's landing damage. It uses BulletAttack and is infinitely tall.");
            P1UtilityOrbProjectileDamage = config.Bind("Phase 1 Utility", "Utility Orb Damage", 4f, "Damage of utility's orb explosion.");
            P1UtilityOrbCount = config.Bind("Phase 1 Utility", "Utility Orb Count", 4, "Number of orbs.");
            P1UtilityOrbJumps = config.Bind<byte>("Phase 1 Utility", "Utility Orb Jumps", 1, "How many times orbs jump when hitting the ground.");
            P1UtilityOrbSpeed = config.Bind("Phase 1 Utility", "Utility Orb Speed", 20f, "Utility's orb speed.");
            P1UtilityOrbInitialAngle = config.Bind("Phase 1 Utility", "Utility Orb Initial Angle", 45f, "Angle of the initial projectile fired from Providence. Doesn't affect the angle of the projectiles fired after bounce.");
            P1UtilityCooldown = config.Bind("Phase 1 Utility", "Utility Cooldown", 15f, "Utility skill cooldown.");

            UtilityOrbBlastRadius = config.Bind("All Phases Utility", "Orb Blast Radius", 6f, "Orb explosion radius. Used for all phases.");
            UtilityOrbOverlapAttackDamage = config.Bind("All Phases Utility", "Orb Overlap Attack Damage", 0.25f, "Orb overlap attack damage, scales of projectile damage. Used for all phases.");

            P1SpecialTimesToFire = config.Bind("Phase 1 Special", "Special Times To Fire", 2, "How many times rings are fired.");
            P1SpecialRingsToFire = config.Bind("Phase 1 Special", "Special Rings To Fire", 1, "How many rings are fired. Rings themselves are selected from the premade rng table to ensure that attack is dodgable with low movement speed.");
            P1SpecialAdditionalRingsMin = config.Bind("Phase 1 Special", "Special Minumim Additional Rings", 1, "How many additional rings are added off Providence current HP.");
            P1SpecialAdditionalRingsMax = config.Bind("Phase 1 Special", "Special Maximum Additional Rings", 1, "How many additional rings are added off Providence current HP.");
            P1SpecialAdditionalTimesMin = config.Bind("Phase 1 Special", "Special Minimum Additional Times To Fire", 1, "How many additional times rings are fired off Providence current HP.");
            P1SpecialAdditionalTimesMax = config.Bind("Phase 1 Special", "Special Minimum Additional Times To Fire", 1, "How many additional times rings are fired off Providence current HP.");
            P1SpecialDamage = config.Bind("Phase 1 Special", "Special Damage", 9f, "Damage of special's overlap attack.");
            P1SpecialForce = config.Bind("Phase 1 Special", "Special Force", 300f, "Special's up force that is applied if attack landed.");
            P1SpecialHealthModifiersMax = config.Bind("Phase 1 Special", "Special Modifiers Minimal HP", 0.3f, "Health's proportion at which maximum additional utility modifiers are applied.");
            P1SpecialHealthModifiersMin = config.Bind("Phase 1 Special", "Special Modifiers Maximum HP", 0.8f, "Health's proportion at which minimum additional utility modifiers are applied.");
            P1SpecialCooldown = config.Bind("Phase 1 Special", "Special Cooldown", 60f, "Special skill cooldown.");

            P1SkullsDamage = config.Bind("Phase 1 Skulls", "Skulls Damage", 5f, "Damage of normal skull attack.");
            P1SkullsFireFrequency = config.Bind("Phase 1 Skulls", "Skulls Fire Frequency", 0.3f, "How frequently skulls are fired.");
            P1SkullsProjectileCount = config.Bind("Phase 1 Skulls", "Skulls Count", 10, "Base number of skulls spawned by the attack.");
            P1SkullsProjectileCountPerPlayer = config.Bind("Phase 1 Skulls", "Additional Skulls Count Per Player", 5, "Additional skulls spawned by the attack per player (above 1).");
            P1SkullsProjectileSpeed = config.Bind("Phase 1 Skulls", "Skulls Projectile Speed", 50f, "Skulls projectile speed");
            P1SkullsMaxDistance = config.Bind("Phase 1 Skulls", "Skulls Max Distance Spawn From Target", 15f, "How far from the target will projectile target spawn. It will spawn within these limits on all axis.");
            P1SkullsCanBeRed = config.Bind("Phase 1 Skulls", "Skulls Can Be Red", false, "Projectile can be red, red projectiles deal non-lethal 99999 damage.");
            P1SkullsCooldown = config.Bind("Phase 1 Skulls", "Skulls Cooldown", 45f, "Skulls skill cooldown. Skulls are used on health thresholds, so cooldown is only a part of when it is used.");
        }
    }
}
