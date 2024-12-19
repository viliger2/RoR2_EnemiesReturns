using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.BlastAttack;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    public class PushBack : BaseState
    {
        public static float baseDuration = 1.5f;

        public static float baseAttackDelay = 1f;

        public static float radius = 6f;

        public static float damage = 1f;

        public static float force = 1200f;

        private float duration;

        private float attackDelay;

        private BlastAttack blastAttack;

        private bool isAttackFired;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            attackDelay = baseAttackDelay / attackSpeedStat;
            PrepareAttack();
            PlayCrossfade("Gesture, Override", "CastTeleportFull", "CastTeleport.playbackRate", duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > attackDelay && !isAttackFired && NetworkServer.active)
            {
                blastAttack.position = base.transform.position;
                var result = blastAttack.Fire();
                for (int i = 0; i < result.hitPoints.Length; i++)
                {
                    Vector3 direction = (result.hitPoints[i].hitPosition - transform.position) * force;
                    result.hitPoints[i].hurtBox.healthComponent.TakeDamageForce(direction, true, false);
                }

                isAttackFired = true;
            }

            if(fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        private void PrepareAttack()
        {
            blastAttack = new BlastAttack();
            blastAttack.radius = radius;
            blastAttack.procCoefficient = 0f;
            blastAttack.attacker = characterBody.gameObject;
            blastAttack.crit = RollCrit();
            blastAttack.baseDamage = damage * damageStat;
            blastAttack.canRejectForce = false;
            blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            //blastAttack.baseForce = force;
            //blastAttack.bonusForce = force;
            blastAttack.teamIndex = characterBody.teamComponent.teamIndex;
            blastAttack.attackerFiltering = AttackerFiltering.Default;
            blastAttack.Fire();
        }
    }
}
