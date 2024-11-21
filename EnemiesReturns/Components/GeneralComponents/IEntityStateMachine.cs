using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace EnemiesReturns.Components.GeneralComponents
{
    public interface IEntityStateMachine
    {
        protected class EntityStateMachineParams
        {
            public string name;
            public EntityStates.SerializableEntityStateType initialState;
            public EntityStates.SerializableEntityStateType mainState;
        }

        protected bool NeedToAddEntityStateMachines();

        protected EntityStateMachineParams[] GetEntityStateMachineParams();

        protected EntityStateMachine[] AddEntityStateMachines(GameObject bodyPrefab, EntityStateMachineParams[] esmParams)
        {
            List<EntityStateMachine> entities = new List<EntityStateMachine>();
            if (NeedToAddEntityStateMachines())
            {
                foreach (var esmParam in esmParams)
                {
                    var esm = bodyPrefab.AddComponent<EntityStateMachine>();
                    esm.customName = esmParam.name;
                    esm.initialStateType = esmParam.initialState;
                    esm.mainStateType = esmParam.mainState;
                    entities.Add(esm);
                }
            }

            return entities.ToArray();
        }
    }
}
