using BepInEx.Configuration;
using R2API;
using UnityEngine;

namespace EnemiesReturns.Configuration
{
    public static class Colossus
    {
        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<int> DirectorCost;
        public static ConfigEntry<int> SelectionWeight;
        public static ConfigEntry<int> MinimumStageCompletion;

        public static ConfigEntry<string> DefaultStageList;
        public static ConfigEntry<string> SkyMeadowStageList;
        public static ConfigEntry<string> GrassyStageList;
        public static ConfigEntry<string> CastleStageList;
        public static ConfigEntry<string> SandyStageList;
        public static ConfigEntry<string> SnowyStageList;

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
        public static ConfigEntry<bool> AddToArtifactOfOrigin;

        //public static ConfigEntry<int> KnurlGolemAllyDamageModifier;
        //public static ConfigEntry<int> KnurlGolemAllyDamageModifierPerStack;
        //public static ConfigEntry<int> KnurlGolemAllyHealthModifier;
        //public static ConfigEntry<int> KnurlGolemAllyHealthModifierPerStack;
        //public static ConfigEntry<int> KnurlGolemAllySpeedModifier;
        //public static ConfigEntry<int> KnurlGolemAllySpeedModifierPerStack;
        //public static ConfigEntry<int> KnurlArmor;
        //public static ConfigEntry<int> KnurlArmorPerStack;

        public static void PopulateConfig(ConfigFile config)
        {
            Colossus.Enabled = config.Bind("Colossus Director", "Enable Colossus", true, "Enables Colossus.");
            Colossus.SelectionWeight = config.Bind("Colossus Director", "Selection Weight", 1, "Selection weight of Colossus.");
            Colossus.MinimumStageCompletion = config.Bind("Colossus Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning.");
            Colossus.DirectorCost = config.Bind("Colossus Director", "Director Cost", 1150, "Director cost of Colossus.");

            Colossus.DefaultStageList = config.Bind("Colossus Director", "Default Variant Stage List",
                string.Join(
                    ",",
                    ""
                ),
                "Stages that Default Colossus appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");
            Colossus.SkyMeadowStageList = config.Bind("Colossus Director", "Sky Meadow Variant Stage List",
                string.Join(
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SkyMeadow),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SkyMeadowSimulacrum)
                ),
                "Stages that Sky Meadow Colossus appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");
            Colossus.GrassyStageList = config.Bind("Colossus Director", "Grassy Variant Stage List",
                string.Join(
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.TitanicPlains),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.TitanicPlainsSimulacrum),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.ShatteredAbodes),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.DisturbedImpact),
                    "FBLScene"
                ),
                "Stages that Grassy Colossus appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");
            Colossus.CastleStageList = config.Bind("Colossus Director", "Castle Variant Stage List",
                string.Join(
                    ",",
                    "sm64_bbf_SM64_BBF"
                ),
                "Stages that Castle Colossus appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");
            Colossus.SandyStageList = config.Bind("Colossus Director", "Sandy Variant Stage List",
                string.Join(
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.AbandonedAqueduct),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.AbandonedAqueductSimulacrum)
                ),
                "Stages that Sandy Colossus appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");
            Colossus.SnowyStageList = config.Bind("Colossus Director", "Snowy Variant Stage List",
                string.Join(
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SiphonedForest),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.RallypointDelta),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.RallypointDeltaSimulacrum)
                ),
                "Stages that Snowy Colossus appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

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
            Colossus.KnurlForce = config.Bind("Colossal Fist", "Colossal Fist Force", 1000f, "Colossal Fist force.");

            Colossus.EmoteKey = config.Bind("Colossus Emotes", "Dance Emote", KeyCode.Alpha1, "Key used to Dance.");
            AddToArtifactOfOrigin = config.Bind("Mod Compat", "RiskyArtifacts - Artifact of Origin", false, "Add monster to Artifact of Origin.");
            //Colossus.KnurlArmor = config.Bind("Colossal Knurl", "Colossal Knurl Armor", 20, "How much armor Colossal Knurl grants.");
            //Colossus.KnurlArmorPerStack = config.Bind("Colossal Knurl", "Colossal Knurl Armor Per Stack", 20, "How much armor Colossal Knurl grants per stack.");
            //Colossus.KnurlGolemAllyDamageModifier = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Damage Modifier", 30, "Additiona damage modifier for ally golem, 10% each.");
            //Colossus.KnurlGolemAllyDamageModifierPerStack = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Damage Modifier Per Stack", 30, "Additiona damage modifier for ally golem per stack, 10% each.");
            //Colossus.KnurlGolemAllyHealthModifier = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Health Modifier", 10, "Additiona health modifier for ally golem, 10% each.");
            //Colossus.KnurlGolemAllyHealthModifierPerStack = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Health Modifier Per Stack", 10, "Additiona health modifier for ally golem per stack, 10% each.");
            //Colossus.KnurlGolemAllySpeedModifier = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Movement Speed Modifier", 5, "Additiona movement speed modifier for ally golem, 14% each (basically one hoof).");
            //Colossus.KnurlGolemAllySpeedModifierPerStack = config.Bind("Colossal Knurl", "Colossal Knurl Golem Ally Movement Speed Modifier Per Stack", 5, "Additiona movement speed modifier for ally golem per stack, 14% each (basically one hoof).");
        }
    }
}
