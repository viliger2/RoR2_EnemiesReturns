using EnemiesReturns.Reflection;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Slide
{
    [RegisterEntityState]
    public class SlideForwardState : BaseSlideState
    {
        public override void OnEnter()
        {
            slideRotation = Quaternion.identity;
            base.OnEnter();
            PlayCrossfade("Gesture, Override", "DashForward", "Slide.playbackRate", BaseSlideState.duration, 0.05f);
        }
    }
}
