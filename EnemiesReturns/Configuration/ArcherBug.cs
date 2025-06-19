using BepInEx.Configuration;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration
{
    internal class ArcherBug : IConfiguration
    {
        public static ConfigEntry<string> DefaultStageList;
        public void PopulateConfig(ConfigFile config)
        {
            ArcherBug.DefaultStageList = config.Bind("Archer Bug Director", "Default Variant Stage List",
                string.Join(
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.AphelianSanctuary),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.AphelianSanctuarySimulacrum),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.TreebornColony),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.GoldenDieback),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.AbandonedAqueduct),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.AbandonedAqueductSimulacrum),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell)
                    ),
                "Stages that Default Archer Bugs appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");
        }
    }
}
