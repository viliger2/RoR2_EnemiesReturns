using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.CargoHoldDoors
{
    [RegisterEntityState]
    public class Closing : BaseState
    {
        public static float baseDuration = 5f;

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
                        PlayAnimationOnAnimator(animator, "Base", "Closing");
                    }
                }

                var doorCollider = childLocator.FindChild("DoorCollider");
                if (doorCollider)
                {
                    doorCollider.gameObject.SetActive(true);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > baseDuration && isAuthority)
            {
                outer.SetNextState(new Closed());
            }
        }
    }
}
