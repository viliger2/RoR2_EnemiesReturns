using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2
{
    [RegisterEntityState]
    public class DeathState : GenericCharacterDeath
    {
        public override void OnEnter()
        {
            bodyPreservationDuration = 20f;
            base.OnEnter();
        }
    }
}
