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

        public static void PopulateConfig(ConfigFile config)
        {
#if DEBUG || NOWEAVER
            DebugWalkSpeedValue = config.Bind("Debug", "walkSpeed value", 1f, "Value speed for walkSpeed animation. For debugging.");
            testconfig = config.Bind("test", "test", 5f, "test");
            JudgementInitialState = config.Bind("Debug", "Judgement Initial State", ModdedEntityStates.Judgement.Mission.DebugIntiialState.InitialState.Phase1, "Initial state of Judgement.");

#endif
        }
    }
}
