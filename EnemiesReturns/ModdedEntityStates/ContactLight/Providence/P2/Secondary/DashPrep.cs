using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Secondary
{
    [RegisterEntityState]
    public class DashPrep : BaseState
    {
        public static float baseDuration => 0.583f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayAnimation("Gesture, Override", "DashPrep", "combo.playbackRate", duration);
            Util.PlaySound("ER_Providence_DashPrep_Play", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            characterDirection.forward = GetAimRay().direction.normalized;

            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextState(new DashAttack());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
