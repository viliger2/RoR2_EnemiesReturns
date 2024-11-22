using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.ModelComponents.Skins
{
    public interface ICreateSkinDefs
    {
        protected SkinDef[] CreateSkinDefs(GameObject modelPrefab);
    }
}
