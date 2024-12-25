using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Configuration.LynxTribe
{
    public static class LynxShaman
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

        public static ConfigEntry<float> SummonProjectilesCooldown;
        public static ConfigEntry<float> SummonProjectilesDamage;
        public static ConfigEntry<float> SummonProjectileProcCoefficient;
        public static ConfigEntry<int> SummonProjectilesCount;
        public static ConfigEntry<float> SummonProjectilesDebuffDuration;
        public static ConfigEntry<float> SummonProjectilesHealingFraction;
        public static ConfigEntry<float> SummonProjectilesLifetime;
        public static ConfigEntry<float> SummonProjectilesSpeed;
        public static ConfigEntry<float> SummonProjectilesTurnSpeed;

        public static ConfigEntry<float> PushBackCooldown;
        public static ConfigEntry<float> PushBackDamage;
        public static ConfigEntry<float> PushBackProcCoefficient;
        public static ConfigEntry<float> PushBackForce;
        public static ConfigEntry<float> PushBackRadius;

        public static ConfigEntry<float> SummonStormCooldown;
        public static ConfigEntry<float> SummonStormCastTime;
        public static ConfigEntry<float> SummonStormMinRange;
        public static ConfigEntry<float> SummonStormMaxRange;
        public static ConfigEntry<float> SummonStormRechargeOnFailure;

        public static ConfigEntry<float> SummonStormStormMoveSpeed;
        public static ConfigEntry<int> SummonStormCount;
        public static ConfigEntry<float> SummonStormRadius;
        public static ConfigEntry<float> SummonStormPullStrength;
        public static ConfigEntry<float> SummmonStormLifetime;
        public static ConfigEntry<float> SummonStormGrabRange;
        public static ConfigEntry<float> SummonStormGrabDuration;
        public static ConfigEntry<float> SummonStormThrowForce;
        public static ConfigEntry<float> SummonStormImmunityDuration;

        public static ConfigEntry<KeyCode> NopeEmoteKey;
        public static ConfigEntry<KeyCode> SingEmoteKey;

        public static void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Lynx Shaman Director", "Enable Lynx Shaman", true, "Enables Lynx Shaman.");
            SelectionWeight = config.Bind("Lynx Shaman Director", "Selection Weight", 1, "Selection weight of Lynx Shaman.");
            MinimumStageCompletion = config.Bind("Lynx Shaman Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning.");
            DirectorCost = config.Bind("Lynx Shaman Director", "Director Cost", 40, "Director cost of Lynx Shaman.");

            BaseMaxHealth = config.Bind("Lynx Shaman Character Stats", "Base Max Health", 300f, "Lynx Shaman' base health.");
            BaseMoveSpeed = config.Bind("Lynx Shaman Character Stats", "Base Movement Speed", 6f, "Lynx Shaman' base movement speed.");
            BaseJumpPower = config.Bind("Lynx Shaman Character Stats", "Base Jump Power", 14f, "Lynx Shaman' base jump power.");
            BaseDamage = config.Bind("Lynx Shaman Character Stats", "Base Damage", 20f, "Lynx Shaman' base damage.");
            BaseArmor = config.Bind("Lynx Shaman Character Stats", "Base Armor", 0f, "Lynx Shaman' base armor.");

            LevelMaxHealth = config.Bind("Lynx Shaman Character Stats", "Health per Level", 90f, "Lynx Shaman' health increase per level.");
            LevelDamage = config.Bind("Lynx Shaman Character Stats", "Damage per Level", 4f, "Lynx Shaman' damage increase per level.");
            LevelArmor = config.Bind("Lynx Shaman Character Stats", "Armor per Level", 0f, "Lynx Shaman' armor increase per level.");

            SummonProjectilesCooldown = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Cooldown", 8f, "Lynx Shaman's Summon Projectiles cooldown.");
            SummonProjectilesDamage = config.Bind("Lynx Shaman Summon Projectiles", "Lunx Shaman Summon Projectiles Damage", 1f, "Lynx Shaman's Summon Projectiles damage coefficient.");
            SummonProjectileProcCoefficient = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Proc Coefficient", 1f, "Lynx Shaman's Summon Projectiles proc coefficient.");
            SummonProjectilesCount = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Count", 5, "Lynx Shaman's Summon Projectiles count.");
            SummonProjectilesDebuffDuration = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Debuff Duration", 3f, "Lynx Shaman's Summon Projectiles minus healing debuff duration.");
            SummonProjectilesHealingFraction = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Healing Debuff Fraction", 0.25f, "Lynx Shaman's Summon Projectiles healing reduction.");
            SummonProjectilesLifetime = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Lifetime", 7f, "Lynx Shaman's Summon Projectiles lifetime of projectiles.");
            SummonProjectilesSpeed = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Speed", 8.5f, "Lynx Shaman's Summon Projectiles speed of projectiles.");
            SummonProjectilesTurnSpeed = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Turn Speed", 90f, "Lynx Shaman's Summon Projectiles turn speed of projectiles.");

            PushBackCooldown = config.Bind("Lynx Shaman Push Back", "Lynx Shaman Push Back Cooldown", 4f, "Lynx Shaman's Push Back Cooldown.");
            PushBackDamage = config.Bind("Lynx Shaman Push Back", "Lynx Shaman Push Back Damage", 1f, "Lynx Shaman's Push Back damage coefficient.");
            PushBackProcCoefficient = config.Bind("Lynx Shaman Push Back", "Lynx Shaman Push Back Proc Coefficient", 1f, "Lynx Shaman's Push Back proc coefficient.");
            PushBackForce = config.Bind("Lynx Shaman Push Back", "Lynx Shaman Push Back Force", 2000f, "Lynx Shaman's Push Back force.");
            PushBackRadius = config.Bind("Lynx Shaman Push Back", "Lynx Shaman Push Back Radius", 6f, "Lynx Shaman's Push Back attack radius.");

            NopeEmoteKey = config.Bind("Lynx Shaman Emotes", "Nope Emote", KeyCode.Alpha1, "Key used to Nope.");
            SingEmoteKey = config.Bind("Lynx Shaman Emotes", "Sing Emote", KeyCode.Alpha2, "Key used to Sing.");






            SummonStormCooldown = config.Bind("Lynx Shaman Summon Storm", "Lynx Shaman Summon Storm Cooldown", 30f, "Lynx Shaman's Summon Storm Cooldown.");
            SummonStormStormMoveSpeed = config.Bind("Lynx Shaman Summon Storm", "Lynx Shaman Summoned Storm Movement Speed", 8.5f, "Lynx Shaman's Summoned Storm movement speed.");
            SummonStormCastTime = config.Bind("Lynx Shaman Summon Storm", "Lynx Shaman Summon Storm Cast Time", 4f, "Lynx Shaman's Summon Storm cast time.");
            SummonStormMinRange = config.Bind("Lynx Shaman Summon Storm", "Lynx Shaman Summon Storm Minimum Range", 10f, "Lynx Shaman's Summon Storm's minimum range of spawning from target.");
            SummonStormMaxRange = config.Bind("Lynx Shaman Summon Storm", "Lynx Shaman Summon Storm Maximum Range", 20f, "Lynx Shaman's Summon Storm's maximum range of spawning from target.");
            SummonStormCount = config.Bind("Lynx Shaman Summon Storm", "Lynx Shaman Summon Storm Count", 1, "Lynx Shaman's number of summoned storms in one cast.");
            SummonStormRechargeOnFailure = config.Bind("Lynx Shaman Summon Storm", "Lynx Shaman Summon Storm Refund on Failure", 0.75f, "Portion of Lynx Shaman's Summon Storm skill cooldown that will be refunded on summon failure.");
            SummonStormRadius = config.Bind("Lynx Shaman Summon Storm", "Lynx Shaman Summon Storm Radius", 20f, "Lynx Shaman's radius of summoned storms.");
            SummonStormPullStrength = config.Bind("Lynx Shaman Summon Storm", "Lynx Shaman Summon Storm Pull Strength", 5f, "Lynx Shaman's summoned storms pull streigth.");
            SummmonStormLifetime = config.Bind("Lynx Shaman Summon Storm", "Lynx Shaman Summon Storm Lifetime", 30f, "Lynx Shaman's summoned storms lifetime.");
            SummonStormGrabRange = config.Bind("Lynx Shaman Summon Storm", "Lynx Shaman Summon Storm Grab Range", 3f, "Lynx Shaman's summoned storms grab range.");
            SummonStormGrabDuration = config.Bind("Lynx Shaman Summon Storm", "Lynx Shaman Summon Storm Grab Duration", 4f, "Lynx Shaman's summoned storms grab duration. Basically for how long target stays in the air.");
            SummonStormThrowForce = config.Bind("Lynx Shaman Summon Storm", "Lynx Shaman Summon Storm Throw Force", 7500f, "Lynx Shaman's summoned storms throw force at the end of the grab.");
            SummonStormImmunityDuration = config.Bind("Lynx Shaman Summon Storm", "Lynx Shaman Summon Storm Immunity Duration", 5f, "For how long players are immune to getting succed by a storm after they got throwned out by it.");
        }

    }
}
