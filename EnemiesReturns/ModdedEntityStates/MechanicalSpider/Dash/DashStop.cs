using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.Dash
{
    public class DashStop : BaseState
    {
        public static float duration = 0.8f;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Body", "DashStop", 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && fixedAge >= duration)
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
