using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.CargoHoldDoors
{
    [RegisterEntityState]
    public class Opening : BaseState
    {
        public static Material matTerminalGreen;

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
                        PlayAnimationOnAnimator(animator, "Base", "Opening");
                    }
                }

                var doorCollider = childLocator.FindChild("DoorCollider");
                if (doorCollider)
                {
                    doorCollider.gameObject.SetActive(false);
                }

                if (matTerminalGreen) 
                {
                    var terminal1 = childLocator.FindChild("Terminal1");
                    if (terminal1)
                    {
                        var renderer = terminal1.GetComponent<Renderer>();
                        if (renderer)
                        {
                            renderer.material = matTerminalGreen;
                        }
                    }

                    var terminal2 = childLocator.FindChild("Terminal2");
                    if (terminal2)
                    {
                        var renderer = terminal2.GetComponent<Renderer>();
                        if (renderer)
                        {
                            renderer.material = matTerminalGreen;
                        }
                    }
                }
            }
        }
    }
}
