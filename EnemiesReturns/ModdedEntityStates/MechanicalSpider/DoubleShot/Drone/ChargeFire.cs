using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot.Drone
{
    [RegisterEntityState]
    public class ChargeFire : BaseChargeFire
    {
        public override string soundString => "ER_Spider_Fire_Charge_Drone_Play";

        public override EntityState GetNextState()
        {
            return new Fire();
        }
    }
}
