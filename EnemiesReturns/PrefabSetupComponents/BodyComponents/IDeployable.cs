using RoR2;
using UnityEngine;

namespace EnemiesReturns.PrefabSetupComponents.BodyComponents
{
    public interface IDeployable
    {
        protected bool NeedToAddDeployable();

        protected Deployable AddDeployable(GameObject bodyPrefab)
        {
            Deployable deployable = null;
            if (NeedToAddDeployable())
            {
                deployable = bodyPrefab.GetOrAddComponent<Deployable>();
            }
            return deployable;
        }
    }
}
