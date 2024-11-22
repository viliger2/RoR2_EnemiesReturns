using RoR2;
using System;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine
{
    public interface ICharacterDeathBehavior
    {
        protected class CharacterDeathBehaviorParams
        {
            public CharacterDeathBehaviorParams(string mainMachine, EntityStates.SerializableEntityStateType deathState)
            {
                this.mainStateMachineName = mainMachine;
                this.deathState = deathState;
            }

            public string mainStateMachineName;
            public EntityStates.SerializableEntityStateType deathState;
        }

        protected bool NeedToAddCharacterDeathBehavior();

        protected CharacterDeathBehaviorParams GetCharacterDeathBehaviorParams();

        protected CharacterDeathBehavior AddCharacterDeathBehavior(GameObject bodyPrefab, EntityStateMachine[] stateMachines, CharacterDeathBehaviorParams characterDeathBehaviorParams)
        {
            CharacterDeathBehavior characterDeathBehavior = null;
            if (NeedToAddCharacterDeathBehavior())
            {
                // surely I didn't fuck up
                var clonedArray = HG.ArrayUtils.Clone(stateMachines);
                var deathStateMachine = clonedArray.First(item => item.customName == characterDeathBehaviorParams.mainStateMachineName);
                HG.ArrayUtils.ArrayRemoveAtAndResize(ref clonedArray, Array.IndexOf(clonedArray, deathStateMachine));

                characterDeathBehavior = bodyPrefab.GetOrAddComponent<CharacterDeathBehavior>();
                characterDeathBehavior.deathState = characterDeathBehaviorParams.deathState;
                characterDeathBehavior.deathStateMachine = deathStateMachine;
                characterDeathBehavior.idleStateMachine = clonedArray;
            }

            return characterDeathBehavior;
        }

    }
}
