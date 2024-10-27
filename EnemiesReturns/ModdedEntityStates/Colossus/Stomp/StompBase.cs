using EnemiesReturns.Configuration;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Stomp
{
    public abstract class StompBase : BaseState
    {
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFXSlash.prefab").WaitForCompletion();

        public static float projectileDamageCoefficient => EnemiesReturns.Configuration.Colossus.StompProjectileDamage.Value;

        public static float stompDamageCoefficient => EnemiesReturns.Configuration.Colossus.StompOverlapAttackDamage.Value;

        public static int projectilesCount => EnemiesReturns.Configuration.Colossus.StompProjectileCount.Value;

        public static float projectileForceMagnitude => EnemiesReturns.Configuration.Colossus.StompProjectileForce.Value;

        public static float stompForceMagnitude => EnemiesReturns.Configuration.Colossus.StompOverlapAttackForce.Value;

        public static float speed => EnemiesReturns.Configuration.Colossus.StompProjectileSpeed.Value;

        public static float baseDuration = 4.5f;

        public static GameObject projectilePrefab;

        public static GameObject stompEffectPrefab;

        internal abstract string animationStateName { get; }

        internal abstract string targetMuzzle { get; }

        internal abstract string hitBoxGroupName { get; }

        internal OverlapAttack attack;

        internal Transform modelTransform;

        private bool hasFired;

        private float duration;

        private Animator modelAnimator;

        private Transform projectileStart;

        public override void OnEnter()
        {
            base.OnEnter();
            projectilePrefab.transform.localScale = new Vector3(5f, 5f, 5f);
            modelAnimator = GetModelAnimator();
            modelTransform = GetModelTransform();
            attack = new OverlapAttack();
            attack.attacker = gameObject;
            attack.inflictor = gameObject;
            attack.teamIndex = teamComponent.teamIndex;
            attack.damage = stompDamageCoefficient * damageStat;
            attack.hitEffectPrefab = hitEffectPrefab;
            attack.forceVector = Vector3.up * stompForceMagnitude;

            projectileStart = FindModelChild(targetMuzzle);
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Gesture, Override", animationStateName, "Stomp.playbackrate", duration, 0.2f);
            if (characterBody)
            {
                characterBody.SetAimTimer(duration + 2f);
            }

            attack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), element => element.groupName == hitBoxGroupName);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (modelAnimator && modelAnimator.GetFloat("Stomp.activate") >= 0.8f)
            {
                Fire();
            }
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void Fire()
        {
            if (NetworkServer.active)
            {
                attack.Fire();
            }
            if (!hasFired)
            {
                if (isAuthority)
                {
                    float angle = 360f / projectilesCount;
                    for (int i = 0; i < projectilesCount; i++)
                    {
                        var forward = Quaternion.AngleAxis(angle * i, Vector3.up);
                        ProjectileManager.instance.FireProjectile(projectilePrefab, projectileStart.position, forward, gameObject, damageStat * projectileDamageCoefficient, projectileForceMagnitude, RollCrit(), DamageColorIndex.Default, null, speed);
                    }
                    EffectManager.SimpleMuzzleFlash(stompEffectPrefab, base.gameObject, targetMuzzle, transmit: true);
                }
                hasFired = true;
                Util.PlayAttackSpeedSound("ER_Colossus_Stomp_Play", gameObject, attackSpeedStat);
            }
        }

        public override void OnExit()
        {
            if (!hasFired)
            {
                Fire();
            }
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }


    }
}
