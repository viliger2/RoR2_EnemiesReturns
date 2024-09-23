using EnemiesReturns.Helpers;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.FlameCharge
{
    public class FlameChargeEnd : BaseState
    {
        public static float baseDuration = 0.8f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / attackSpeedStat;
            duration = baseDuration / attackSpeedStat;
            var modelTransform = GetModelTransform();
            if (modelTransform && modelTransform.gameObject.TryGetComponent<TransformScaler>(out var transformScaler))
            {
                transformScaler.SetScaling(new UnityEngine.Vector3(1f, 1f, 1f), duration, new UnityEngine.Vector3(1.25f, 1.25f, 1.25f), true);
            }
            Util.PlaySound("Play_bison_charge_attack_end_skid", base.gameObject);
            PlayCrossfade("Gesture,Override", "FlameBlastEnd", "FlameBlast.playbackRate", duration, 0.2f);

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
