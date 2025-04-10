using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Hellzone
{
    [RegisterEntityState]
    public class FireHellzoneEnd : BaseState
    {
        public static float baseDuration = 2.2f;

        public static string attackString = "";

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / attackSpeedStat;
            Util.PlayAttackSpeedSound(attackString, gameObject, attackSpeedStat);
            PlayAnimation("Gesture,Override", "FireballEnd", "FireFireball.playbackRate", duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture,Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
