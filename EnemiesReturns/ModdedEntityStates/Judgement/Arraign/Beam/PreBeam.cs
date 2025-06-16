using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Beam
{
    [RegisterEntityState]
    public class PreBeam : EntityState
    {
        public static float distanceToCenter = 170f;

        private Transform centerOfArena;

        public override void OnEnter()
        {
            base.OnEnter();
            if (isAuthority)
            {
                ChildLocator component = SceneInfo.instance.GetComponent<ChildLocator>();
                if ((bool)component)
                {
                    centerOfArena = component.FindChild("CenterOfArena");
                }
                if (!centerOfArena)
                {
                    outer.SetNextState(new BeamStart());
                    return;
                }

                if(Vector3.Distance(centerOfArena.position, transform.position) > distanceToCenter)
                {
                    FindSlideRotationAndSlideNextState();
                } else
                {
                    outer.SetNextState(new BeamStart());
                }
            }
        }

        private void FindSlideRotationAndSlideNextState()
        {
            string animationName = "DashForward";

            var directionToTarget = Vector3.Normalize(centerOfArena.position - transform.position);
            Vector3 forward = base.characterDirection.forward;
            Vector3 rhs = Vector3.Cross(Vector3.up, forward);
            float num = Vector3.Dot(directionToTarget, forward);
            float num2 = Vector3.Dot(directionToTarget, rhs);
            if (Mathf.Abs(num2) > Mathf.Abs(num))
            {
                if (num2 <= 0f)
                {
                    animationName = "DashLeft";
                }
                else
                {
                    animationName = "DashRight";
                }
            }
            else if (num <= 0f)
            {
                animationName = "DashBackwards";
            }
            else
            {
                animationName = "DashForward";
            }
            var angle = Vector3.SignedAngle(forward, directionToTarget, Vector3.up);
            outer.SetNextState(new SlideToCenter() { animationStateName = animationName, degreesToCenter = angle });
        }
    }
}
