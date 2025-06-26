using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Slide
{
    [RegisterEntityState]
    public class SlideIntroState : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            bool flag = false;
            if ((bool)base.inputBank && base.isAuthority)
            {
                Vector3 normalized = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
                Vector3 forward = base.characterDirection.forward;
                Vector3 rhs = Vector3.Cross(Vector3.up, forward);
                float num = Vector3.Dot(normalized, forward);
                float num2 = Vector3.Dot(normalized, rhs);
                if ((bool)base.characterDirection)
                {
                    base.characterDirection.moveVector = base.inputBank.aimDirection;
                }
                if (Mathf.Abs(num2) > Mathf.Abs(num))
                {
                    if (num2 <= 0f)
                    {
                        flag = true;
                        outer.SetNextState(new SlideLeftState());
                    }
                    else
                    {
                        flag = true;
                        outer.SetNextState(new SlideRightState());
                    }
                }
                else if (num <= 0f)
                {
                    flag = true;
                    outer.SetNextState(new SlideBackwardState());
                }
                else
                {
                    flag = true;
                    outer.SetNextState(new SlideForwardState());
                }
            }
            if (!flag)
            {
                outer.SetNextState(new SlideForwardState());
            }
        }
    }
}
