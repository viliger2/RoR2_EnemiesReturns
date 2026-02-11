using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.Configuration
{
    public class MechanicalSpider : IConfiguration
    {
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

        public static ConfigEntry<float> DroneBaseMaxHealth;
        public static ConfigEntry<float> DroneBaseMoveSpeed;
        public static ConfigEntry<float> DroneBaseDamage;
        public static ConfigEntry<float> DroneBaseArmor;
        public static ConfigEntry<float> DroneBaseRegen;

        public static ConfigEntry<float> DroneLevelMaxHealth;
        public static ConfigEntry<float> DroneLevelDamage;
        public static ConfigEntry<float> DroneLevelArmor;
        public static ConfigEntry<float> DroneLevelRegen;

        public static ConfigEntry<bool> DroneUseInitialStageCostCoef;
        public static ConfigEntry<DroneType> DroneType;
        public static ConfigEntry<ItemTier> DroneTier;
        public static ConfigEntry<bool> DroneCanDrop;
        public static ConfigEntry<bool> DroneCanScrap;
        public static ConfigEntry<bool> DroneCanCombine;

        public static ConfigEntry<KeyCode> EmoteKey;

        public static ConfigEntry<bool> EngiSkillEnabled;
        public static ConfigEntry<bool> EngiSkillForceUnlock;

        public static ConfigEntry<int> EngiSkillBaseMaxStock;
        public static ConfigEntry<float> EngiSkillBaseRecharge;
        public static ConfigEntry<int> EngiSkillRechargeStock;

        public static ConfigEntry<int> EngiSkillScepterBaseMaxStock;
        public static ConfigEntry<float> EngiSkillScepterBaseRecharge;
        public static ConfigEntry<int> EngiSkillScepterRechargeStock;

        public static ConfigEntry<float> TurretBaseMoveSpeed;
        public static ConfigEntry<float> TurretBaseRegen;
        public static ConfigEntry<float> TurretLevelRegen;
        public static ConfigEntry<float> TurretDoubleShotDamage;

        public static ConfigEntry<int> ScepterTurretProjectileMultiplier;
        public static ConfigEntry<float> ScepterTurretAttackSpeedMultiplier;

        public void PopulateConfig(ConfigFile config)
        {
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
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.ConduitCanyon),
                    "snowtime_gephyrophobia"
                ),
                "Stages that Default Mechanical Spider appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            SnowyStageList = config.Bind("Mechanical Spider Director", "Snowy Variant Stage List",
                string.Join
                (
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.RallypointDelta),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.RallypointDeltaSimulacrum),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.PretendersPrecipice)
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

            DoubleShotCooldown = config.Bind("Mechanical Spider Double Shot", "Double Shot Cooldown", 0f, "Mechanical Spider's Double Shot cooldown.");
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

            DroneBaseMaxHealth = config.Bind("Mechanical Spider Drone", "Base Max Health", 420f, "Mechanical Spider's base health.");
            DroneBaseMoveSpeed = config.Bind("Mechanical Spider Drone", "Base Movement Speed", 9f, "Mechanical Spider's base movement speed.");
            DroneBaseDamage = config.Bind("Mechanical Spider Drone", "Base Damage", 36f, "Mechanical Spider's base damage.");
            DroneBaseArmor = config.Bind("Mechanical Spider Drone", "Base Armor", 0f, "Mechanical Spider's base armor.");
            DroneBaseRegen = config.Bind("Mechanical Spider Drone", "Base Regen", 15f, "Base health regeneration of allied Mechanical Spider.");

            DroneLevelMaxHealth = config.Bind("Mechanical Spider Character Stats", "Health per Level", 126f, "Mechanical Spider's health increase per level.");
            DroneLevelDamage = config.Bind("Mechanical Spider Character Stats", "Damage per Level", 7.2f, "Mechanical Spider's damage increase per level.");
            DroneLevelArmor = config.Bind("Mechanical Spider Character Stats", "Armor per Level", 0f, "Mechanical Spider's armor increase per level.");
            DroneLevelRegen = config.Bind("Mechanical Spider Drone", "Regen Per Level", 3f, "Per level health regeneration of allied Mechanical Spider.");

            DroneUseInitialStageCostCoef = config.Bind("Mechanical Spider Drone", "Use Initial Stage Cost Coefficient", false, "Use initial stage coefficient for price. Basically it means that the cost of spider drone won't scale with time on current stage, using the same price coefficient that was used when initial interactables were spawned. So if spider drone spawns the moment you enter a stage or 5 minutes into it price will be the same.");
            DroneType = config.Bind("Mechanical Spider Drone", "Drone Type", RoR2.DroneType.Combat, "Drone type. Used for Drone Catalog and determines highlight color, somewhere (lol).");
            DroneTier = config.Bind("Mechanical Spider Drone", "Drone Tier", ItemTier.Tier2, "Drone tier. Determines what scrap is dropped.");
            DroneCanDrop = config.Bind("Mechanical Spider Drone", "Drone Can Drop", false, "Enables drone to appear in drone shops.");
            DroneCanScrap = config.Bind("Mechanical Spider Drone", "Drone Can Scrap", true, "Enables scrapping of drone.");
            DroneCanCombine = config.Bind("Mechanical Spider Drone", "Drone Can Combine", true, "Enables combining of drone.");

            EmoteKey = config.Bind("Mechanical Spider Emotes", "Dance Emote", KeyCode.Alpha1, "Key used to Dance.");

            EngiSkillEnabled = config.Bind("Engineer Special Skill", "Enable", true, "Enables Engineer Special skill to use Mechanical Spiders as turrets.");
            EngiSkillForceUnlock = config.Bind("Engineer Special Skill", "Force Unlock", false, "Force unlocks Engineer Special skill.");

            EngiSkillBaseMaxStock = config.Bind("Engineer Special Skill", "Base Max Stock", 2, "Base maximum stock for Engineer skill. Does not affect maximum number of turrets.");
            EngiSkillBaseRecharge = config.Bind("Engineer Special Skill", "Base Recharge Interval", 30f, "Base recharge interval for Engineer skill.");
            EngiSkillRechargeStock = config.Bind("Engineer Special Skill", "Recharge Stock", 1, "Ammount of stock that gets recharged on skill cooldown.");

            EngiSkillScepterBaseMaxStock = config.Bind("Engineer Special Skill", "Scepter Base Max Stock", 2, "Base maximum stock for Engineer skill with Scepter. Does not affect maximum number of turrets.");
            EngiSkillScepterBaseRecharge = config.Bind("Engineer Special Skill", "Scepter Base Recharge Interval", 30f, "Base recharge interval for Engineer skill with Scepter.");
            EngiSkillScepterRechargeStock = config.Bind("Engineer Special Skill", "Scepter Recharge Stock", 1, "Ammount of stock that gets recharged on skill cooldown with Scepter.");

            TurretBaseRegen = config.Bind("Engineer Special Skill", "Turret Base Regen", 1f, "Base health regeneration of Engi Mechanical Spider.");
            TurretLevelRegen = config.Bind("Engineer Special Skill", "Turret Level Regen", 0.2f, "Per level health regeneration of Engi Mechanical Spider.");
            TurretBaseMoveSpeed = config.Bind("Engineer Special Skill", "Turret Movement Speed", 10.15f, "Base movement speed of Engi Mechanical Spider.");
            TurretDoubleShotDamage = config.Bind("Engineer Special Skill", "Turret Double Shot Damage", 2.5f, "Double Shot damage of Engi Mechanical Spider.");

            ScepterTurretProjectileMultiplier = config.Bind("Engineer Special Skill", "Scepter Projecile Multiplier", 2, "Projectile count multiplier for Engi Mechanical Spiders with Scepter.");
            ScepterTurretAttackSpeedMultiplier = config.Bind("Engineer Special Skill", "Scepter Attack Speed Multiplier", 2f, "Attack speed multiplier for Engi Mechanical Spiders with Scepter.");
        }
    }
}
