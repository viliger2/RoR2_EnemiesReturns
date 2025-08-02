using RoR2;
using static R2API.DamageAPI;

namespace EnemiesReturns
{
    public static class Content
    {
        public static class Stages
        {
            public static SceneDef OutOfTime;

            public static SceneDef JudgementOutro;
        }

        public static class MusicTracks
        {
            public static MusicTrackDef Unknown;
            public static MusicTrackDef UnknownBoss;
            public static MusicTrackDef TheOrigin;
        }

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

            public static ItemDef PartyHat;
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
