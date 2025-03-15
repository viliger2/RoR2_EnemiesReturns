using BepInEx.Configuration;
using R2API;

namespace EnemiesReturns.Configuration.LynxTribe
{
    public class LynxTotem : IConfiguration
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

        public static ConfigEntry<float> SummonTribeCooldown;
        public static ConfigEntry<int> SummonTribeSummonCountPerCast;
        public static ConfigEntry<int> SummonTribeMaxCount;

        public static ConfigEntry<float> SummonStormCooldown;
        public static ConfigEntry<float> SummonStormMinRange;
        public static ConfigEntry<float> SummonStormMaxRange;
        public static ConfigEntry<int> SummonStormMaxCount;

        public static ConfigEntry<float> SummonStormStormMoveSpeed;
        public static ConfigEntry<float> SummonStormRadius;
        public static ConfigEntry<float> SummonStormPullStrength;
        public static ConfigEntry<float> SummmonStormLifetime;
        public static ConfigEntry<float> SummonStormGrabRange;
        public static ConfigEntry<float> SummonStormGrabDuration;
        public static ConfigEntry<float> SummonStormThrowForce;
        public static ConfigEntry<float> SummonStormImmunityDuration;
        public static ConfigEntry<float> SummonStormPoisonDuration;
        public static ConfigEntry<float> SummonStormPoisonCoefficient;
        public static ConfigEntry<bool> SummonStormOldBehavior;
        public static ConfigEntry<bool> SummonStormZeroJumps;

        public static ConfigEntry<float> GroundpoundCooldown;
        public static ConfigEntry<float> GroundpoundDamage;
        public static ConfigEntry<float> GroundpoundProcCoefficient;
        public static ConfigEntry<float> GroundpoundRadius;
        public static ConfigEntry<float> GroundpoundForce;

        public static ConfigEntry<bool> ItemEnabled;
        public static ConfigEntry<int> LynxFetishBonusHP;
        public static ConfigEntry<int> LynxFetishBonusDamage;
        public static ConfigEntry<int> LynxFetishBonusHPPerStack;
        public static ConfigEntry<int> LynxFetishBonusDamagePerStack;
        public static ConfigEntry<float> LynxFetishBuffWardRadius;
        public static ConfigEntry<float> LynxFetishBuffWardBuffDuration;
        public static ConfigEntry<float> LynxFetishBuffWardBuffRefreshTimer;
        public static ConfigEntry<float> LynxFetishArcherDamageBuff;
        public static ConfigEntry<float> LynxFetishHunterArmorBuff;
        public static ConfigEntry<float> LynxFetishScoutSpeedBuff;
        public static ConfigEntry<float> LynxFetishShamanSpecialBuff;

        public static ConfigEntry<bool> AddToArtifactOfOrigin;

        public void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Lynx Totem Director", "Enable Lynx Totem", true, "Enables Lynx Totem.");
            SelectionWeight = config.Bind("Lynx Totem Director", "Selection Weight", 1, "Selection weight of Lynx Totem.");
            MinimumStageCompletion = config.Bind("Lynx Totem Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning.");
            DirectorCost = config.Bind("Lynx Totem Director", "Director Cost", 600, "Director cost of Lynx Totem.");
            DefaultStageList = config.Bind("Lynx Totem Director", "Default Variant Stage List",
                string.Join(",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.TitanicPlains),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.DistantRoost),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.ShatteredAbodes),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.DisturbedImpact),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.WetlandAspect),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.TreebornColony),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.GoldenDieback),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SunderedGrove),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SkyMeadow),

                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.TitanicPlainsSimulacrum),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SkyMeadowSimulacrum)
                ),
                "Stages that Default Lynx Totem appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            BaseMaxHealth = config.Bind("Lynx Totem Character Stats", "Base Max Health", 2100f, "Lynx Totem' base health.");
            BaseMoveSpeed = config.Bind("Lynx Totem Character Stats", "Base Movement Speed", 7f, "Lynx Totem' base movement speed.");
            BaseJumpPower = config.Bind("Lynx Totem Character Stats", "Base Jump Power", 10f, "Lynx Totem' base jump power.");
            BaseDamage = config.Bind("Lynx Totem Character Stats", "Base Damage", 16f, "Lynx Totem' base damage.");
            BaseArmor = config.Bind("Lynx Totem Character Stats", "Base Armor", 20f, "Lynx Totem' base armor.");

            LevelMaxHealth = config.Bind("Lynx Totem Character Stats", "Health per Level", 630f, "Lynx Totem' health increase per level.");
            LevelDamage = config.Bind("Lynx Totem Character Stats", "Damage per Level", 3.2f, "Lynx Totem' damage increase per level.");
            LevelArmor = config.Bind("Lynx Totem Character Stats", "Armor per Level", 0f, "Lynx Totem' armor increase per level.");

            SummonTribeCooldown = config.Bind("Lynx Totem Summon Tribe", "Summon Tribe Cooldown", 25f, "Lynx Totem's Summon Tribe cooldown.");
            SummonTribeSummonCountPerCast = config.Bind("Lynx Totem Summon Tribe", "Summon Tribe Summon Count Per Use", 4, "Lynx Totem's Summon Tribe number of summoned tribesmen per ability use.");
            SummonTribeMaxCount = config.Bind("Lynx Totem Summon Tribe", "Summon Tribe Summon Cap", 12, "Lynx Totem's Summon Tribe capacity. Once Totem reaches capacity it stops summoning tribesmen until at least one is dead.");

            SummonStormCooldown = config.Bind("Lynx Totem Summon Storm", "Summon Storm Cooldown", 35f, "Lynx Totem's Summon Storm cooldown.");
            SummonStormMinRange = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Minimum Range", 10f, "Lynx Totem's Summon Storm's minimum range of spawning from target.");
            SummonStormMaxRange = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Maximum Range", 20f, "Lynx Totem's Summon Storm's maximum range of spawning from target.");
            SummonStormMaxCount = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Max Count", 16, "Lynx Totem's Summon Storm's max count. It summons one storm for each player, this config is for limiting number of storms for modded lobies.");

            SummonStormStormMoveSpeed = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summoned Storm Movement Speed", 8f, "Lynx Totem's Summoned Storm movement speed.");
            SummonStormRadius = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Radius", 20f, "Lynx Totem's radius of summoned storms.");
            SummonStormPullStrength = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Pull Strength", 5f, "Lynx Totem's summoned storms pull streigth.");
            SummmonStormLifetime = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Lifetime", 20f, "Lynx Totem's summoned storms lifetime.");
            SummonStormGrabRange = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Grab Range", 3.6f, "Lynx Totem's summoned storms grab range.");
            SummonStormGrabDuration = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Grab Duration", 4f, "Lynx Totem's summoned storms grab duration. Basically for how long target stays in the air.");
            SummonStormThrowForce = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Throw Force", 3500f, "Lynx Totem's summoned storms throw force at the end of the grab.");
            SummonStormImmunityDuration = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Immunity Duration", 8f, "For how long players are immune to getting succed by a storm after they got throwned out by it.");
            SummonStormPoisonDuration = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Posion Duration", 7f, "For how long posion DoT is applied. Since posion deals static damage every tick it directly affect total damage dealt.");
            SummonStormPoisonCoefficient = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Posion Damage Coefficient", 0.8f, "Poison damage coefficient. While poison should deal % health damage, players would realistically never get enough health to overshadow initial calculation of damage off base damage, so you can use this value to adjust poison damage directly. Poison deals 33% of base damage every 0.33 seconds before this coefficient.");
            SummonStormOldBehavior = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Old Behavior", false, "Enables Summon Storm old behavior where it would summon one storm for each living player");
            SummonStormZeroJumps = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Zero Out Jump Count", true, "If player gets grabbed by the storm their jump counts gets zero'd so they would not be able to escape with double jumps.");

            GroundpoundCooldown = config.Bind("Lynx Totem Groundpound", "Lynx Totem Groundpound Cooldown", 10f, "Lynx Totem's Groundpound's cooldown.");
            GroundpoundDamage = config.Bind("Lynx Totem Groundpound", "Lynx Totem Groundpound Damage", 3f, "Lynx Totem's Groundpound's damage coefficient.");
            GroundpoundProcCoefficient = config.Bind("Lynx Totem Groundpound", "Lynx Totem Groundpound Proc Coefficient", 1f, "Lynx Totem's Groundpound's proc coefficient.");
            GroundpoundForce = config.Bind("Lynx Totem Groundpound", "Lynx Totem Groundpound Force", 1500f, "Lynx Totem's Groundpound's upwards force.");
            GroundpoundRadius = config.Bind("Lynx Totem Groundpound", "Lynx Totem Groundpound Radius", 25f, "Lynx Totem's Groundpound's radius.");

            ItemEnabled = config.Bind("Lynx Fetish", "Enable Lynx Fetish", true, "Enables Lynx Fetish to drop from Lynx Totem and appear in printers. Item cannot be enabled without Totem.");
            LynxFetishBonusDamage = config.Bind("Lynx Fetish", "Spawned Tribesmen Bonus Damage", 10, "Bonus damage boost from base stats (the same as normal lynx tribesman), boosts by 10% for each value.");
            LynxFetishBonusHP = config.Bind("Lynx Fetish", "Spawned Tribesmen Bonus Health", 10, "Bonus health boost from base stats (the same as normal lynx tribesman), boosts by 10% for each value.");
            LynxFetishBonusDamagePerStack = config.Bind("Lynx Fetish", "Spawned Tribesmen Bonus Damage Per Stack", 3, "Bonus damage boost from base stats per stack after 4th (the same as normal lynx tribesman), boosts by 10% for each value.");
            LynxFetishBonusHPPerStack = config.Bind("Lynx Fetish", "Spawned Tribesmen Bonus Health Per Stack", 2, "Bonus health boost from base stats per stack after 4th (the same as normal lynx tribesman), boosts by 10% for each value.");
            LynxFetishBuffWardRadius = config.Bind("Lynx Fetish", "Spawned Tribesmen Buff Ward Radius", 60f, "Buff ward radius of spawned tribesmen. By default equal to teleporter hold out zone radius.");
            LynxFetishBuffWardBuffDuration = config.Bind("Lynx Fetish", "Spawned Tribesmen Buff Ward Buff Duration", 5f, "Buff ward applied buff duration of spawned tribesmen.");
            LynxFetishBuffWardBuffRefreshTimer = config.Bind("Lynx Fetish", "Spawned Tribesmen Buff Ward Buff Refresh Timer", 4f, "Buff ward refresh timer of buff application of spawned tribesmen.");

            LynxFetishArcherDamageBuff = config.Bind("Lynx Fetish", "Spawned Archer Damage Buff Value", 15f, "Damage buff value of spawned Archer in percent.");
            LynxFetishHunterArmorBuff = config.Bind("Lynx Fetish", "Spawned Hunter Armor Buff Value", 20f, "Armor buff value of spawned Hunter.");
            LynxFetishScoutSpeedBuff = config.Bind("Lynx Fetish", "Spawned Scout Attack and Movement Speed Buff Value", 15f, "Attack and Movement speed buff value of spawned Scout in percent.");
            LynxFetishShamanSpecialBuff = config.Bind("Lynx Fetish", "Spawned Shaman Special Ability Damage Buff Value", 30f, "Damage buff of special ability value of spawned Shaman in percent.");

            AddToArtifactOfOrigin = config.Bind("Mod Compat", "RiskyArtifacts - Artifact of Origin", true, "Add monster to Artifact of Origin.");
        }
    }
}
