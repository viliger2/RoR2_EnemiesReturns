using EnemiesReturns.Enemies.Colossus;
using EntityStates;
using Rewired;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Stomp
{
    public class StompBase : BaseState
    {
        public static float baseDuration = 4.5f;

        public static GameObject projectilePrefab;

        public static GameObject stompEffectPrefab;

        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFXSlash.prefab").WaitForCompletion();

        public static float projectileDamageCoefficient = EnemiesReturnsConfiguration.Colossus.StompProjectileDamage.Value;

        public static float stompDamageCoefficient = EnemiesReturnsConfiguration.Colossus.StompOverlapAttackDamage.Value;

        public static int projectilesCount = EnemiesReturnsConfiguration.Colossus.StompProjectileCount.Value;

        public static float projectileForceMagnitude = EnemiesReturnsConfiguration.Colossus.StompProjectileForce.Value; // its minus for beetle guard

        public static float stompForceMagnitude = EnemiesReturnsConfiguration.Colossus.StompOverlapAttackForce.Value;

        public static float speed = EnemiesReturnsConfiguration.Colossus.StompProjectileSpeed.Value;

        public string animationStateName;

        public string targetMuzzle;

        public OverlapAttack attack;

        public Transform modelTransform;

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
                    hasFired = true;
                }
                Util.PlayAttackSpeedSound("ER_Colossus_Stomp_Play", gameObject, attackSpeedStat);
            }
        }

        public override void OnExit()
        {
            if(!hasFired)
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
