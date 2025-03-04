using EnemiesReturns.Behaviors;
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
