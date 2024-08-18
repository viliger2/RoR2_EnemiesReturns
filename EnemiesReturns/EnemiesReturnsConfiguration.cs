using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns
{
    public static class EnemiesReturnsConfiguration
    {
        public static ConfigEntry<float> DebugWalkSpeedValue;

        public static ConfigEntry<float> testconfig;

        public struct Spitter
        {
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

            public static ConfigEntry<float> BiteDamageModifier;
            public static ConfigEntry<float> BiteDamageForce;
            public static ConfigEntry<float> BiteRadius;

            [Obsolete]
            public static ConfigEntry<float> NormalSpitDamage;
            [Obsolete]
            public static ConfigEntry<float> NormalSpitSpeed;
            [Obsolete]
            public static ConfigEntry<float> NormalSpitForce;

            public static ConfigEntry<float> ChargedProjectileCooldown;
            public static ConfigEntry<float> ChargedProjectileDamage;
            public static ConfigEntry<float> ChargedProjectileForce;
            public static ConfigEntry<float> ChargedProjectileLargeDoTZoneDamage;
            public static ConfigEntry<float> ChargedProjectileLargeDoTZoneScale;
            public static ConfigEntry<float> ChargedProjectileFlyTime;
            public static ConfigEntry<float> ChargedProjectileSmallDoTZoneDamage;
            public static ConfigEntry<float> ChargedProjectileSmallDoTZoneScale;
        }

        public struct Colossus
        {
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

            public static ConfigEntry<float> KnurlDamage;
            public static ConfigEntry<float> KnurlDamagePerStack;
            public static ConfigEntry<float> KnurlProcChance;
            public static ConfigEntry<float> KnurlProcCoefficient;
            public static ConfigEntry<float> KnurlForce;

            //public static ConfigEntry<int> KnurlGolemAllyDamageModifier;
            //public static ConfigEntry<int> KnurlGolemAllyDamageModifierPerStack;
            //public static ConfigEntry<int> KnurlGolemAllyHealthModifier;
            //public static ConfigEntry<int> KnurlGolemAllyHealthModifierPerStack;
            //public static ConfigEntry<int> KnurlGolemAllySpeedModifier;
            //public static ConfigEntry<int> KnurlGolemAllySpeedModifierPerStack;
            //public static ConfigEntry<int> KnurlArmor;
            //public static ConfigEntry<int> KnurlArmorPerStack;

            //public static ConfigEntry<float> HeadLaserDuration;
            //public static ConfigEntry<float> HeadLaserFireFrequency;
            //public static ConfigEntry<float> HeadLaserDamage;
            //public static ConfigEntry<float> HeadLaserForce;
            //public static ConfigEntry<float> HeadLaserRadius;
            //public static ConfigEntry<int> HeadLaserTurnCount;
            //public static ConfigEntry<float> HeadLaserPitchStart;
            //public static ConfigEntry<float> HeadLaserPitchStep;
        }

        public static void PopulateConfig(ConfigFile config) 
        {
            DebugWalkSpeedValue = config.Bind("Debug", "walkSpeed value", 1f, "Value speed for walkSpeed animation. For debugging.");
            testconfig = config.Bind("test", "test", 5f, "test");

            #region Spitter

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

            Spitter.BiteDamageModifier = config.Bind("Spitter Bite", "Bite Damage", 1.5f, "Spitter's Bite damage.");
            Spitter.BiteDamageForce = config.Bind("Spitter Bite", "Bite Force", 200f, "Spitter's Bite force, by default equal to that of Lemurian.");
            Spitter.BiteRadius = config.Bind("Spitter Bite", "Bite Radius", 0f, "Spitter's Bite radius.");

            //Spitter.NormalSpitDamage = config.Bind("Spitter Normal Spit", "Spit Damage", 2f, "Spitter's Normal Spit projectile damage.");
            //Spitter.NormalSpitForce = config.Bind("Spitter Normal Spit", "Spit Force", 1000f, "Spitter's Normal Spit force.");
            //Spitter.NormalSpitSpeed = config.Bind("Spitter Normal Spit", "Spit Speed", 50f, "Spitter's Normal Spit projectile speed.");

            Spitter.ChargedProjectileCooldown = config.Bind("Spitter Charged Spit", "Charged Spit Cooldown", 6f, "Charged Spit Cooldown");
            Spitter.ChargedProjectileDamage = config.Bind("Spitter Charged Spit", "Charged Spit Damage", 1.6f, "Spitter's Charged projectile damage.");
            Spitter.ChargedProjectileForce = config.Bind("Spitter Charged Spit", "Charged Spit Force", 0f, "Spitter's Charged projectile force.");
            Spitter.ChargedProjectileFlyTime = config.Bind("Spitter Charged Spit", "Charged Spit Fly Time", 0.75f, "Spitter's Charged Projectile fly time. The higher the value the bigger the arc and the slower the projectile will be. Don't set it too low otherwise projectile will fly pass targes and puddles will be somewhere in the back.");

            Spitter.ChargedProjectileLargeDoTZoneDamage = config.Bind("Spitter Charged Spit", "Charged Spit Large DoT Zone Damage", 0.15f, "Spitter's Charged Large DoT zone damage off projectile's damage.");
            Spitter.ChargedProjectileLargeDoTZoneScale = config.Bind("Spitter Charged Spit", "Charged Spit Large DoT Zone Size Scale", 0.55f, "Spitter's Charged Large DoT Zone size scale off Mini Mushrim's DoT zone (since it was used as basis). Also controls projectile's blast radius.");
            Spitter.ChargedProjectileSmallDoTZoneDamage = config.Bind("Spitter Charged Spit", "Charged Spit Small DoT Zone Damage", 0.15f, "Spitter's Charged Large DoT zone damage off projectile's damage.");
            Spitter.ChargedProjectileSmallDoTZoneScale = config.Bind("Spitter Charged Spit", "Charged Spit Small DoT Zone Scale", 0.3f, "Spitter's Charged Large DoT Zone scale off Mini Mushrim's DoT zone (since it was used as basis). Also controls projectile's blast radius.");

            #endregion

            #region Colossus

            Colossus.SelectionWeight = config.Bind("Colossus Director", "Selection Weight", 1, "Selection weight of Colossus.");
            Colossus.MinimumStageCompletion = config.Bind("Colossus Director", "Minimum Stage Completion", 3, "Minimum stages players need to complete before monster starts spawning.");
            Colossus.DirectorCost = config.Bind("Colossus Director", "Director Cost", 2000, "Director cost of Colossus.");

            Colossus.BaseMaxHealth = config.Bind("Colossus Character Stats", "Base Max Health", 7000f, "Colossus' base health.");
            Colossus.BaseMoveSpeed = config.Bind("Colossus Character Stats", "Base Movement Speed", 8f, "Colossus' base movement speed.");
            Colossus.BaseJumpPower = config.Bind("Colossus Character Stats", "Base Jump Power", 10f, "Colossus' base jump power.");
            Colossus.BaseDamage = config.Bind("Colossus Character Stats", "Base Damage", 40f, "Colossus' base damage.");
            Colossus.BaseArmor = config.Bind("Colossus Character Stats", "Base Armor", 35f, "Colossus' base armor.");

            Colossus.LevelMaxHealth = config.Bind("Colossus Character Stats", "Health per Level", 2100f, "Colossus' health increase per level.");
            Colossus.LevelDamage = config.Bind("Colossus Character Stats", "Damage per Level", 8f, "Colossus' damage increase per level.");
            Colossus.LevelArmor = config.Bind("Colossus Character Stats", "Armor per Level", 0f, "Colossus' armor increase per level.");

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
            Colossus.RockClapProjectileForce = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Force", 3000f, "Colossus' Rock Clap projectile force."); // TODO: might be too much
            Colossus.RockClapProjectileSpeed = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Speed", 50f, "Colossus' Rock Clap projectile speed.");
            Colossus.RockClapProjectileSpeedDelta = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Speed Delta", 5f, "Colossus' Rock Clap projectile speed delta (speed = Random(speed - delta, speed + delta)).");
            Colossus.RockClapProjectileBlastRadius = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Blast Radius", 5f, "Colossus' Rock Clap projectile blast radius.");
            Colossus.RockClapProjectileDistanceFraction = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Distance Fraction", 0.8f, "Basically determines angle at which rocks fly upwards. Uses colossus' position and rock initial position and takes a distance between them at this fraction.");
            Colossus.RockClapProjectileDistanceFractionDelta = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Distance Fraction Delta", 0.1f, "Projectile distance delta. See Projectile distance for explanation and see Speed delta for the formula.");
            Colossus.RockClapProjectileSpawnDistance = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Spawn Distance", 15f, "Colossus' Rock Clap projectile distance from body. Basically controls how far rocks spawn from the center of the body.");
            Colossus.RockClapProjectileCount = config.Bind("Colossus Rock Clap", "Rock Clap Projectile Count", 20, "Colossus' Rock Clap projectile count.");
            Colossus.RockClapDamage = config.Bind("Colossus Rock Clap", "Rock Clap Damage", 2.5f, "Colossus' Rock Clap damage.");
            Colossus.RockClapForce = config.Bind("Colossus Rock Clap", "Rock Clap Force", 6000f, "Colossus' Rock Clap force."); // TODO: might be too much
            Colossus.RockClapRadius = config.Bind("Colossus Rock Clap", "Rock Clap Radius", 16f, "Colossus' Rock Clap radius.");
            Colossus.RockClapHomingSpeed = config.Bind("Colossus Rock Clap", "Rock Clap Homing Speed", 15f, "Colossus' Rock Clap homing speed. Rocks home onto target by x and z axis (basically parallel to the ground, without homing up or down).");
            Colossus.RockClapHomingRange = config.Bind("Colossus Rock Clap", "Rock Clap Homing Range", 100f, "Colossus' Rock Clap homing range. How far rocks look for a targer.");

            Colossus.LaserBarrageCooldown = config.Bind("Colossus Laser Barrage", "Laser Barrage Cooldown", 45f, "Colossus' Laser Barrage cooldown.");
            Colossus.LaserBarrageDamage = config.Bind("Colossus Laser Barrage", "Laser Barrage Damage", 0.5f, "Colossus' Laser Barrage damage.");
            Colossus.LaserBarrageDuration = config.Bind("Colossus Laser Barrage", "Laser Barrage Duration", 5f, "Colossus' Laser Barrage duration.");
            Colossus.LaserBarrageProjectileSpeed = config.Bind("Colossus Laser Barrage", "Laser Barrage Projectile Speed", 50f, "Colossus' Laser Barrage projectile speed.");
            Colossus.LaserBarrageFrequency = config.Bind("Colossus Laser Barrage", "Laser Barrage Fire Frequency", 0.2f, "Colossus' Laser Barrage fire frequency.");
            Colossus.LaserBarrageSpread = config.Bind("Colossus Laser Barrage", "Laser Barrage Spread", 0.18f, "Colossus' Laser Barrage spread. The lower the value, more tight each cluster of shots will be.");
            Colossus.LaserBarrageHeadPitch = config.Bind("Colossus Laser Barrage", "Laser Barrage Head Pitch", 0.75f, "Colossus' Laser Barrage head pitch. 1 is all the way up, 0 is all the way down.");
            Colossus.LaserBarrageForce = config.Bind("Colossus Laser Barrage", "Laser Barrage Force", 0f, "Colossus' Laser Barrage force.");
            Colossus.LaserBarrageProjectileCount = config.Bind("Colossus Laser Barrage", "Laser Barrage Projectiles per Shot", 8, "Colossus' Laser Barrage projectiles per shot count.");
            Colossus.LaserBarrageExplosionRadius = config.Bind("Colossus Laser Barrage", "Laser Barrage Explosion Radius", 5f, "Colossus' Laser Barrage explosion radius.");
            Colossus.LaserBarrageExplosionDamage = config.Bind("Colossus Laser Barrage", "Laser Barrage Explosion Damage", 1.25f, "Colossus' Laser Barrage explosion damage, fraction of projectile damage.");
            Colossus.LaserBarrageExplosionDelay = config.Bind("Colossus Laser Barrage", "Laser Barrage Explosion Delay", 0.5f, "Colossus' Laser Barrage explosion delay after hitting the ground.");

            Colossus.KnurlDamage = config.Bind("Colossal Fist", "Colossal Fist Damage", 8f, "Colossal Fist' damage");
            Colossus.KnurlDamagePerStack = config.Bind("Colossal Fist", "Colossal Fist Damage Per Stack", 8f, "Colossal Fist' damage per stack");
            Colossus.KnurlProcCoefficient = config.Bind("Colossal Fist", "Colossal Fist Proc Coefficient", 0f, "Colossal Fist proc coefficient.");
            Colossus.KnurlProcChance = config.Bind("Colossal Fist", "Colossal Fist Proc Chance", 5f, "Colossal Fist proc chance.");
            Colossus.KnurlForce = config.Bind("Colossal Fist", "Colossal Fist Force", 0f, "Colossal Fist force.");

            //Colossus.KnurlArmor = config.Bind("Colossal Knurl", "Colossal Knurl Armor", 20, "How much armor Colossal Knurl grants.");
            //Colossus.KnurlArmorPerStack = config.Bind("Colossal Knurl", "Colossal Knurl Armor Per Stack", 20, "How much armor Colossal Knurl grants per stack.");
            //Colossus.KnurlGolemAllyDamageModifier = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Damage Modifier", 30, "Additiona damage modifier for ally golem, 10% each.");
            //Colossus.KnurlGolemAllyDamageModifierPerStack = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Damage Modifier Per Stack", 30, "Additiona damage modifier for ally golem per stack, 10% each.");
            //Colossus.KnurlGolemAllyHealthModifier = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Health Modifier", 10, "Additiona health modifier for ally golem, 10% each.");
            //Colossus.KnurlGolemAllyHealthModifierPerStack = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Health Modifier Per Stack", 10, "Additiona health modifier for ally golem per stack, 10% each.");
            //Colossus.KnurlGolemAllySpeedModifier = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Movement Speed Modifier", 5, "Additiona movement speed modifier for ally golem, 14% each (basically one hoof).");
            //Colossus.KnurlGolemAllySpeedModifierPerStack = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Movement Speed Modifier Per Stack", 5, "Additiona movement speed modifier for ally golem per stack, 14% each (basically one hoof).");

            //Colossus.HeadLaserDuration = config.Bind("Colossus Head Laser", "Head Laser Duration", 25f, "Colossus' Head Laser duration. Only includes firing laser itself, pre and post states are not included.");
            //Colossus.HeadLaserFireFrequency = config.Bind("Colossus Head Laser", "Head Laser Fire Frequency", 0.06f, "How frequently Colossus' Head Laser fires. Has no effect on visuals.");
            //Colossus.HeadLaserDamage = config.Bind("Colossus Head Laser", "Head Laser Damage", 0.5f, "Colossus' Head Laser Damage");
            //Colossus.HeadLaserForce = config.Bind("Colossus Head Laser", "Head Laser Force", 0f, "Colossus' Head Laser force");
            //Colossus.HeadLaserRadius = config.Bind("Colossus Head Laser", "Head Laser Radius", 7.5f, "Colossus' Head Laser radius.");
            //Colossus.HeadLaserTurnCount = config.Bind("Colossus Head Laser", "Head Laser Head Turn Count", 3, "How many times Colossus turns its head left to right and back during Head Laser attack. Duration of each turn is (Head Laser Duration)/(Head Laser Head Turn Count).");
            //Colossus.HeadLaserPitchStart = config.Bind("Colossus Head Laser", "Head Laser Starting Pitch", 0.05f, "Determines starting pitch of Colossus' head. Values (including total value) above 1 will be limited to 1.");
            //Colossus.HeadLaserPitchStep = config.Bind("Colossus Head Laser", "Head Laser Head Pitch Step", 0.25f, "Determines how much higher Colossus' head gets after each turn. Final value is (Head Laser Starting Pitch)+(Head Laser Head Turn Count)*(Head Laser Head Pitch Step). Values (including total value) above 1 will be limited to 1.");

            #endregion
        }


    }
}
