using EnemiesReturns.Enemies.MechanicalSpider.Drone;
using UnityEngine;
using static RoR2.MasterCatalog;

namespace EnemiesReturns.Enemies.MechanicalSpider.Turret
{
    public class MechanicalSpiderTurretMaster : MechanicalSpiderDroneMaster
    {
        public static new GameObject MasterPrefab;

        public static MasterIndex MasterIndex;

        protected override bool AddSetDontDestoyOnLoad => false;
    }
}
