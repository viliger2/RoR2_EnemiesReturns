using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine
{
    public interface INetworkStateMachine
    {
        protected bool NeedToAddNetworkStateMachine();

        protected NetworkStateMachine AddNetworkStateMachine(GameObject bodyPrefab, params EntityStateMachine[] esms)
        {
            NetworkStateMachine networkStateMachine = null;
            if (NeedToAddNetworkStateMachine())
            {
                networkStateMachine = bodyPrefab.GetOrAddComponent<NetworkStateMachine>();
                networkStateMachine.stateMachines = esms;
            }

            return networkStateMachine;
        }
    }
}
