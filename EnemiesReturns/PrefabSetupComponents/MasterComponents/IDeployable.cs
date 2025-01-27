using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.PrefabSetupComponents.MasterComponents
{
    public interface IDeployable
    {
        protected bool NeedtoAddDeployable();

        internal Deployable AddDeployable(GameObject masterPrefab)
        {
            Deployable deployable = null;
            if (NeedtoAddDeployable())
            {
                deployable = masterPrefab.GetOrAddComponent<Deployable>();
            }
            return deployable;
        }

    }
}
