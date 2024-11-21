using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.MasterComponents
{
    public interface IMinionOwnership
    {
        protected bool NeedToAddMinionOwnership();

        protected MinionOwnership AddMinionOwnership(GameObject masterPrefab)
        {
            MinionOwnership ownership = null;
            if (NeedToAddMinionOwnership())
            {
                ownership = masterPrefab.GetOrAddComponent<MinionOwnership>();
            }
            return ownership;
        }

    }
}
