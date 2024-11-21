using EnemiesReturns.Components.ModelComponents.Hitboxes;
using UnityEngine;

namespace EnemiesReturns.Components.ModelComponents
{
    public interface IAddHitboxes : IHitboxes, IHitBoxGroup
    {
        public void SetupHitboxes(GameObject modelPrefab)
        {
            var hitboxes = AddHitboxes(modelPrefab, GetHitBoxesParams());
            AddHitBoxGroups(modelPrefab, hitboxes);
        }
    }
}
