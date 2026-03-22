using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.CargoHoldDoors
{
    public class Closed : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            var childLocator = gameObject.GetComponent<ChildLocator>();
            if (childLocator)
            {
                var cargoDoor = childLocator.FindChild("CargoDoor");
                if (cargoDoor)
                {
                    var animator = cargoDoor.gameObject.GetComponent<Animator>();
                    if (animator)
                    {
                        PlayAnimationOnAnimator(animator, "Base", "Closed");
                    }
                }
            }
        }
    }
}
