using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface IHealthComponent
    {
        protected class HealthComponentParams
        {
            public bool dontShowHealthbar = false;
            public float globalDeathEventChance = 1f;
        }

        protected HealthComponentParams GetHealthComponentParams();

        protected bool NeedToAddHealthComponent();

        protected HealthComponent AddHealthComponent(GameObject bodyPrefab, HealthComponentParams healthComponentParams)
        {
            HealthComponent healthComponent = null;
            if (NeedToAddHealthComponent())
            {
                healthComponent = bodyPrefab.GetOrAddComponent<HealthComponent>();

                healthComponent.dontShowHealthbar = healthComponentParams.dontShowHealthbar;
                healthComponent.globalDeathEventChanceCoefficient = healthComponentParams.globalDeathEventChance;
            }

            return healthComponent;
        }

    }
}
