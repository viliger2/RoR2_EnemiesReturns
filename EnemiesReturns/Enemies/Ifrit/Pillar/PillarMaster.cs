using EnemiesReturns.Components;
using EnemiesReturns.Components.MasterComponents;
using System;
using UnityEngine;

namespace EnemiesReturns.Enemies.Ifrit.Pillar
{
    public class PillarMaster : MasterBase
    {
        public static GameObject AllyMasterPrefab;
        public static GameObject EnemyMasterPrefab;

        protected override bool AddAIOwnership => true;

        protected override bool AddDeployable => true;

        protected override IAISkillDriver.AISkillDriverParams[] AISkillDriverParams()
        {
            return Array.Empty<IAISkillDriver.AISkillDriverParams>();
        }
    }
}
