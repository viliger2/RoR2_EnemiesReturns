using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface IInteractionDriver
    {
        protected bool NeedToAddInteractionDriver();

        protected InteractionDriver AddInteractionDriver(GameObject bodyPrefab)
        {
            InteractionDriver driver = null;
            if (NeedToAddInteractionDriver())
            {
                driver = bodyPrefab.GetOrAddComponent<InteractionDriver>();
            }
            return driver;
        }
    }
}
