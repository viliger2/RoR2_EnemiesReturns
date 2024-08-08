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

        public static GameObject projectilePrefab = ColossusFactory.stompProjectile;

        public static GameObject stompEffectPrefab = ColossusFactory.stompEffect;
        //public static GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSunderWave.prefab").WaitForCompletion();

        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFXSlash.prefab").WaitForCompletion();

        public static float projectileDamageCoefficient = 3f;

        public static float stompDamageCoefficient = 6f;

        public static int projectilesCount = 16;

        public static float projectileForceMagnitude = -2500f; // its minus for beetle guard

        public static float stompForceMagnitude = 6000f;

        public static float speed = 60f;

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
            if (isAuthority && !hasFired)
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
