using BepInEx.Configuration;

namespace EnemiesReturns.Configuration.Judgement
{
    public class Judgement : IConfiguration
    {
        public static ConfigEntry<bool> Enabled;

        public static ConfigEntry<bool> EnableAnointedSkins;

        public static ConfigEntry<bool> ForceUnlock;

        public static ConfigEntry<string> JudgementEnemyBlacklist;
        public static ConfigEntry<bool> EulogyZeroSupport;

        public static ConfigEntry<float> MithrixHammerAeonianBonusDamage;
        public static ConfigEntry<float> MithrixHammerDamageCoefficient;
        public static ConfigEntry<float> MithrixHammerCooldown;

        public static ConfigEntry<bool> EnableCustomPhase3Music;

        public static ConfigEntry<float> AeonianEliteGoldMultiplier;
        public static ConfigEntry<float> AeonianEliteHealthMultiplier;
        public static ConfigEntry<float> AeonianEliteDamageMultiplier;
        public static ConfigEntry<float> AeonianEliteStunAndFreezeReduction;

        public static ConfigEntry<int> WavesTier1ItemMinCount;
        public static ConfigEntry<int> WavesTier1ItemMaxCount;

        public static ConfigEntry<int> WavesTier2ItemMinCount;
        public static ConfigEntry<int> WavesTier2ItemMaxCount;

        public static ConfigEntry<int> WavesTier3ItemMinCount;
        public static ConfigEntry<int> WavesTier3ItemMaxCount;

        public static ConfigEntry<int> WavesTierBossItemMinCount;
        public static ConfigEntry<int> WavesTierBossItemMaxCount;

        public void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Judgement", "Enabled", true, "Enables all content related to Judgement.");
            EnableAnointedSkins = config.Bind("Judgement", "Enable Anointed Skins", true, "Enables the ability to unlock Anointed skins.");
            ForceUnlock = config.Bind("Judgement", "Force Unlock Anointed Skins", false, "Force unlocks all Anointed skins by removing UnlockableDef from them.");
            JudgementEnemyBlacklist = config.Bind("Judgement", "Enemy Blacklist",
                "GeepMaster,GipMaster,GupMaster,ClayBruiserMaster,MinorConstructMaster,VoidMegaCrabMaster,LunarGolemMaster,LunarWispMaster,NullifierMaster,VoidJailerMaster,HalcyoniteMaster,LunarExploderMaster,VoidBarnacleMaster",
                "List of enemies that are blacklisted from appearing in Judgement. Requiers master names, you can get master names via DebugToolkit's list_ai command");

            MithrixHammerAeonianBonusDamage = config.Bind("Mithrix Hammer", "Mithrix Hammer Bonus Damage Against Aeonians", 500f, "Bonus damage multiplier against Aeonian elites.");
            MithrixHammerDamageCoefficient = config.Bind("Mithrix Hammer", "Mithrix Hammer Damage Coefficient", 30f, "Mithrix Hammer damage coefficient off base damage.");
            MithrixHammerCooldown = config.Bind("Mithrix Hammer", "Mithrix Hammer Cooldown", 15f, "Mithrix Hammer cooldown.");

            AeonianEliteGoldMultiplier = config.Bind("Aeonian Elites", "Gold Multiplier", 3f, "Gold and exp multiplier (from standard reward) of Aeonian elites in Judgement.");
            AeonianEliteHealthMultiplier = config.Bind("Aeonian Elites", "Health Multiplier", 18f, "Aeonian elite health multiplier. By default equal to that of T2 elites. Gets overwritten by EliteReworks.");
            AeonianEliteDamageMultiplier = config.Bind("Aeonian Elites", "Damage Multiplier", 6f, "Aeonian elite damage multiplier. By default equal to that of T2 elites. Gets overwritten by EliteReworks");
            AeonianEliteStunAndFreezeReduction = config.Bind("Aeonian Elites", "Stun and Freeze Duration Reduction", 0.5f, "Aeonian elite stun and freeze reduction, 0.5 will half the duration, 0 will remove the mechanic, it will not make it completely immune since it will still swap states and would get stunned for a few logic frames. Values above 1 will be ignored.");

            EnableCustomPhase3Music = config.Bind("Judgement", "Enable Custom Phase 3 Music", false, "Enables custom (as in not from Starstorm 1) music for Phase 3 (Phase 2 of boss fight)");

            WavesTier1ItemMinCount = config.Bind("Waves", "Tier 1 Item Min Count", 25, "Minimum number of Tier 1 items that will be given to monsters.");
            WavesTier1ItemMaxCount = config.Bind("Waves", "Tier 1 Item Max Count", 35, "Maximum number of Tier 1 items that will be given to monsters.");

            WavesTier2ItemMinCount = config.Bind("Waves", "Tier 2 Item Min Count", 10, "Minimum number of Tier 2 items that will be given to monsters.");
            WavesTier2ItemMaxCount = config.Bind("Waves", "Tier 2 Item Max Count", 15, "Maximum number of Tier 2 items that will be given to monsters.");

            WavesTier3ItemMinCount = config.Bind("Waves", "Tier 3 Item Min Count", 4, "Minimum number of Tier 3 items that will be given to monsters.");
            WavesTier3ItemMaxCount = config.Bind("Waves", "Tier 3 Item Max Count", 8, "Maximum number of Tier 3 items that will be given to monsters.");

            WavesTierBossItemMinCount = config.Bind("Waves", "Boss Tier Item Min Count", 8, "Minimum number of Boss Tier items that will be given to monsters.");
            WavesTierBossItemMaxCount = config.Bind("Waves", "Boss Tier Item Max Count", 10, "Maximum number of Boss Tier items that will be given to monsters.");

            EulogyZeroSupport = config.Bind("Waves", "Eulogy Zero Support", true, "Allows Eulogy Zero to replace items with lunars. Number of items given will between lowest number of Tier 3 to highest number of Tier 1.");
        }
    }
}
