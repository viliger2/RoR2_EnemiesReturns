using EnemiesReturns.Reflection;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Slide
{
    [RegisterEntityState]
    public class SlideBackwardState : BaseSlideState
    {
        public override void OnEnter()
        {
            slideRotation = Quaternion.AngleAxis(-180f, Vector3.up);
            base.OnEnter();
            PlayCrossfade("Gesture, Override", "DashBackwards", "Slide.playbackRate", BaseSlideState.duration, 0.05f);
        }
    }
}
