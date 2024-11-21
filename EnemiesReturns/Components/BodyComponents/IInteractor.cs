using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface IInteractor
    {
        protected bool NeedToAddInteractor();

        protected float GetInteractionDistance();

        internal Interactor AddInteractor(GameObject bodyPrefab, float interactionDistance)
        {
            Interactor interactor = null;
            if (NeedToAddInteractor())
            {
                interactor = bodyPrefab.GetOrAddComponent<Interactor>();
                interactor.maxInteractionDistance = interactionDistance;
            }

            return interactor;
        }
    }
}
