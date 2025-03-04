using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration
{
    public interface IConfiguration
    {
        public void PopulateConfig(ConfigFile config);
    }
}
