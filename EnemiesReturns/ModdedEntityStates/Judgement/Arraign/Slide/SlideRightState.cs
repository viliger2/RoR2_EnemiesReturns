using EnemiesReturns.Reflection;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Slide
{
    [RegisterEntityState]
    public class SlideRightState : BaseSlideState
    {
        public override void OnEnter()
        {
            slideRotation = Quaternion.AngleAxis(90f, Vector3.up);
            base.OnEnter();
            PlayCrossfade("Gesture, Override", "DashRight", "Slide.playbackRate", BaseSlideState.duration, 0.05f);
        }
    }
}
