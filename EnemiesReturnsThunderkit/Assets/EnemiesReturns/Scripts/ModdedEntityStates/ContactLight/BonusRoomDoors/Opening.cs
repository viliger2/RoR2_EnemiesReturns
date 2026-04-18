using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.BonusRoomDoors
{
    [RegisterEntityState]
    public class Opening : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            var animator = GetComponent<Animator>();
            PlayAnimationOnAnimator(animator, "Base", "Opening");
        }
    }
}
