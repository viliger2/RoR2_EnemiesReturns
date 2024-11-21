using EnemiesReturns.Components.ModelComponents.Hurtboxes;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.ModelComponents
{
    public interface IHurtboxes : ISetupHurtboxes, IHurtBoxGroup
    {
        public void SetupHurtboxes(GameObject model, HealthComponent healthComponent)
        {
            var hurtBoxes = SetupHurtboxes(model, GetSurfaceDef(), healthComponent);
            AddHurtBoxGroup(model, hurtBoxes);
        }

    }
}
