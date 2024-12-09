using EnemiesReturns.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.PrefabSetupComponents.ModelComponents
{
    public interface IRemoveJitterBones
    {
        protected bool NeedToAddRemoveJitterBones();

        protected RemoveJitterBones AddRemoveJitterBones(GameObject model)
        {
            RemoveJitterBones rmb = null;
            if (NeedToAddRemoveJitterBones())
            {
                rmb = model.AddComponent<RemoveJitterBones>();
            }
            return rmb;
        }

    }
}
