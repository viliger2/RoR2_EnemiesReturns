using RoR2;
using UnityEngine;

namespace EnemiesReturns.PrefabSetupComponents.MasterComponents
{
    public interface ISetDontDestroyOnLoad
    {
        protected bool NeedToAddSetDontDestroyOnLoad();

        protected SetDontDestroyOnLoad AddSetDontDestroyOnLoad(GameObject masterPrefab)
        {
            SetDontDestroyOnLoad dontDestoy = null;
            if (NeedToAddSetDontDestroyOnLoad())
            {
                var dontDestroy = masterPrefab.GetOrAddComponent<SetDontDestroyOnLoad>();
            }
            return dontDestoy;
        }
    }
}
