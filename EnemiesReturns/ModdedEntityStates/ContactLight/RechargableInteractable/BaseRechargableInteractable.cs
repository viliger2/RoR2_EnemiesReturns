using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.RechargableInteractable
{
    public class BaseRechargableInteractable : BaseState
    {
        public Animator animator;

        public ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            childLocator = gameObject.GetComponent<ChildLocator>();
            if (childLocator)
            {
                var animatorObject = childLocator.FindChild("Animator");
                if (animatorObject)
                {
                    animator = animatorObject.GetComponent<Animator>();
                }
            }
        }

    }
}
