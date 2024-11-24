using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.GeneralComponents;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface IBodyStateMachines : ICharacterDeathBehavior, IEntityStateMachine, INetworkStateMachine, ISetStateOnHurt
    {
        public EntityStateMachine[] AddBodyStateMachines(GameObject bodyPrefab)
        {
            var esms = AddEntityStateMachines(bodyPrefab, GetEntityStateMachineParams());
            AddCharacterDeathBehavior(bodyPrefab, esms, GetCharacterDeathBehaviorParams());
            AddNetworkStateMachine(bodyPrefab, esms);
            AddSetStateOnHurt(bodyPrefab, esms, GetSetStateOnHurtParams());
            return esms;
        }

    }
}
