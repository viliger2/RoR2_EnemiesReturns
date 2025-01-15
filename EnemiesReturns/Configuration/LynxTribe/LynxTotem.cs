using BepInEx.Configuration;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration.LynxTribe
{
    public class LynxTotem
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
        public static ConfigEntry<int> SummonTribeSummonCount;

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

        public static ConfigEntry<float> GroundpoundCooldown;
        public static ConfigEntry<float> GroundpoundDamage;
        public static ConfigEntry<float> GroundpoundProcCoefficient;
        public static ConfigEntry<float> GroundpoundRadius;
        public static ConfigEntry<float> GroundpoundForce;

        public static void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Lynx Totem Director", "Enable Lynx Totem", true, "Enables Lynx Totem.");
            SelectionWeight = config.Bind("Lynx Totem Director", "Selection Weight", 1, "Selection weight of Lynx Totem.");
            MinimumStageCompletion = config.Bind("Lynx Totem Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning.");
            DirectorCost = config.Bind("Lynx Totem Director", "Director Cost", 800, "Director cost of Lynx Totem.");
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
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell),

                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.TitanicPlainsSimulacrum),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SkyMeadowSimulacrum)
                ),
                "Stages that Default Lynx Totem appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            BaseMaxHealth = config.Bind("Lynx Totem Character Stats", "Base Max Health", 2800f, "Lynx Totem' base health.");
            BaseMoveSpeed = config.Bind("Lynx Totem Character Stats", "Base Movement Speed", 7f, "Lynx Totem' base movement speed.");
            BaseJumpPower = config.Bind("Lynx Totem Character Stats", "Base Jump Power", 10f, "Lynx Totem' base jump power.");
            BaseDamage = config.Bind("Lynx Totem Character Stats", "Base Damage", 16f, "Lynx Totem' base damage.");
            BaseArmor = config.Bind("Lynx Totem Character Stats", "Base Armor", 20f, "Lynx Totem' base armor.");

            LevelMaxHealth = config.Bind("Lynx Totem Character Stats", "Health per Level", 840f, "Lynx Totem' health increase per level.");
            LevelDamage = config.Bind("Lynx Totem Character Stats", "Damage per Level", 3.2f, "Lynx Totem' damage increase per level.");
            LevelArmor = config.Bind("Lynx Totem Character Stats", "Armor per Level", 0f, "Lynx Totem' armor increase per level.");

            SummonTribeCooldown = config.Bind("Lynx Totem Summon Tribe", "Summon Tribe Cooldown", 25f, "Lynx Totem's Summon Tribe cooldown.");
            SummonTribeSummonCount = config.Bind("Lynx Totem Summon Tribe", "Summon Tribe summon cooldown", 4, "Lynx Totem's Summon Tribe number of summoned tribesmen.");

            SummonStormCooldown = config.Bind("Lynx Totem Summon Storm", "Summon Storm Cooldown", 40f, "Lynx Totem's Summon Storm cooldown.");
            SummonStormMinRange = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Minimum Range", 10f, "Lynx Totem's Summon Storm's minimum range of spawning from target.");
            SummonStormMaxRange = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Maximum Range", 20f, "Lynx Totem's Summon Storm's maximum range of spawning from target.");
            SummonStormMaxCount = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Max Count", 16, "Lynx Totem's Summon Storm's max count. It summons one storm for each player, this config is for limiting number of storms for modded lobies.");

            SummonStormStormMoveSpeed = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summoned Storm Movement Speed", 8.5f, "Lynx Totem's Summoned Storm movement speed.");
            SummonStormRadius = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Radius", 20f, "Lynx Totem's radius of summoned storms.");
            SummonStormPullStrength = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Pull Strength", 5f, "Lynx Totem's summoned storms pull streigth.");
            SummmonStormLifetime = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Lifetime", 30f, "Lynx Totem's summoned storms lifetime.");
            SummonStormGrabRange = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Grab Range", 3f, "Lynx Totem's summoned storms grab range.");
            SummonStormGrabDuration = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Grab Duration", 4f, "Lynx Totem's summoned storms grab duration. Basically for how long target stays in the air.");
            SummonStormThrowForce = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Throw Force", 3500f, "Lynx Totem's summoned storms throw force at the end of the grab.");
            SummonStormImmunityDuration = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Storm Immunity Duration", 5f, "For how long players are immune to getting succed by a storm after they got throwned out by it.");
            SummonStormPoisonDuration = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Posion Duration", 5f, "For how long posion DoT is applied. Since posion deals static damage every tick it directly affect total damage dealt.");
            SummonStormPoisonCoefficient = config.Bind("Lynx Totem Summon Storm", "Lynx Totem Summon Posion Damage Coefficient", 0.6f, "Poison damage coefficient. While poison should deal % health damage, players would realistically never get enough health to overshadow initial calculation of damage off base damage, so you can use this value to adjust poison damage directly. Poison deals 33% of base damage every 0.33 seconds before this coefficient.");

            GroundpoundCooldown = config.Bind("Lynx Totem Groundpound", "Lynx Totem Groundpound Cooldown", 10f, "Lynx Totem's Groundpound's cooldown.");
            GroundpoundDamage = config.Bind("Lynx Totem Groundpound", "Lynx Totem Groundpound Damage", 3f, "Lynx Totem's Groundpound's damage coefficient.");
            GroundpoundProcCoefficient = config.Bind("Lynx Totem Groundpound", "Lynx Totem Groundpound Proc Coefficient", 1f, "Lynx Totem's Groundpound's proc coefficient.");
            GroundpoundForce = config.Bind("Lynx Totem Groundpound", "Lynx Totem Groundpound Force", 1500f, "Lynx Totem's Groundpound's upwards force.");
            GroundpoundRadius = config.Bind("Lynx Totem Groundpound", "Lynx Totem Groundpound Radius", 25f, "Lynx Totem's Groundpound's radius.");
        }
    }
}
