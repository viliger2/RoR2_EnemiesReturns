using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.ModelComponents
{
    public interface IAnimationEvents
    {
        protected bool NeedToAddAnimationEvents();

        internal AnimationEvents AddAnimationEvents(GameObject model)
        {
            AnimationEvents events = null;
            if (NeedToAddAnimationEvents())
            {
                events = model.GetOrAddComponent<AnimationEvents>();
            }
            return events;
        }
    }
}
