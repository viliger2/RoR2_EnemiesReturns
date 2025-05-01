using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Slide
{
    public class SlideBackwardState : BaseSlideState
    {
        public override void OnEnter()
        {
            slideRotation = Quaternion.AngleAxis(-180f, Vector3.up);
            base.OnEnter();
        }
    }
}
