using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface ICharacterDirection
    {
        protected bool NeedToAddCharacterDirection();

        protected float GetCharacterDirectionTurnSpeed();

        protected CharacterDirection AddCharacterDirection(GameObject bodyPrefab, Transform modelBase, Animator modelAnimator, float turnSpeed)
        {
            CharacterDirection direction = null;
            if (NeedToAddCharacterDirection())
            {
                direction = bodyPrefab.GetOrAddComponent<CharacterDirection>();
                direction.targetTransform = modelBase;
                direction.turnSpeed = turnSpeed;
                direction.modelAnimator = modelAnimator;
            }

            return direction;
        }

    }
}
