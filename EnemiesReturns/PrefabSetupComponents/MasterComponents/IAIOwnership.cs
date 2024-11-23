using RoR2.CharacterAI;
using UnityEngine;

namespace EnemiesReturns.PrefabSetupComponents.MasterComponents
{
    public interface IAIOwnership
    {
        protected bool NeedToAddAIOwnership();

        protected AIOwnership AddAIOwnership(GameObject master)
        {
            AIOwnership aIOwnership = null;
            if (NeedToAddAIOwnership())
            {
                aIOwnership = master.GetOrAddComponent<AIOwnership>();
            }
            return aIOwnership;
        }
    }
}
