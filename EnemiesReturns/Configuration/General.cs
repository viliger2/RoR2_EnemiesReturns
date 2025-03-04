using BepInEx.Configuration;

namespace EnemiesReturns.Configuration
{
    public static class General
    {
#if DEBUG || NOWEAVER
        public static ConfigEntry<float> DebugWalkSpeedValue;
        public static ConfigEntry<float> testconfig;
#endif

        public static void PopulateConfig(ConfigFile config)
        {
#if DEBUG || NOWEAVER
            DebugWalkSpeedValue = config.Bind("Debug", "walkSpeed value", 1f, "Value speed for walkSpeed animation. For debugging.");
            testconfig = config.Bind("test", "test", 5f, "test");
#endif
        }
    }
}
