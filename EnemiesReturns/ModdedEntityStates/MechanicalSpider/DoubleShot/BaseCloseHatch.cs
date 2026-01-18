using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot
{
    [RegisterEntityState]
    public abstract class BaseCloseHatch : BaseState
    {
        public static float baseDuration = 0.7f;

        public abstract string closeHatchSound { get; }

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            GetModelAnimator().SetBool("hatchOpen", false);
            Util.PlaySound(closeHatchSound, gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
