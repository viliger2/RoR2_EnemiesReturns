using BepInEx.Configuration;
using R2API;
using UnityEngine;

namespace EnemiesReturns.Configuration
{
    public class MechanicalSpider : IConfiguration
    {
        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<int> DirectorCost;
        public static ConfigEntry<int> SelectionWeight;
        public static ConfigEntry<int> MinimumStageCompletion;

        public static ConfigEntry<string> DefaultStageList;
        public static ConfigEntry<string> GrassyStageList;
        public static ConfigEntry<string> SnowyStageList;

        public static ConfigEntry<float> BaseMaxHealth;
        public static ConfigEntry<float> BaseMoveSpeed;
        public static ConfigEntry<float> BaseJumpPower;
        public static ConfigEntry<float> BaseDamage;
        public static ConfigEntry<float> BaseArmor;
        public static ConfigEntry<float> LevelMaxHealth;
        public static ConfigEntry<float> LevelDamage;
        public static ConfigEntry<float> LevelArmor;

        public static ConfigEntry<float> DoubleShotCooldown;
        public static ConfigEntry<float> DoubleShotDamage;
        public static ConfigEntry<float> DoubleShotChargeDuration;
        public static ConfigEntry<int> DoubleShotShots;
        public static ConfigEntry<float> DoubleShotProjectileSpeed;
        public static ConfigEntry<float> DoubleShotDelayBetween;
        public static ConfigEntry<float> DoubleShotMinSpread;
        public static ConfigEntry<float> DoubleShotMaxSpread;

        public static ConfigEntry<float> DashCooldown;
        public static ConfigEntry<float> DashDuration;
        public static ConfigEntry<float> DashHeightCheck;

        public static ConfigEntry<float> DroneSpawnChance;
        public static ConfigEntry<int> DroneCost;
        public static ConfigEntry<float> DroneEliteCostMultiplier;
        public static ConfigEntry<float> DroneBaseRegen;
        public static ConfigEntry<float> DroneLevelRegen;
        public static ConfigEntry<int> DroneBonusHP;
        public static ConfigEntry<int> DroneBonusDamage;
        public static ConfigEntry<bool> DroneUseInitialStageCostCoef;

        public static ConfigEntry<KeyCode> EmoteKey;

        public void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Mechanical Spider Director", "Enable Mechanical Spider", true, "Enables Mechanical Spider.");
            SelectionWeight = config.Bind("Mechanical Spider Director", "Selection Weight", 1, "Selection weight of Mechanical Spider.");
            MinimumStageCompletion = config.Bind("Mechanical Spider Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning.");
            DirectorCost = config.Bind("Mechanical Spider Director", "Director Cost", 28, "Director cost of Mechanical Spider.");

            DefaultStageList = config.Bind("Mechanical Spider Director", "Default Variant Stage List",
                string.Join
                (
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VerdantFalls),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.ViscousFalls),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell),
                    "snowtime_gephyrophobia"
                ),
                "Stages that Default Mechanical Spider appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            SnowyStageList = config.Bind("Mechanical Spider Director", "Snowy Variant Stage List",
                string.Join
                (
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.RallypointDelta),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.RallypointDeltaSimulacrum)
                ),
                "Stages that Snowy Mechanical Spider appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            GrassyStageList = config.Bind("Mechanical Spider Director", "Grassy Variant Stage List",
                string.Join
                (
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SirensCall),
                    "snowtime_gmconstruct",
                    "snowtime_gmflatgrass"
                ),
                "Stages that Grassy Mechanical Spider appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            BaseMaxHealth = config.Bind("Mechanical Spider Character Stats", "Base Max Health", 140f, "Mechanical Spider's base health.");
            BaseMoveSpeed = config.Bind("Mechanical Spider Character Stats", "Base Movement Speed", 9f, "Mechanical Spider's base movement speed.");
            BaseJumpPower = config.Bind("Mechanical Spider Character Stats", "Base Jump Power", 20f, "Mechanical Spider's base jump power.");
            BaseDamage = config.Bind("Mechanical Spider Character Stats", "Base Damage", 12f, "Mechanical Spider's base damage.");
            BaseArmor = config.Bind("Mechanical Spider Character Stats", "Base Armor", 0f, "Mechanical Spider's base armor.");

            LevelMaxHealth = config.Bind("Mechanical Spider Character Stats", "Health per Level", 42f, "Mechanical Spider's health increase per level.");
            LevelDamage = config.Bind("Mechanical Spider Character Stats", "Damage per Level", 2.4f, "Mechanical Spider's damage increase per level.");
            LevelArmor = config.Bind("Mechanical Spider Character Stats", "Armor per Level", 0f, "Mechanical Spider's armor increase per level.");

            DoubleShotCooldown = config.Bind("Mechanical Spider Double Shot", "Double Shot Cooldown", 2f, "Mechanical Spider's Double Shot cooldown.");
            DoubleShotDamage = config.Bind("Mechanical Spider Double Shot", "Double Shot Damage", 1f, "Mechanical Spider's Double Shot damage.");
            DoubleShotShots = config.Bind("Mechanical Spider Double Shot", "Double Shot Shots", 2, "Mechanical Spider's Double Shot number of shots, making it, surprisingly, not double.");
            DoubleShotDelayBetween = config.Bind("Mechanical Spider Double Shot", "Double Shot Delay Between Shots", 0.15f, "Mechanical Spider's Double Shot delay between shots. First shot always comes out instantly after charging state is done, each one after comes out with this delay.");
            DoubleShotProjectileSpeed = config.Bind("Mechanical Spider Double Shot", "Double Shot Projectile Speed", 80f, "Mechanical Spider's Double Shot projectile speed.");
            DoubleShotMinSpread = config.Bind("Mechanical Spider Double Shot", "Double Shot Minimal Spread", 0f, "Mechanical Spider's Double Shot minimal spread.");
            DoubleShotMaxSpread = config.Bind("Mechanical Spider Double Shot", "Double Shot Maximum Spread", 0f, "Mechanical Spider's Double Shot maximum spread.");
            DoubleShotChargeDuration = config.Bind("Mechanical Spider Double Shot", "Double Shot Charge Duration", 0.75f, "Mechanical Spider's Double Shot charge duration.");

            DashCooldown = config.Bind("Mechanical Spider Dash", "Dash Cooldown", 5f, "Mechanical Spider's Dash cooldown.");
            DashDuration = config.Bind("Mechanical Spider Dash", "Dash Duration", 0.75f, "Mechanical Spider's Dash duration. Basically controls how far it will go.");
            DashHeightCheck = config.Bind("Mechanical Spider Dash", "Dash Height Check", 50f, "Checks for falls in front of Mechanical Spider and stops his so it wouldn't yeet itself off cliffs. Set it above 1000 to basically disable the functionality.");

            DroneSpawnChance = config.Bind("Mechanical Spider Drone", "Chance to Spawn Drone", 2f, "Chance to spawn purchasable Mechanical Spider on death.");
            DroneCost = config.Bind("Mechanical Spider Drone", "Drone Cost", 60, "Cost to repair broken Mechanical Spider.");
            DroneEliteCostMultiplier = config.Bind("Mechanical Spider Drone", "Elite Cost Multiplier", 0.5f, "Elite cost multiplier. Multiplies elite director cost to this value and then multiplies gold values to result. T1 elites are 6, T2 elites are 36, honor elites are half of those values.");
            DroneBaseRegen = config.Bind("Mechanical Spider Drone", "Base Regen", 5f, "Base health regeneration of allied Mechanical Spider.");
            DroneLevelRegen = config.Bind("Mechanical Spider Drone", "Regen Per Level", 1f, "Per level health regeneration of allied Mechanical Spider.");
            DroneBonusHP = config.Bind("Mechanical Spider Drone", "Bonus HP Boost", 20, "Bonus health boost from base stats (the same as normal spider), boosts by 10% for each value.");
            DroneBonusDamage = config.Bind("Mechanical Spider Drone", "Bonus Damage Boost", 10, "Bonus damage boost from base stats (the same as normal spider), boosts by 10% for each value.");
            DroneUseInitialStageCostCoef = config.Bind("Mechanical Spider Drone", "Use Initial Stage Cost Coefficient", false, "Use initial stage coefficient for price. Basically it means that the cost of spider drone won't scale with time on current stage, using the same price coefficient that was used when initial interactables were spawned. So if spider drone spawns the moment you enter a stage or 5 minutes into it price will be the same.");

            EmoteKey = config.Bind("Mechanical Spider Emotes", "Dance Emote", KeyCode.Alpha1, "Key used to Dance.");
        }
    }
}
