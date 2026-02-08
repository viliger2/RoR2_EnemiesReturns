using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseSkulls
{
    public abstract class BasePrepareAttack : BaseState
    {
        public abstract float baseDuration { get; }

        public abstract string layerName { get; }

        public abstract string animationStateName { get; }

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade(layerName, animationStateName, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > baseDuration && isAuthority)
            {
                outer.SetNextState(GetNextState());
            }
        }

        public abstract EntityState GetNextState();

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade(layerName, "BufferEmpty", 0.1f);
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
