using EnemiesReturns.Components.ModelComponents.Skins;
using UnityEngine;

namespace EnemiesReturns.Components.ModelComponents
{
    public interface ISkins : ICreateSkinDefs, IModelSkinController
    {
        public void AddSkins(GameObject modelPrefab)
        {
            var skinDefs = CreateSkinDefs(modelPrefab);
            AddModelSkinController(modelPrefab, skinDefs);
        }
    }
}
