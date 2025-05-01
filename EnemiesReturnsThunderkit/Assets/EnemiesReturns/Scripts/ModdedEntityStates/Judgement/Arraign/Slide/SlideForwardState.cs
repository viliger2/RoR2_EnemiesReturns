using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Slide
{
    public class SlideForwardState : BaseSlideState
    {
        public override void OnEnter()
        {
            slideRotation = Quaternion.identity;
            base.OnEnter();
        }
    }
}
