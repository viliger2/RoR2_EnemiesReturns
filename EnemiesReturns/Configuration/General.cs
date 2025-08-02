using BepInEx.Configuration;

namespace EnemiesReturns.Configuration
{
    public static class General
    {
#if DEBUG || NOWEAVER
        public static ConfigEntry<float> DebugWalkSpeedValue;
        public static ConfigEntry<float> testconfig;
        public static ConfigEntry<ModdedEntityStates.Judgement.Mission.DebugIntiialState.InitialState> JudgementInitialState;
#endif
        public static ConfigEntry<bool> UseConfigFile;
        public static ConfigEntry<bool> SkipJudgementCutscene;
        public static ConfigEntry<PartyTime> PartyTimeConfig;

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
        }
    }
}
