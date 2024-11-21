using RoR2;
using System;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine
{
    public interface ISetStateOnHurt
    {
        protected class SetStateOnHurtParams
        {
            public SetStateOnHurtParams(string mainStateMachine, EntityStates.SerializableEntityStateType hurtState)
            {
                this.mainStateMachine = mainStateMachine;
                this.hurtState = hurtState;
            }

            public string mainStateMachine;
            public EntityStates.SerializableEntityStateType hurtState;
            public float hitThreshold = 0.3f;
            public bool canBeHitStunned = true;
            public bool canBeStunned = true;
            public bool canBeFrozen = true;
        }

        protected bool NeedToAddSetStateOnHurt();

        protected SetStateOnHurtParams GetSetStateOnHurtParams();

        protected SetStateOnHurt AddSetStateOnHurt(GameObject bodyPrefab, EntityStateMachine[] esms, SetStateOnHurtParams hurtParams)
        {
            SetStateOnHurt state = null;
            if (NeedToAddSetStateOnHurt())
            {
                var clonedArray = HG.ArrayUtils.Clone(esms);
                var targetStateMachine = clonedArray.First(item => item.customName == hurtParams.mainStateMachine);
                HG.ArrayUtils.ArrayRemoveAtAndResize(ref clonedArray, Array.IndexOf(clonedArray, targetStateMachine));

                state = bodyPrefab.GetOrAddComponent<SetStateOnHurt>();
                state.targetStateMachine = targetStateMachine;
                state.idleStateMachine = clonedArray;
                state.hurtState = hurtParams.hurtState;
                state.hitThreshold = hurtParams.hitThreshold;
                state.canBeFrozen = hurtParams.canBeFrozen;
                state.canBeHitStunned = hurtParams.canBeHitStunned;
                state.canBeStunned = hurtParams.canBeStunned;
            }

            return state;
        }
    }
}
