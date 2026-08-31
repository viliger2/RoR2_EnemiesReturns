using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseSkulls;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3.Secondary
{

    [RegisterEntityState]
    public class SkullsOutro : BaseSkullOutro
    {
        public override float baseDuration => 1.5f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "SummonSkullsExit";

        public override EntityState GetNextState()
        {
            return EntityStateCatalog.InstantiateState(ref outer.mainStateType);
        }
    }
}
