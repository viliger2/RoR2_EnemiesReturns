using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.ModelComponents
{
    public interface IAimAnimator
    {
        protected class AimAnimatorParams
        {
            public float pitchRangeMin = -25f; // its looking up, not down, for fuck sake
            public float pitchRangeMax = 25f;

            public float yawRangeMin = -70f;
            public float yawRangeMax = 70f;

            public float pitchGiveUpRange = 20f;
            public float yawGiveUpRange = 20f;

            public float giveUpDuration = 3f;

            public float raisedApproachSpeed = 720f;
            public float loweredApproachSpeed = 360f;
            public float smoothTime = 0.1f;

            public bool fullYaw = false;
            public AimAnimator.AimType aimType = AimAnimator.AimType.Direct;

            public bool enableAimWeight = false;
            public bool UseTransformedAimVector = false;
        }

        protected bool NeedToAddAimAnimator();

        protected AimAnimatorParams GetAimAnimatorParams();

        protected AimAnimator AddAimAnimator(GameObject model, InputBankTest inputBank, CharacterDirection direction, AimAnimatorParams aimParams)
        {
            AimAnimator aimAnimator = null;
            if (NeedToAddAimAnimator())
            {
                aimAnimator = model.GetOrAddComponent<AimAnimator>();

                aimAnimator.inputBank = inputBank;
                aimAnimator.directionComponent = direction;
                aimAnimator.pitchRangeMin = aimParams.pitchRangeMin;
                aimAnimator.pitchRangeMax = aimParams.pitchRangeMax;
                aimAnimator.yawRangeMin = aimParams.yawRangeMin;
                aimAnimator.yawRangeMax = aimParams.yawRangeMax;
                aimAnimator.pitchGiveupRange = aimParams.pitchGiveUpRange;
                aimAnimator.yawGiveupRange = aimParams.yawGiveUpRange;
                aimAnimator.giveupDuration = aimParams.giveUpDuration;
                aimAnimator.raisedApproachSpeed = aimParams.raisedApproachSpeed;
                aimAnimator.loweredApproachSpeed = aimParams.loweredApproachSpeed;
                aimAnimator.smoothTime = aimParams.smoothTime;
                aimAnimator.fullYaw = aimParams.fullYaw;
                aimAnimator.aimType = aimParams.aimType;
                aimAnimator.enableAimWeight = aimParams.enableAimWeight;
                aimAnimator.UseTransformedAimVector = aimParams.UseTransformedAimVector;
            }
            return aimAnimator;
        }
    }
}
