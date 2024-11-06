using BepInEx.Configuration;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration
{
    public static class Ifrit
    {
        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<int> DirectorCost;
        public static ConfigEntry<int> SelectionWeight;
        public static ConfigEntry<int> MinimumStageCompletion;

        public static ConfigEntry<string> DefaultStageList;

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

        public static ConfigEntry<bool> AddToArtifactOfOrigin;

        public static void PopulateConfig(ConfigFile config)
        {
            Ifrit.Enabled = config.Bind("Ifrit Director", "Enable Ifrit", true, "Enables Ifrit.");
            Ifrit.SelectionWeight = config.Bind("Ifrit Director", "Selection Weight", 1, "Selection weight of Ifrit.");
            Ifrit.MinimumStageCompletion = config.Bind("Ifrit Director", "Minimum Stage Completion", 2, "Minimum stages players need to complete before monster starts spawning.");
            Ifrit.DirectorCost = config.Bind("Ifrit Director", "Director Cost", 800, "Director cost of Ifrit.");

            Ifrit.DefaultStageList = config.Bind("Ifrit Director", "Default Variant Stage List",
                string.Join(
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.AbyssalDepths),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.AbyssalDepthsSimulacrum),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SiphonedForest),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.RallypointDelta),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.RallypointDeltaSimulacrum),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.HelminthHatchery),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell),
                    "catacombs_DS1_Catacombs",
                    "snowtime_gephyrophobia"
                    ),
                "Stages that Default Ifrit appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            Ifrit.BaseMaxHealth = config.Bind("Ifrit Character Stats", "Base Max Health", 2800f, "Ifrit' base health.");
            Ifrit.BaseMoveSpeed = config.Bind("Ifrit Character Stats", "Base Movement Speed", 13f, "Ifrit' base movement speed.");
            Ifrit.BaseJumpPower = config.Bind("Ifrit Character Stats", "Base Jump Power", 15f, "Ifrit' base jump power.");
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

            AddToArtifactOfOrigin = config.Bind("Mod Compat", "RiskyArtifacts - Artifact of Origin", true, "Add monster to Artifact of Origin.");
        }
    }
}
