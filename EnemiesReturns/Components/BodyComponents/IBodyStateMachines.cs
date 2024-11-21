using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.GeneralComponents;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface IBodyStateMachines : ICharacterDeathBehavior, IEntityStateMachine, INetworkStateMachine, ISetStateOnHurt
    {
        public void AddBodyStateMachines(GameObject bodyPrefab)
        {
            var esms = AddEntityStateMachines(bodyPrefab, GetEntityStateMachineParams());
            AddCharacterDeathBehavior(bodyPrefab, esms, GetCharacterDeathBehaviorParams());
            AddNetworkStateMachine(bodyPrefab, esms);
            AddSetStateOnHurt(bodyPrefab, esms, GetSetStateOnHurtParams());
        }

    }
}
