using BepInEx.Configuration;
using R2API;
using UnityEngine;

namespace EnemiesReturns.Configuration
{
    internal class ArcherBug : IConfiguration
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

        public static ConfigEntry<float> CausticSpitCooldown;
        public static ConfigEntry<float> CausticSpitDamage;
        public static ConfigEntry<float> CausticSpitProcCoefficient;
        public static ConfigEntry<float> CausticSpitBlastRadius;
        public static ConfigEntry<float> CausticSpitForce;
        public static ConfigEntry<int> CausitcSpitProjectileCount;
        public static ConfigEntry<float> CausticSpitProjectileSpread;

        public static ConfigEntry<KeyCode> BuckBumbleKey;

        public void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Archer Bug Director", "Enable Archer Bug", true, "Enables Archer Bug.");

            DirectorCost = config.Bind("Archer Bug Director", "Director Cost", 28, "Director cost of Archer Bug.");
            SelectionWeight = config.Bind("Archer Bug Director", "Selection Weight", 1, "Selection weight of Archer Bug.");
            MinimumStageCompletion = config.Bind("Archer Bug Director", "Minimum Stage Completion", 1, "Minimum stages players need to complete before monster starts spawning.");

            BaseMaxHealth = config.Bind("Archer Bug Character Stats", "Base Max Health", 140f, "Archer Bug' base health.");
            BaseMoveSpeed = config.Bind("Archer Bug Character Stats", "Base Movement Speed", 30f, "Archer Bug' base movement speed.");
            BaseJumpPower = config.Bind("Archer Bug Character Stats", "Base Jump Power", 18f, "Archer Bug' base jump power.");
            BaseDamage = config.Bind("Archer Bug Character Stats", "Base Damage", 12f, "Archer Bug' base damage.");
            BaseArmor = config.Bind("Archer Bug Character Stats", "Base Armor", 0f, "Archer Bug' base armor.");

            LevelMaxHealth = config.Bind("Archer Bug Character Stats", "Health per Level", 42f, "Archer Bug' health increase per level.");
            LevelDamage = config.Bind("Archer Bug Character Stats", "Damage per Level", 2.4f, "Archer Bug' damage increase per level.");
            LevelArmor = config.Bind("Archer Bug Character Stats", "Armor per Level", 0f, "Archer Bug' armor increase per level.");

            CausticSpitCooldown = config.Bind("Archer Bug Caustic Spit", "Caustic Spit Cooldown", 3f, "Archer Bug's Caustic Spit cooldown.");
            CausticSpitDamage = config.Bind("Archer Bug Caustic Spit", "Caustic Spit Damage", 1.2f, "Archer Bug's Caustic Spit damage.");
            CausticSpitProcCoefficient = config.Bind("Archer Bug Caustic Spit", "Caustic Spit Proc Coefficient", 1f, "Archer Bug's Caustic Spit proc coefficient.");
            CausticSpitForce = config.Bind("Archer Bug Caustic Spit", "Caustic Spit Projectile Force", 0f, "Archer Bug's Caustic Spit force.");
            CausticSpitBlastRadius = config.Bind("Archer Bug Caustic Spit", "Caustic Spit Blast Radius", 2f, "Archer Bug's Caustic Spit blast radius.");
            CausticSpitProjectileSpread = config.Bind("Archer Bug Caustic Spit", "Caustic Spit Projectile Spread", 20f, "Archer Bug's Caustic Spit projectile spread, basically angle between projectiles.");
            CausitcSpitProjectileCount = config.Bind("Archer Bug Caustic Spit", "Caustic Spit Projectile Count", 3, "Archer Bug's Caustic Spit projectile count.");

            ArcherBug.DefaultStageList = config.Bind("Archer Bug Director", "Default Variant Stage List",
                string.Join(
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.AphelianSanctuary),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.AphelianSanctuarySimulacrum),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SunderedGrove),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.TreebornColony),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.GoldenDieback),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.ScorchedAcres),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SirensCall),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell),
                    "snowtime_gmconstruct",
                    "snowtime_sandtrap",
                    "snowtime_gmflatgrass"
                    ),
                "Stages that Default Archer Bugs appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            BuckBumbleKey = config.Bind("Buck Bumble", "Buck Bumble", KeyCode.Alpha1, "Mah bassy madde ooh la de DE.");
        }
    }
}
