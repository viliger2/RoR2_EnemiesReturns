using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class OnEnableESMStateSetter : MonoBehaviour
    {
        [Serializable]
        public struct Objects
        {
            public EntityStateMachine esm;
            public SerializableEntityStateType stateToSet;
        }

        public Objects[] objects;

        private void OnEnable()
        {
            if(objects == null)
            {
                return;
            }

            for(int i = 0; i < objects.Length;i++)
            {
                var obj = objects[i];

                if (!obj.esm)
                {
                    continue;
                }

                if (!obj.esm.networker)
                {
                    // ESM doesn't have network identity, so we are just setting the state as is
                    obj.esm.SetNextState(EntityStateCatalog.InstantiateState(ref obj.stateToSet));
                } else
                {
                    if (Util.HasEffectiveAuthority(obj.esm.networker.gameObject))
                    {
                        obj.esm.SetNextState(EntityStateCatalog.InstantiateState(ref obj.stateToSet));
                    }
                }
            }
        }

    }
}
