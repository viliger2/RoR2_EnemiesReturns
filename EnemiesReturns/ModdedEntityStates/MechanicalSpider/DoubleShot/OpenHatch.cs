using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot
{
    public class OpenHatch : BaseState
    {
        public static float baseDuration = 0.7f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayAnimation("Hatch", "OpenHatch", "Fire.playbackRate", duration);
            GetModelAnimator().SetBool("hatchOpen", true);
            Util.PlaySound("ER_Spider_Hatch_Open_Play", base.gameObject);
            //PlayAnimation("Hatch", "OpenHatch");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && fixedAge > duration)
            {
                outer.SetNextState(new ChargeFire());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
