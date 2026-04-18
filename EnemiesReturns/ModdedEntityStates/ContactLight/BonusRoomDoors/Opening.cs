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
        public static Material openedMaterial;

        public override void OnEnter()
        {
            base.OnEnter();
            var childLocator = gameObject.GetComponent<ChildLocator>();
            if (!childLocator)
            {
                return;
            }

            var doorAnimator = childLocator.FindChild("DoorAnimator");
            if (doorAnimator)
            {
                var animator = doorAnimator.GetComponent<Animator>();
                if (animator)
                {
                    PlayAnimationOnAnimator(animator, "Base", "Opening");
                }
            }

            var panel = childLocator.FindChild("Panel1");
            if (panel)
            {
                var meshRenderer = panel.GetComponent<MeshRenderer>();
                if (meshRenderer && openedMaterial)
                {
                    meshRenderer.material = openedMaterial;
                }
            }

            panel = childLocator.FindChild("Panel2");
            if (panel)
            {
                var meshRenderer = panel.GetComponent<MeshRenderer>();
                if (meshRenderer && openedMaterial)
                {
                    meshRenderer.material = openedMaterial;
                }
            }
        }
    }
}
