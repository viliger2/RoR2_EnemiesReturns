using BepInEx.Configuration;

namespace EnemiesReturns.Configuration
{
    public interface IConfiguration
    {
        public void PopulateConfig(ConfigFile config);
    }
}
