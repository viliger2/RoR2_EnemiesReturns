using BepInEx.Configuration;

namespace EnemiesReturns.Configuration
{
    public static class General
    {

        public static ConfigEntry<float> DebugWalkSpeedValue;
        public static ConfigEntry<float> testconfig;
        public static ConfigEntry<ModdedEntityStates.Judgement.Mission.DebugIntiialState.InitialState> JudgementInitialState;

        public static ConfigEntry<float> ProvidenceP1PrimaryPreSwingDuration;
        public static ConfigEntry<float> ProvidenceP1PrimaryProjectileTime;
        public static ConfigEntry<float> ProvidenceP1PrimarySwingDuration;

        public static ConfigEntry<float> ProvidenceP1SecondaryPreDuration;
        public static ConfigEntry<float> ProvidenceP1SecondaryDuration;
        public static ConfigEntry<float> ProvidenceP1SecondaryPostDuration;

        public static ConfigEntry<float> ProvidenceP1UtilityPreDuration;
        public static ConfigEntry<float> ProvidenceP1UtilityPredictionTimer;
        public static ConfigEntry<float> ProvidenceP1UtilityInvisibleDuration;
        public static ConfigEntry<float> ProvidenceP1UtilityAttackDuraion;
        public static ConfigEntry<float> ProvidenceP1UtilityEarlyExit;

        public static ConfigEntry<float> ProvidenceP1SpecialTeleportDuration;
        public static ConfigEntry<int> ProvidenceP1SpecialTimesToFire;
        public static ConfigEntry<int> ProvidenceP1SpecialRingsToFire;
        public static ConfigEntry<float> ProvidenceP1SpecialDelayBetweenRings;
        public static ConfigEntry<float> ProvidenceP1SpecialOneRingDuration;

        public static ConfigEntry<bool> UseConfigFile;
        public static ConfigEntry<bool> SkipJudgementCutscene;
        public static ConfigEntry<PartyTime> PartyTimeConfig;

        public static ConfigEntry<bool> EnableArcherBug;
        public static ConfigEntry<bool> EnableColossus;
        public static ConfigEntry<bool> EnableColossusItem;
        public static ConfigEntry<bool> EnableIfrit;
        public static ConfigEntry<bool> EnableIfritItem;
        public static ConfigEntry<bool> EnableSandCrab;
        public static ConfigEntry<bool> EnableSpitter;
        public static ConfigEntry<bool> EnableSwift;
        public static ConfigEntry<bool> EnableMechanicalSpider;
        public static ConfigEntry<bool> EnableLynxTotem;
        public static ConfigEntry<bool> EnableLynxTotemItem;
        public static ConfigEntry<bool> EnableLynxShaman;
        public static ConfigEntry<bool> EnableLynxShrine;
        public static ConfigEntry<bool> EnableLynxTrap;
        public static ConfigEntry<bool> EnableJudgement;

        public enum PartyTime
        {
            None,
            Default,
            AllYear
        }

        public static void PopulateConfig(ConfigFile config)
        {
#if DEBUG || NOWEAVER
            DebugWalkSpeedValue = config.Bind("Debug", "walkSpeed value", 1f, "Value speed for walkSpeed animation. For debugging.");
            testconfig = config.Bind("test", "test", 5f, "test");
            JudgementInitialState = config.Bind("Debug", "Judgement Initial State", ModdedEntityStates.Judgement.Mission.DebugIntiialState.InitialState.Phase1, "Initial state of Judgement.");

            ProvidenceP1PrimaryPreSwingDuration = config.Bind("Providence", "P1 Primary Pre Swing Duration", 0.5f);
            ProvidenceP1PrimaryProjectileTime = config.Bind("Providence", "P1 Primary Projectile Time", 0.5f);
            ProvidenceP1PrimarySwingDuration = config.Bind("Providence", "P1 Primary Swing Duration", 1f);

            ProvidenceP1SecondaryPreDuration = config.Bind("Providence", "P1 Secondary Pre Duration", 1.8f);
            ProvidenceP1SecondaryDuration = config.Bind("Providence", "P1 Secondary Duration", 0.75f);
            ProvidenceP1SecondaryPostDuration = config.Bind("Providence", "P1 Secondary Post Duration", 0.25f);

            ProvidenceP1UtilityPreDuration = config.Bind("Providence", "P1 Utility Pre Duration", 0.75f);
            ProvidenceP1UtilityPredictionTimer = config.Bind("Providence", "P1 Utility Prediction Timer", 0.75f);
            ProvidenceP1UtilityInvisibleDuration = config.Bind("Providence", "P1 Utility Invisible Duration", 1f);
            ProvidenceP1UtilityAttackDuraion = config.Bind("Providence", "P1 Utility Attack Duration", 3.5f);
            ProvidenceP1UtilityEarlyExit = config.Bind("Providence", "P1 Utility Early Exit Timer", 1.25f);

            ProvidenceP1SpecialTeleportDuration = config.Bind("Providence", "P1 Special Teleport Duration", 1f);
            ProvidenceP1SpecialTimesToFire = config.Bind("Providence", "P1 Special Times To Fire", 3);
            ProvidenceP1SpecialRingsToFire = config.Bind("Providence", "P1 Special Rings To Fire", 2);
            ProvidenceP1SpecialDelayBetweenRings = config.Bind("Providence", "P1 Special Delay Between Rings", 0.5f);
            ProvidenceP1SpecialOneRingDuration = config.Bind("Providence", "P1 Special One Ring Duration", 2f);
#endif
            UseConfigFile = config.Bind<bool>("Config", "Use Config File", false, "Use config file for storring config. Each enemy gets their own config file. Due to mod being currently unfinished and unbalanced, we deploy rapid changes to values. So this way we can still have configs, but without the issue of people having those values saved.");
            SkipJudgementCutscene = config.Bind("Judgement", "Skip Judgement Cutscene", false, "Automatically skips Judgement cutscene. Sadly currently there is no way to skip it and it crashes the game if you quit during it, so if you don't want to see it after a few runs enable this value.");
            PartyTimeConfig = config.Bind("Party Time", "Party Duration", PartyTime.Default, "By default party is only thrown on 8th of August. You can disable it or throw party all year long.");

            EnableArcherBug = config.Bind("Content", "Enable Archer Bug", true, "Enables Archer Bug to spawn.");
            EnableSandCrab = config.Bind("Content", "Enable Sand Crab", true, "Enables Sand Crab to spawn.");
            EnableSpitter = config.Bind("Content", "Enable Spitter", true, "Enables Spitter to spawn.");
            EnableSwift = config.Bind("Content", "Enable Swift", true, "Enables Swift to spawn.");
            EnableMechanicalSpider = config.Bind("Content", "Enable Mechanical Spider", true, "Enables Mechanical Spider to spawn.");

            EnableJudgement = config.Bind("Content", "Enable Judgement", true, "Enables content related to Judgement.");

            EnableColossus = config.Bind("Content", "Enable Colossus", true, "Enables Colossus to spawn.");
            EnableColossusItem = config.Bind("Content", "Enable Colossus Item (Colossal Fist)", true, "Enables Colossal Fist to drop from Colossus and appear in printers.");

            EnableIfrit = config.Bind("Content", "Enable Ifrit", true, "Enables Ifrit to spawn.");
            EnableIfritItem = config.Bind("Content", "Enable Ifrit Item (Infernal Lantern)", true, "Enables Infernal Lantern to drop from Ifrit and appear in printers.");

            EnableLynxTotem = config.Bind("Content", "Enable Lynx Totem", true, "Enables Lynx Totem to spawn.");
            EnableLynxTotemItem = config.Bind("Content", "Enable Lynx Totem Item (Lynx Fetish)", true, "Enables Lynx Fetish to drop from Lynx Totem and appear in printers. Item cannot be enabled without Totem since disabling Totem disables lesser Lynx tribe members.");
            EnableLynxShaman = config.Bind("Content", "Enable Lynx Shaman", true, "Enables Lynx Shaman to spawn.");
            EnableLynxShrine = config.Bind("Content", "Enable Lynx Shrine", true, "Enables Lynx Shrine. Has no effect if Lynx Totem is disabled.");
            EnableLynxTrap = config.Bind("Content", "Enable Lynx Trap", true, "Enables Lynx Trap. Has no effect is Lynx Totem is disabled.");

#pragma warning disable CS0618 // Type or member is obsolete
            Configuration.ArcherBug.Enabled = EnableArcherBug;
            Configuration.SandCrab.Enabled = EnableSandCrab;
            Configuration.Spitter.Enabled = EnableSpitter;
            Configuration.Swift.Enabled = EnableSwift;
            Configuration.MechanicalSpider.Enabled = EnableMechanicalSpider;
            Configuration.Judgement.Judgement.Enabled = EnableJudgement;
            Configuration.Colossus.Enabled = EnableColossus;
            Configuration.Colossus.ItemEnabled = EnableColossusItem;
            Configuration.Ifrit.Enabled = EnableIfrit;
            Configuration.Ifrit.ItemEnabled = EnableIfritItem;
            Configuration.LynxTribe.LynxTotem.Enabled = EnableLynxTotem;
            Configuration.LynxTribe.LynxTotem.ItemEnabled = EnableLynxTotemItem;
            Configuration.LynxTribe.LynxShaman.Enabled = EnableLynxShaman;
            Configuration.LynxTribe.LynxStuff.LynxShrineEnabled = EnableLynxShrine;
            Configuration.LynxTribe.LynxStuff.LynxTrapEnabled = EnableLynxTrap;
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
