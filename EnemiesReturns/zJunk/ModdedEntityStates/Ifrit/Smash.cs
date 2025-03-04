using EntityStates;
using RoR2;
using System;
using UnityEngine;

namespace EnemiesReturns.Junk.ModdedEntityStates.Ifrit
{
    public class Smash : BaseState
    {
        public static float damageCoefficient => 4.5f;

        public static float forceMagnitude => 500f;

        public static float baseDuration = 1.5f;

        public static float baseAttackTime => baseDuration * 0.5939f; // magic number of attack connecting in animator

        public static float baseAttackStop => baseAttackTime + 0.2f; // magic numbers because who cares

        private OverlapAttack attack;

        private float duration;

        private float attackTime;

        private float attackStop;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            attackTime = baseAttackTime / attackSpeedStat;
            attackStop = baseAttackStop / attackSpeedStat;
            Transform modelTransform = GetModelTransform();

            attack = new OverlapAttack();
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.teamIndex = TeamComponent.GetObjectTeam(attack.attacker);
            attack.damage = damageCoefficient * damageStat;
            attack.isCrit = RollCrit();
            attack.damageType = DamageSource.Primary;
            if ((bool)modelTransform)
            {
                attack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Smash");
            }
            var result = UnityEngine.Random.Range(0, 100);
            if (result > 50)
            {
                PlayAnimation("Gesture, Additive", "SmashL", "Smash.playbackRate", duration);
            }
            else
            {
                PlayAnimation("Gesture, Additive", "SmashR", "Smash.playbackRate", duration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (fixedAge >= attackTime && fixedAge < attackStop)
                {
                    attack.forceVector = base.transform.forward * forceMagnitude;
                    attack.Fire();
                }
                if (fixedAge >= duration)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
