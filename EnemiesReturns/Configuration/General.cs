using BepInEx.Configuration;

namespace EnemiesReturns.Configuration
{
    public static class General
    {
#if DEBUG || NOWEAVER
        public enum ShamanTornadoBehavior
        {
            SetPosition,
            AddDisplacement,
            SetVelocity
        }

        public enum SummonProjectileType
        {
            Shotgun,
            RapidFire
        }

        public static ConfigEntry<float> DebugWalkSpeedValue;

        public static ConfigEntry<float> testconfig;

        public static ConfigEntry<bool> ShamanVoices;
#endif

        public static void PopulateConfig(ConfigFile config)
        {
#if DEBUG || NOWEAVER
            DebugWalkSpeedValue = config.Bind("Debug", "walkSpeed value", 1f, "Value speed for walkSpeed animation. For debugging.");
            testconfig = config.Bind("test", "test", 5f, "test");
            ShamanVoices = config.Bind("Shaman Voices", "Shaman has a voice", false, "Crash Bandicoot");
#endif
        }
    }
}
