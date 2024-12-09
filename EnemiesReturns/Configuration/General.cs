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

        public static ConfigEntry<ShamanTornadoBehavior> ShamanTornado;

        public static ConfigEntry<SummonProjectileType> ShamanSummonProjectileType;
#endif

        public static void PopulateConfig(ConfigFile config)
        {
#if DEBUG || NOWEAVER
            DebugWalkSpeedValue = config.Bind("Debug", "walkSpeed value", 1f, "Value speed for walkSpeed animation. For debugging.");
            testconfig = config.Bind("test", "test", 5f, "test");
            ShamanTornado = config.Bind("Shaman Tornado Debug", "Shaman Tornado Type", ShamanTornadoBehavior.SetVelocity, "Type of tornado behavior");
            ShamanSummonProjectileType = config.Bind("Shaman Summon Projectile Debug", "Shaman Summon Projectile Type", SummonProjectileType.Shotgun, "Type of summon projectile behavior");
#endif
        }
    }
}
