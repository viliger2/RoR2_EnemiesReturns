using BepInEx.Configuration;

namespace EnemiesReturns.Configuration
{
    public static class General
    {
        public static ConfigEntry<float> DebugWalkSpeedValue;
        public static ConfigEntry<float> testconfig;
        public static ConfigEntry<ModdedEntityStates.Judgement.Mission.DebugIntiialState.InitialState> JudgementInitialState;

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

        public static ConfigEntry<bool> EnableContactLight;
        public static ConfigEntry<bool> EnableAdrenalineCore;

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

            EnableContactLight = config.Bind("Content", "Enable Contact Light", true, "Enables content related to Contact Light.");
            EnableAdrenalineCore = config.Bind("Content", "Enable Adrenaline Core", true, "Enables Adrenaline Core item.");

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
