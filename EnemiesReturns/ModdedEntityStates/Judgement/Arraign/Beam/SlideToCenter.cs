using EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Slide;
using EnemiesReturns.Reflection;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Beam
{
    [RegisterEntityState]
    public class SlideToCenter : BaseSlideState
    {
        public static float distanceToCenter = 170f;

        public string animationStateName;

        public float degreesToCenter;

        public override void OnEnter()
        {
            slideRotation = Quaternion.AngleAxis(degreesToCenter, Vector3.up);
            base.OnEnter();
            PlayCrossfade("Gesture, Override", animationStateName, "Slide.playbackRate", BaseSlideState.duration, 0.05f);
        }

        public override void SetNextState()
        {
            ChildLocator component = SceneInfo.instance.GetComponent<ChildLocator>();
            if ((bool)component)
            {
                var centerOfArena = component.FindChild("CenterOfArena");
                if (centerOfArena)
                {
                    if (Vector3.Distance(centerOfArena.position, transform.position) > distanceToCenter)
                    {
                        var newDashState = new SlideToCenter();
                        newDashState.animationStateName = animationStateName;
                        newDashState.degreesToCenter = degreesToCenter;
                        outer.SetNextState(newDashState);
                        return;
                    }
                }
            }
            outer.SetNextState(new BeamStart());
        }
    }
}
