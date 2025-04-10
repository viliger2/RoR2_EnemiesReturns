using EnemiesReturns.Behaviors;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.FlameCharge
{
    [RegisterEntityState]
    public class FlameChargeBegin : BaseState
    {
        public static float baseDuration = 1.8f;

        public static string attackString = "ER_Ifrit_BreathIn_Charge_Play";

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            var modelTransform = GetModelTransform();
            if (modelTransform && modelTransform.gameObject.TryGetComponent<TransformScaler>(out var transformScaler))
            {
                transformScaler.SetScaling(new UnityEngine.Vector3(1.25f, 1.25f, 1.25f), duration);
            }
            Util.PlayAttackSpeedSound(attackString, gameObject, attackSpeedStat);
            PlayCrossfade("Gesture,Override", "FlameBlastBegin", "FlameBlast.playbackRate", duration, 0.2f);

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new FlameCharge());
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
