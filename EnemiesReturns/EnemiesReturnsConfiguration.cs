using BepInEx.Configuration;
using UnityEngine;

namespace EnemiesReturns
{
    public static class EnemiesReturnsConfiguration
    {
        public static ConfigEntry<float> DebugWalkSpeedValue;

        public static ConfigEntry<float> testconfig;

        public struct Spitter
        {
            public static ConfigEntry<bool> Enabled;
            public static ConfigEntry<int> DirectorCost;
            public static ConfigEntry<int> SelectionWeight;
            public static ConfigEntry<int> MinimumStageCompletion;

            public static ConfigEntry<float> BaseMaxHealth;
            public static ConfigEntry<float> BaseMoveSpeed;
            public static ConfigEntry<float> BaseJumpPower;
            public static ConfigEntry<float> BaseDamage;
            public static ConfigEntry<float> BaseArmor;
            public static ConfigEntry<float> LevelMaxHealth;
            public static ConfigEntry<float> LevelDamage;
            public static ConfigEntry<float> LevelArmor;

            public static ConfigEntry<float> BiteCooldown;
            public static ConfigEntry<float> BiteDamageModifier;
            public static ConfigEntry<float> BiteDamageForce;

            public static ConfigEntry<float> ChargedProjectileCooldown;
            public static ConfigEntry<float> ChargedProjectileDamage;
            public static ConfigEntry<float> ChargedProjectileForce;
            public static ConfigEntry<float> ChargedProjectileLargeDoTZoneDamage;
            public static ConfigEntry<float> ChargedProjectileLargeDoTZoneScale;
            public static ConfigEntry<float> ChargedProjectileFlyTime;
            public static ConfigEntry<float> ChargedProjectileSmallDoTZoneDamage;
            public static ConfigEntry<float> ChargedProjectileSmallDoTZoneScale;

            public static ConfigEntry<KeyCode> EmoteKey;
        }

        public struct Colossus
        {
            public static ConfigEntry<bool> Enabled;
            public static ConfigEntry<int> DirectorCost;
            public static ConfigEntry<int> SelectionWeight;
            public static ConfigEntry<int> MinimumStageCompletion;

            public static ConfigEntry<float> BaseMaxHealth;
            public static ConfigEntry<float> BaseMoveSpeed;
            public static ConfigEntry<float> BaseJumpPower;
            public static ConfigEntry<float> BaseDamage;
            public static ConfigEntry<float> BaseArmor;
            public static ConfigEntry<float> LevelMaxHealth;
            public static ConfigEntry<float> LevelDamage;
            public static ConfigEntry<float> LevelArmor;

            public static ConfigEntry<bool> DestroyModelOnDeath;

            public static ConfigEntry<float> FootstepShockwaveDistance;
            public static ConfigEntry<float> FootstepShockwaveForce;

            public static ConfigEntry<float> StompCooldown;
            public static ConfigEntry<float> StompOverlapAttackDamage;
            public static ConfigEntry<float> StompOverlapAttackForce;
            public static ConfigEntry<float> StompProjectileDamage;
            public static ConfigEntry<float> StompProjectileForce;
            public static ConfigEntry<float> StompProjectileSpeed;
            public static ConfigEntry<int> StompProjectileCount;
            public static ConfigEntry<float> StompProjectileScale;
            public static ConfigEntry<float> StompProjectileLifetime;

            public static ConfigEntry<float> RockClapCooldown;
            public static ConfigEntry<float> RockClapProjectileDamage;
            public static ConfigEntry<float> RockClapProjectileForce;
            public static ConfigEntry<float> RockClapProjectileSpeed;
            public static ConfigEntry<float> RockClapProjectileSpeedDelta;
            public static ConfigEntry<float> RockClapProjectileBlastRadius;
            public static ConfigEntry<float> RockClapProjectileDistanceFraction;
            public static ConfigEntry<float> RockClapProjectileDistanceFractionDelta;
            public static ConfigEntry<float> RockClapProjectileSpawnDistance;
            public static ConfigEntry<int> RockClapProjectileCount;
            public static ConfigEntry<float> RockClapDamage;
            public static ConfigEntry<float> RockClapForce;
            public static ConfigEntry<float> RockClapRadius;
            public static ConfigEntry<float> RockClapHomingSpeed;
            public static ConfigEntry<float> RockClapHomingRange;
            public static ConfigEntry<bool> RockClapPostLoopSpawns;

            public static ConfigEntry<float> LaserBarrageCooldown;
            public static ConfigEntry<float> LaserBarrageDuration;
            public static ConfigEntry<float> LaserBarrageFrequency;
            public static ConfigEntry<float> LaserBarrageDamage;
            public static ConfigEntry<float> LaserBarrageProjectileSpeed;
            public static ConfigEntry<float> LaserBarrageForce;
            public static ConfigEntry<float> LaserBarrageSpread;
            public static ConfigEntry<float> LaserBarrageHeadPitch;
            public static ConfigEntry<int> LaserBarrageProjectileCount;
            public static ConfigEntry<float> LaserBarrageExplosionRadius;
            public static ConfigEntry<float> LaserBarrageExplosionDamage;
            public static ConfigEntry<float> LaserBarrageExplosionDelay;

            public static ConfigEntry<bool> ItemEnabled;
            public static ConfigEntry<float> KnurlDamage;
            public static ConfigEntry<float> KnurlDamagePerStack;
            public static ConfigEntry<float> KnurlProcChance;
            public static ConfigEntry<float> KnurlProcCoefficient;
            public static ConfigEntry<float> KnurlForce;

            public static ConfigEntry<KeyCode> EmoteKey;

            //public static ConfigEntry<int> KnurlGolemAllyDamageModifier;
            //public static ConfigEntry<int> KnurlGolemAllyDamageModifierPerStack;
            //public static ConfigEntry<int> KnurlGolemAllyHealthModifier;
            //public static ConfigEntry<int> KnurlGolemAllyHealthModifierPerStack;
            //public static ConfigEntry<int> KnurlGolemAllySpeedModifier;
            //public static ConfigEntry<int> KnurlGolemAllySpeedModifierPerStack;
            //public static ConfigEntry<int> KnurlArmor;
            //public static ConfigEntry<int> KnurlArmorPerStack;
        }

        public struct Ifrit
        {
            public static ConfigEntry<bool> Enabled;
            public static ConfigEntry<int> DirectorCost;
            public static ConfigEntry<int> SelectionWeight;
            public static ConfigEntry<int> MinimumStageCompletion;

            public static ConfigEntry<float> BaseMaxHealth;
            public static ConfigEntry<float> BaseMoveSpeed;
            public static ConfigEntry<float> BaseJumpPower;
            public static ConfigEntry<float> BaseDamage;
            public static ConfigEntry<float> BaseArmor;
            public static ConfigEntry<float> LevelMaxHealth;
            public static ConfigEntry<float> LevelDamage;
            public static ConfigEntry<float> LevelArmor;
            public static ConfigEntry<float> TurnSpeed;

            public static ConfigEntry<float> FlameChargeCooldown;
            public static ConfigEntry<float> FlameChargeDuration;
            public static ConfigEntry<float> FlameChargeSpeedCoefficient;
            public static ConfigEntry<float> FlameChargeDamage;
            public static ConfigEntry<float> FlameChargeForce;
            public static ConfigEntry<float> FlameChargeProcCoefficient;
            public static ConfigEntry<float> FlameChargeFlameDamage;
            public static ConfigEntry<float> FlameChargeFlameIgniteChance;
            public static ConfigEntry<float> FlameChargeFlameForce;
            public static ConfigEntry<float> FlameChargeFlameTickFrequency;
            public static ConfigEntry<float> FlameChargeFlameProcCoefficient;
            public static ConfigEntry<float> FlameChargeHeighCheck;

            public static ConfigEntry<float> HellzoneCooldown;
            public static ConfigEntry<float> HellzoneFireballDamage;
            public static ConfigEntry<float> HellzoneFireballProjectileSpeed;
            public static ConfigEntry<float> HellzoneFireballForce;
            public static ConfigEntry<float> HellzoneDoTZoneDamage;
            public static ConfigEntry<float> HellzoneDoTZoneLifetime;
            public static ConfigEntry<float> HellzoneRadius;
            public static ConfigEntry<int> HellzonePillarCount;
            public static ConfigEntry<float> HellzonePillarDelay;
            public static ConfigEntry<float> HellzonePillarDamage;
            public static ConfigEntry<float> HellzonePillarForce;

            public static ConfigEntry<float> PillarCooldown;
            public static ConfigEntry<float> PillarExplosionDamage;
            public static ConfigEntry<float> PillarExplosionChargeDuration;
            public static ConfigEntry<float> PillarExplosionRadius;
            public static ConfigEntry<bool> PillarExplosionIgnoesLoS;
            public static ConfigEntry<float> PillarBodyBaseMaxHealth;
            public static ConfigEntry<float> PillarBodyLevelMaxHealth;
            public static ConfigEntry<float> PillarExplosionForce;
            public static ConfigEntry<int> PillarMaxInstances;
            public static ConfigEntry<float> PillarMinSpawnDistance;
            public static ConfigEntry<float> PillarMaxSpawnDistance;

            public static ConfigEntry<bool> ItemEnabled;
            public static ConfigEntry<float> SpawnPillarOnChampionKillDamage;
            public static ConfigEntry<float> SpawnPillarOnChampionKillDamagePerStack;
            public static ConfigEntry<float> SpawnPillarOnChampionKillRadius;
            public static ConfigEntry<float> SpawnPillarOnChampionKillChargeTime;
            public static ConfigEntry<float> SpawnPillarOnChampionKillProcCoefficient;
            public static ConfigEntry<float> SpawnPillarOnChampionKillBodyBaseDamage;
            public static ConfigEntry<float> SpawnPillarOnChampionKillBodyLevelDamage;
            public static ConfigEntry<float> SpawnPillarOnChampionKillEliteChance;
        }

        public static void PopulateConfig(ConfigFile config)
        {
            DebugWalkSpeedValue = config.Bind("Debug", "walkSpeed value", 1f, "Value speed for walkSpeed animation. For debugging.");
            testconfig = config.Bind("test", "test", 5f, "test");

            #region Spitter

            Spitter.Enabled = config.Bind("Spitter Director", "Enable Spitter", true, "Enables Spitter.");
            Spitter.SelectionWeight = config.Bind("Spitter Director", "Selection Weight", 1, "Selection weight of Spitter.");
            Spitter.MinimumStageCompletion = config.Bind("Spitter Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning.");
            Spitter.DirectorCost = config.Bind("Spitter Director", "Director Cost", 30, "Director cost of Spitter.");

            Spitter.BaseMaxHealth = config.Bind("Spitter Character Stats", "Base Max Health", 300f, "Spitter's base health.");
            Spitter.BaseMoveSpeed = config.Bind("Spitter Character Stats", "Base Movement Speed", 7f, "Spitter's base movement speed.");
            Spitter.BaseJumpPower = config.Bind("Spitter Character Stats", "Base Jump Power", 20f, "Spitter's base jump power.");
            Spitter.BaseDamage = config.Bind("Spitter Character Stats", "Base Damage", 20f, "Spitter's base damage.");
            Spitter.BaseArmor = config.Bind("Spitter Character Stats", "Base Armor", 0f, "Spitter's base armor.");

            Spitter.LevelMaxHealth = config.Bind("Spitter Character Stats", "Health per Level", 90f, "Spitter's health increase per level.");
            Spitter.LevelDamage = config.Bind("Spitter Character Stats", "Damage per Level", 4f, "Spitter's damage increase per level.");
            Spitter.LevelArmor = config.Bind("Spitter Character Stats", "Armor per Level", 0f, "Spitter's armor increase per level.");

            Spitter.BiteCooldown = config.Bind("Spitter Bite", "Bite Cooldown", 2f, "Spitter's Bite cooldown.");
            Spitter.BiteDamageModifier = config.Bind("Spitter Bite", "Bite Damage", 1.5f, "Spitter's Bite damage.");
            Spitter.BiteDamageForce = config.Bind("Spitter Bite", "Bite Force", 200f, "Spitter's Bite force, by default equal to that of Lemurian.");

            Spitter.ChargedProjectileCooldown = config.Bind("Spitter Charged Spit", "Charged Spit Cooldown", 6f, "Charged Spit Cooldown");
            Spitter.ChargedProjectileDamage = config.Bind("Spitter Charged Spit", "Charged Spit Damage", 1.6f, "Spitter's Charged projectile damage.");
            Spitter.ChargedProjectileForce = config.Bind("Spitter Charged Spit", "Charged Spit Force", 0f, "Spitter's Charged projectile force.");
            Spitter.ChargedProjectileFlyTime = config.Bind("Spitter Charged Spit", "Charged Spit Fly Time", 0.75f, "Spitter's Charged Projectile fly time. The higher the value the bigger the arc and the slower the projectile will be. Don't set it too low otherwise projectile will fly pass targes and puddles will be somewhere in the back.");

            Spitter.ChargedProjectileLargeDoTZoneDamage = config.Bind("Spitter Charged Spit", "Charged Spit Large DoT Zone Damage", 0.15f, "Spitter's Charged Large DoT zone damage off projectile's damage.");
            Spitter.ChargedProjectileLargeDoTZoneScale = config.Bind("Spitter Charged Spit", "Charged Spit Large DoT Zone Size Scale", 0.55f, "Spitter's Charged Large DoT Zone size scale off Mini Mushrim's DoT zone (since it was used as basis). Also controls projectile's blast radius.");
            Spitter.ChargedProjectileSmallDoTZoneDamage = config.Bind("Spitter Charged Spit", "Charged Spit Small DoT Zone Damage", 0.15f, "Spitter's Charged Large DoT zone damage off projectile's damage.");
            Spitter.ChargedProjectileSmallDoTZoneScale = config.Bind("Spitter Charged Spit", "Charged Spit Small DoT Zone Scale", 0.3f, "Spitter's Charged Large DoT Zone scale off Mini Mushrim's DoT zone (since it was used as basis). Also controls projectile's blast radius.");

            Spitter.EmoteKey = config.Bind("Colossus Emotes", "Dance Emote", KeyCode.Alpha1, "Key used to Dance.");
            #endregion

            #region Colossus

            Colossus.Enabled = config.Bind("Colossus Director", "Enable Colossus", true, "Enables Colossus.");
            Colossus.SelectionWeight = config.Bind("Colossus Director", "Selection Weight", 1, "Selection weight of Colossus.");
            Colossus.MinimumStageCompletion = config.Bind("Colossus Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning.");
            Colossus.DirectorCost = config.Bind("Colossus Director", "Director Cost", 1150, "Director cost of Colossus.");

            Colossus.BaseMaxHealth = config.Bind("Colossus Character Stats", "Base Max Health", 7000f, "Colossus' base health.");
            Colossus.BaseMoveSpeed = config.Bind("Colossus Character Stats", "Base Movement Speed", 8f, "Colossus' base movement speed.");
            Colossus.BaseJumpPower = config.Bind("Colossus Character Stats", "Base Jump Power", 10f, "Colossus' base jump power.");
            Colossus.BaseDamage = config.Bind("Colossus Character Stats", "Base Damage", 40f, "Colossus' base damage.");
            Colossus.BaseArmor = config.Bind("Colossus Character Stats", "Base Armor", 35f, "Colossus' base armor.");

            Colossus.LevelMaxHealth = config.Bind("Colossus Character Stats", "Health per Level", 2100f, "Colossus' health increase per level.");
            Colossus.LevelDamage = config.Bind("Colossus Character Stats", "Damage per Level", 8f, "Colossus' damage increase per level.");
            Colossus.LevelArmor = config.Bind("Colossus Character Stats", "Armor per Level", 0f, "Colossus' armor increase per level.");

            Colossus.DestroyModelOnDeath = config.Bind("Colossus Model", "Destroy Model On Death", false, "Destroys model 5 seconds after Colossus' death. Use to remove a giant slab from the battlefield that might block stuff.");

            Colossus.FootstepShockwaveDistance = config.Bind("Colossus Footstep Shockwave", "Shockwave Distance", 9f, "Colossus's footstep shockwave distance.");
            Colossus.FootstepShockwaveForce = config.Bind("Colossus Footstep Shockwave", "Shockwave Force", 1800f, "Colossus's footstep shockwave force.");

            Colossus.StompCooldown = config.Bind("Colossus Stomp", "Stomp Cooldown", 10f, "Colossus' Stomp cooldown.");
            Colossus.StompOverlapAttackDamage = config.Bind("Colossus Stomp", "Stomp Overlap Damage", 2.5f, "Colossus' Stomp Overlap (aka stomp itself) damage.");
            Colossus.StompOverlapAttackForce = config.Bind("Colossus Stomp", "Stomp Overlap Force", 6000f, "Colossus' Stomp Overlap (aka stomp itself) force.");
            Colossus.StompProjectileDamage = config.Bind("Colossus Stomp", "Stomp Projectile Damage", 1.5f, "Colossus' Stomp Projectile damage.");
            Colossus.StompProjectileForce = config.Bind("Colossus Stomp", "Stomp Projectile Force", -2500f, "Colossus' Stomp Projectile force. Default number is negative, that means it pulls towards Colossus.");
            Colossus.StompProjectileSpeed = config.Bind("Colossus Stomp", "Stomp Projectile Speed", 60f, "Colossus' Stomp Projectile speed.");
            Colossus.StompProjectileCount = config.Bind("Colossus Stomp", "Stomp Projectile Count", 16, "Colossus' Stomp Projectile count.");
            Colossus.StompProjectileScale = config.Bind("Colossus Stomp", "Stomp Projectile Scale", 2f, "Colossus' Stomp Projectile scale.");
            Colossus.StompProjectileLifetime = config.Bind("Colossus Stomp", "Stomp Projectile Lifetime", 2f, "Colossus' Stomp Projectile lifetime.");

            Colossus.RockClapCooldown = config.Bind("Colossus Rock Clap", "Rock Clap Cooldown", 15f, "Colossus' Rock Clap cooldown.");
            Colossus.RockClapProjectileDamage = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Damage", 1.25f, "Colossus' Rock Clap projectile damage.");
            Colossus.RockClapProjectileForce = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Force", 3000f, "Colossus' Rock Clap projectile force.");
            Colossus.RockClapProjectileSpeed = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Speed", 50f, "Colossus' Rock Clap projectile speed.");
            Colossus.RockClapProjectileSpeedDelta = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Speed Delta", 5f, "Colossus' Rock Clap projectile speed delta (speed = Random(speed - delta, speed + delta)).");
            Colossus.RockClapProjectileBlastRadius = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Blast Radius", 5f, "Colossus' Rock Clap projectile blast radius.");
            Colossus.RockClapProjectileDistanceFraction = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Distance Fraction", 0.8f, "Basically determines angle at which rocks fly upwards. Uses colossus' position and rock initial position and takes a distance between them at this fraction.");
            Colossus.RockClapProjectileDistanceFractionDelta = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Distance Fraction Delta", 0.1f, "Projectile distance delta. See Projectile distance for explanation and see Speed delta for the formula.");
            Colossus.RockClapProjectileSpawnDistance = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Spawn Distance", 15f, "Colossus' Rock Clap projectile distance from body. Basically controls how far rocks spawn from the center of the body.");
            Colossus.RockClapProjectileCount = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Count", 20, "Colossus' Rock Clap projectile count.");
            Colossus.RockClapDamage = config.Bind("Colossus Rock Clap", "Rock Clap Damage", 2.5f, "Colossus' Rock Clap damage.");
            Colossus.RockClapForce = config.Bind("Colossus Rock Clap", "Rock Clap Force", 6000f, "Colossus' Rock Clap force.");
            Colossus.RockClapRadius = config.Bind("Colossus Rock Clap", "Rock Clap Radius", 16f, "Colossus' Rock Clap radius.");
            Colossus.RockClapHomingSpeed = config.Bind("Colossus Rock Clap", "Rock Clap Homing Speed", 15f, "Colossus' Rock Clap homing speed. Rocks home onto target by x and z axis (basically parallel to the ground, without homing up or down).");
            Colossus.RockClapHomingRange = config.Bind("Colossus Rock Clap", "Rock Clap Homing Range", 100f, "Colossus' Rock Clap homing range. How far rocks look for a targer.");
            Colossus.RockClapPostLoopSpawns = config.Bind("Colossus Rock Clap", "Rock Clap Post Loop Spawns", false, "Colossus' Rock Clap spawns enemies post loop, similar to how he does it in RoR1.");

            Colossus.LaserBarrageCooldown = config.Bind("Colossus Laser Barrage", "Laser Barrage Cooldown", 45f, "Colossus' Laser Barrage cooldown.");
            Colossus.LaserBarrageDamage = config.Bind("Colossus Laser Barrage", "Laser Barrage Damage", 0.5f, "Colossus' Laser Barrage damage.");
            Colossus.LaserBarrageDuration = config.Bind("Colossus Laser Barrage", "Laser Barrage Duration", 5f, "Colossus' Laser Barrage duration.");
            Colossus.LaserBarrageProjectileSpeed = config.Bind("Colossus Laser Barrage", "Laser Barrage Projectile Speed", 50f, "Colossus' Laser Barrage projectile speed.");
            Colossus.LaserBarrageFrequency = config.Bind("Colossus Laser Barrage", "Laser Barrage Fire Frequency", 0.2f, "Colossus' Laser Barrage fire frequency.");
            Colossus.LaserBarrageSpread = config.Bind("Colossus Laser Barrage", "Laser Barrage Spread", 0.15f, "Colossus' Laser Barrage spread. The lower the value, more tight each cluster of shots will be.");
            Colossus.LaserBarrageHeadPitch = config.Bind("Colossus Laser Barrage", "Laser Barrage Head Pitch", 0.05f, "Colossus' Laser Barrage head pitch. 1 is all the way up, 0 is all the way down.");
            Colossus.LaserBarrageForce = config.Bind("Colossus Laser Barrage", "Laser Barrage Force", 0f, "Colossus' Laser Barrage force.");
            Colossus.LaserBarrageProjectileCount = config.Bind("Colossus Laser Barrage", "Laser Barrage Projectiles per Shot", 8, "Colossus' Laser Barrage projectiles per shot count.");
            Colossus.LaserBarrageExplosionRadius = config.Bind("Colossus Laser Barrage", "Laser Barrage Explosion Radius", 10f, "Colossus' Laser Barrage explosion radius.");
            Colossus.LaserBarrageExplosionDamage = config.Bind("Colossus Laser Barrage", "Laser Barrage Explosion Damage", 1.25f, "Colossus' Laser Barrage explosion damage, fraction of projectile damage.");
            Colossus.LaserBarrageExplosionDelay = config.Bind("Colossus Laser Barrage", "Laser Barrage Explosion Delay", 0.5f, "Colossus' Laser Barrage explosion delay after hitting the ground.");

            Colossus.ItemEnabled = config.Bind("Colossal Fist", "Enable Colossal Fist", true, "Enables Colossal Fist to drop from Colossus and appear in printers.");
            Colossus.KnurlDamage = config.Bind("Colossal Fist", "Colossal Fist Damage", 5f, "Colossal Fist' damage");
            Colossus.KnurlDamagePerStack = config.Bind("Colossal Fist", "Colossal Fist Damage Per Stack", 5f, "Colossal Fist' damage per stack");
            Colossus.KnurlProcCoefficient = config.Bind("Colossal Fist", "Colossal Fist Proc Coefficient", 0f, "Colossal Fist proc coefficient.");
            Colossus.KnurlProcChance = config.Bind("Colossal Fist", "Colossal Fist Proc Chance", 8f, "Colossal Fist proc chance.");
            Colossus.KnurlForce = config.Bind("Colossal Fist", "Colossal Fist Force", 0f, "Colossal Fist force.");

            Colossus.EmoteKey = config.Bind("Colossus Emotes", "Dance Emote", KeyCode.Alpha1, "Key used to Dance.");
            //Colossus.KnurlArmor = config.Bind("Colossal Knurl", "Colossal Knurl Armor", 20, "How much armor Colossal Knurl grants.");
            //Colossus.KnurlArmorPerStack = config.Bind("Colossal Knurl", "Colossal Knurl Armor Per Stack", 20, "How much armor Colossal Knurl grants per stack.");
            //Colossus.KnurlGolemAllyDamageModifier = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Damage Modifier", 30, "Additiona damage modifier for ally golem, 10% each.");
            //Colossus.KnurlGolemAllyDamageModifierPerStack = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Damage Modifier Per Stack", 30, "Additiona damage modifier for ally golem per stack, 10% each.");
            //Colossus.KnurlGolemAllyHealthModifier = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Health Modifier", 10, "Additiona health modifier for ally golem, 10% each.");
            //Colossus.KnurlGolemAllyHealthModifierPerStack = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Health Modifier Per Stack", 10, "Additiona health modifier for ally golem per stack, 10% each.");
            //Colossus.KnurlGolemAllySpeedModifier = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Movement Speed Modifier", 5, "Additiona movement speed modifier for ally golem, 14% each (basically one hoof).");
            //Colossus.KnurlGolemAllySpeedModifierPerStack = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Movement Speed Modifier Per Stack", 5, "Additiona movement speed modifier for ally golem per stack, 14% each (basically one hoof).");
            #endregion

            #region Ifrit
            Ifrit.Enabled = config.Bind("Ifrit Director", "Enable Ifrit", true, "Enables Ifrit.");
            Ifrit.SelectionWeight = config.Bind("Ifrit Director", "Selection Weight", 1, "Selection weight of Ifrit.");
            Ifrit.MinimumStageCompletion = config.Bind("Ifrit Director", "Minimum Stage Completion", 2, "Minimum stages players need to complete before monster starts spawning.");
            Ifrit.DirectorCost = config.Bind("Ifrit Director", "Director Cost", 800, "Director cost of Ifrit.");

            Ifrit.BaseMaxHealth = config.Bind("Ifrit Character Stats", "Base Max Health", 2800f, "Ifrit' base health.");
            Ifrit.BaseMoveSpeed = config.Bind("Ifrit Character Stats", "Base Movement Speed", 13f, "Ifrit' base movement speed.");
            Ifrit.BaseJumpPower = config.Bind("Ifrit Character Stats", "Base Jump Power", 30f, "Ifrit' base jump power.");
            Ifrit.BaseDamage = config.Bind("Ifrit Character Stats", "Base Damage", 16f, "Ifrit' base damage.");
            Ifrit.BaseArmor = config.Bind("Ifrit Character Stats", "Base Armor", 20f, "Ifrit' base armor.");
            Ifrit.TurnSpeed = config.Bind("Ifrit Character Stats", "Turn Speed", 300f, "Ifrit's turn speed.");

            Ifrit.LevelMaxHealth = config.Bind("Ifrit Character Stats", "Health per Level", 840f, "Ifrit' health increase per level.");
            Ifrit.LevelDamage = config.Bind("Ifrit Character Stats", "Damage per Level", 3.2f, "Ifrit' damage increase per level.");
            Ifrit.LevelArmor = config.Bind("Ifrit Character Stats", "Armor per Level", 0f, "Ifrit' armor increase per level.");

            Ifrit.FlameChargeCooldown = config.Bind("Ifrit Flame Charge", "Flame Charge Cooldown", 15f, "Ifrit's Flame Charge cooldown.");
            Ifrit.FlameChargeDuration = config.Bind("Ifrit Flame Charge", "Flame Charge Duration", 5f, "Ifrit's Flame Charge duration.");
            Ifrit.FlameChargeSpeedCoefficient = config.Bind("Ifrit Flame Charge", "Flame Charge Movement Speed Multiplier", 2f, "Ifrit's Flame Charge speed multiplier.");
            Ifrit.FlameChargeDamage = config.Bind("Ifrit Flame Charge", "Flame Charge Body Damage", 2.5f, "Ifrit's Flame Charge body contact damage.");
            Ifrit.FlameChargeForce = config.Bind("Ifrit Flame Charge", "Flame Charge Body Force", 5000f, "Ifrit's Flame Charge body contact force.");
            Ifrit.FlameChargeProcCoefficient = config.Bind("Ifrit Flame Charge", "Flame Charge Body Proc Coefficient", 1f, "Ifrit's Flame Charge body contact proc coefficient.");
            Ifrit.FlameChargeFlameDamage = config.Bind("Ifrit Flame Charge", "Flame Charge Flame Damage", 0.4f, "Ifrit's Flame Charge flame damage.");
            Ifrit.FlameChargeFlameIgniteChance = config.Bind("Ifrit Flame Charge", "Flame Charge Flame Ignite Chance", 100f, "Ifrit's Flame Charge flame ignite chance.");
            Ifrit.FlameChargeFlameForce = config.Bind("Ifrit Flame Charge", "Flame Charge Flame Force", 0f, "Ifrit's Flame Charge flame force.");
            Ifrit.FlameChargeFlameTickFrequency = config.Bind("Ifrit Flame Charge", "Flame Charge Flame Tick Frequence", 8f, "How many times per second Ifrit's Flame Charge flame deal damage.");
            Ifrit.FlameChargeFlameProcCoefficient = config.Bind("Ifrit Flame Charge", "Flame Charge Flame Proc Coefficient", 0.2f, "Ifrit's Flame Charge flame proc coefficient.");
            Ifrit.FlameChargeHeighCheck = config.Bind("Ifrit Flame Charge", "Flame Charge Height Check", 50f, "Checks for falls in front of Ifrit and stops his so he wouldn't yeet himself off cliffs. Set it above 1000 to basically disable the functionality.");

            Ifrit.HellzoneCooldown = config.Bind("Ifrit Hellzone", "Hellzone Cooldown", 10f, "Ifrit's Hellzone cooldown.");
            Ifrit.HellzoneRadius = config.Bind("Ifrit Hellzone", "Hellzone Radius", 9f, "Ifrit's Hellzone radius.");
            Ifrit.HellzoneFireballDamage = config.Bind("Ifrit Hellzone", "Hellzone Fireball Damage", 2.5f, "Ifrit's Hellzone initial fireball projectile damage.");
            Ifrit.HellzoneFireballProjectileSpeed = config.Bind("Ifrit Hellzone", "Hellzone Fireball Projectile Speed", 50f, "Ifrit's Hellzone fireball projectile speed.");
            Ifrit.HellzoneFireballForce = config.Bind("Ifrit Hellzone", "Hellzone Fireball Projectile Force", 0f, "Ifrit's Hellzone fireball projectile force.");
            Ifrit.HellzonePillarCount = config.Bind("Ifrit Hellzone", "Hellzone Secondary Projectile Count", 3, "Ifrit's Hellzone number of secondary explosions.");
            Ifrit.HellzonePillarDelay = config.Bind("Ifrit Hellzone", "Hellzone Secondary Projectile Delay", 0.5f, "Ifrit's Hellzone delay between each secondary explosion.");
            Ifrit.HellzoneDoTZoneLifetime = config.Bind("Ifrit Hellzone", "Hellzone DoT Zone Lifetime", 3f, "Ifrit's Hellzone DoT zone initial lifetime. Secondary projectile count multiplied by secondary projectile delay will be added to this value to get total dot zone lifetime.");
            Ifrit.HellzoneDoTZoneDamage = config.Bind("Ifrit Hellzone", "Hellzone DoT Zone Damage", 0.5f, "Ifrit's Hellzone DoT zone damage. Scales off Hellzone initial projectile damage.");
            Ifrit.HellzonePillarDamage = config.Bind("Ifrit Hellzone", "Hellzone Secondary Projectile Damage", 3f, "Ifrit's Hellzone secondary projectile damage. Scales of DoT Zone Damage.");
            Ifrit.HellzonePillarForce = config.Bind("Ifrit Hellzone", "Hellzone Secondary Projectile Force", 2400f, "Ifrit's Hellzone secondary projectile force.");

            Ifrit.PillarCooldown = config.Bind("Ifrit Pillar", "Summon Pillar Cooldown", 45f, "Ifrit's Summon Pillar cooldown.");
            Ifrit.PillarExplosionDamage = config.Bind("Ifrit Pillar", "Summon Pillar Damage", 10f, "Ifrit's Summon Pillar explosion damage.");
            Ifrit.PillarExplosionChargeDuration = config.Bind("Ifrit Pillar", "Summon Pillar Explosion Charge Time", 15f, "Ifrit's Summon Pillar explosion charge time.");
            Ifrit.PillarExplosionRadius = config.Bind("Ifrit Pillar", "Summon Pillar Explosion Radius", 120f, "Ifrit's Summon Pillar explosion radius.");
            Ifrit.PillarExplosionForce = config.Bind("Ifrit Pillar", "Summon Pillar Explosion Force", 3000f, "Ifrit's Summon Pillar explosion force.");
            Ifrit.PillarExplosionIgnoesLoS = config.Bind("Ifrit Pillar", "Summon Pillar Explosion Ignores LoS", false, "Ifrit's Summon Pillar explosion ignores line of sight.");
            Ifrit.PillarBodyBaseMaxHealth = config.Bind("Ifrit Pillar", "Summon Pillar Body Base Max Health", 585f, "Ifrit's Summon Pillar body base max health.");
            Ifrit.PillarBodyLevelMaxHealth = config.Bind("Ifrit Pillar", "Summon Pillar Body Per Level Max Health", 176f, "Ifrit's Summon Pillar body per level max health.");
            Ifrit.PillarMaxInstances = config.Bind("Ifrit Pillar", "Summon Pillar Max Instances", 2, "Maximum instances of Ifrit's Pillar that can exist at the same time. This also controls how many pillar will be summoned on one skill use.");
            Ifrit.PillarMinSpawnDistance = config.Bind("Ifrit Pillar", "Summon Pillar Min Spawn Distance", 50f, "Ifrit's Summon Pillar minimum distance for pillar spawning.");
            Ifrit.PillarMaxSpawnDistance = config.Bind("Ifrit Pillar", "Summon Pillar Max Spawn Distance", 80f, "Ifrit's Summon Pillar maximum distance for pillar spawning.");

            Ifrit.ItemEnabled = config.Bind("Infernal Lantern", "Enable Infernal Lantern", true, "Enables Infernal Lantern to drop from Ifrit and appear in printers.");
            Ifrit.SpawnPillarOnChampionKillDamage = config.Bind("Infernal Lantern", "Infernal Lantern Damage", 10f, "Infernal Lantern explosion damage.");
            Ifrit.SpawnPillarOnChampionKillDamagePerStack = config.Bind("Infernal Lantern", "Infernal Lantern Damage Per Stack", 10f, "Infernal Lantern explosion damage per stack.");
            Ifrit.SpawnPillarOnChampionKillRadius = config.Bind("Infernal Lantern", "Infernal Lantern Radius", 80f, "Infernal Lantern explosion radius.");
            Ifrit.SpawnPillarOnChampionKillChargeTime = config.Bind("Infernal Lantern", "Infernal Lantern Charge Time", 5f, "Infernal Lantern explosion charge time.");
            Ifrit.SpawnPillarOnChampionKillProcCoefficient = config.Bind("Infernal Lantern", "Infernal Lantern Proc Coefficient", 0f, "Infernal Lantern explosion proc coefficient.");
            Ifrit.SpawnPillarOnChampionKillBodyBaseDamage = config.Bind("Infernal Lantern", "Infernal Lantern Base Damage", 12f, "Infernal Lantern pillar base damage. By default equal to most survivors.");
            Ifrit.SpawnPillarOnChampionKillBodyLevelDamage = config.Bind("Infernal Lantern", "Infernal Lantern Base Damage", 2.4f, "Infernal Lantern pillar damage per level. By default equal to most survivors.");
            Ifrit.SpawnPillarOnChampionKillEliteChance = config.Bind("Infernal Lantern", "Infernal Lantern Elite Kill Spawn Chance", 20f, "Infernal Lantern chance to spawn on elite kill.");
            #endregion
        }
    }
}
