using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using static R2API.DamageAPI;

namespace EnemiesReturns
{
    public static class Content
    {
        public static class GameEndings
        {
            public static GameEndingDef SurviveJudgement;
        }

        public static class ItemTiers
        {
            public static ItemTierDef HiddenInLogbook;
        }

        public static class Items
        {
            public static ItemDef ColossalCurl;

            public static ItemDef SpawnPillarOnChampionKill;

            public static ItemDef LynxFetish;

            public static ItemDef TradableRock;

            public static ItemDef LunarFlower;

            public static ItemDef HiddenAnointed;
        }

        public static class ItemRelationshipProviders
        {
            public static ItemRelationshipProvider ModdedContagiousItemProvider;
        }

        public static class Equipment
        {
            public static EquipmentDef MithrixHammer;

            public static EquipmentDef EliteAeonian;
        }

        public static class Buffs
        {
            public static BuffDef LynxArcherDamage;

            public static BuffDef LynxHunterArmor;

            public static BuffDef LynxScoutSpeed;

            public static BuffDef LynxShamanSpecialDamage;

            public static BuffDef ReduceHealing;

            public static BuffDef LynxStormImmunity;

            public static BuffDef AffixAeoninan;

            public static BuffDef ImmuneToAllDamageExceptHammer;

            public static BuffDef ImmuneToHammer;
        }

        public static class Elites
        {
            public static EliteDef Aeonian;
        }

        public static class DamageTypes
        {
            public static ModdedDamageType ApplyReducedHealing;

            public static ModdedDamageType EndGameBossWeapon;
        }
    }
}
