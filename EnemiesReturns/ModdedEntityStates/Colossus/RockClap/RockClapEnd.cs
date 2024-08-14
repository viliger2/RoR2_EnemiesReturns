using EnemiesReturns.Enemies.Colossus;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace EnemiesReturns.ModdedEntityStates.Colossus.RockClap
{
    public class RockClapEnd : BaseState
    {
        public static GameObject projectilePrefab;

        public static float baseDuration = 2.2f;

        public static float projectileDamageCoefficient = EnemiesReturnsConfiguration.Colossus.RockClapProjectileDamage.Value;

        public static float projectileForce = EnemiesReturnsConfiguration.Colossus.RockClapProjectileForce.Value;

        public static float projectileSpeed = EnemiesReturnsConfiguration.Colossus.RockClapProjectileSpeed.Value;

        public static float projectileSpeedDelta = EnemiesReturnsConfiguration.Colossus.RockClapProjectileSpeedDelta.Value;

        public static float projectileDistanceFraction = EnemiesReturnsConfiguration.Colossus.RockClapProjectileDistanceFraction.Value;

        public static float projectileDistanceFractionDelta = EnemiesReturnsConfiguration.Colossus.RockClapProjectileDistanceFractionDelta.Value;

        public static GameObject clapEffect;

        public static float clapDamageCoefficient = EnemiesReturnsConfiguration.Colossus.RockClapDamage.Value;

        public static float clapForce = EnemiesReturnsConfiguration.Colossus.RockClapForce.Value;

        public static float clapRadius = EnemiesReturnsConfiguration.Colossus.RockClapRadius.Value;

        private Animator modelAnimator;

        private float duration;

        private bool hasFired;

        private FloatingRocksController rockController;

        private Transform clapTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            modelAnimator = GetModelAnimator();
            rockController = GetModelTransform().gameObject.GetComponent<FloatingRocksController>();
            clapTransform = FindModelChild("ClapPoint");
            PlayCrossfade("Gesture, Override", "ClapEnd", "Clap.playbackrate", duration, 0.1f);
            Util.PlayAttackSpeedSound("ER_Colossus_Clap_Play", gameObject, attackSpeedStat);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (modelAnimator && modelAnimator.GetFloat("Clap.activate") >= 0.8f && !hasFired)
            {
                if (rockController)
                {
                    if (isAuthority)
                    {
                        foreach (GameObject rock in rockController.floatingRocks)
                        {
                            var position = (rock.transform.position - modelLocator.modelTransform.position) * UnityEngine.Random.Range(projectileDistanceFraction - projectileDistanceFractionDelta, projectileDistanceFraction + projectileDistanceFractionDelta);
                            position = new Vector3(modelLocator.modelTransform.position.x + position.x, modelLocator.modelTransform.position.y, modelLocator.modelTransform.position.z + position.z);

                            //var position = rock.transform.position - new Vector3(0f, 5f, 0f);
                            var rotation = Quaternion.LookRotation(rock.transform.position - position, Vector3.up);
                            ProjectileManager.instance.FireProjectile(projectilePrefab, rock.transform.position, rotation, gameObject, damageStat * projectileDamageCoefficient, projectileForce, RollCrit(), RoR2.DamageColorIndex.Default, null, UnityEngine.Random.Range(projectileSpeed - projectileSpeedDelta, projectileSpeed + projectileSpeedDelta));
                        }
                        var attack = new BlastAttack();
                        attack.attacker = gameObject;
                        attack.inflictor = gameObject;
                        attack.teamIndex = teamComponent.teamIndex;
                        attack.baseDamage = clapDamageCoefficient * damageStat;
                        attack.baseForce = clapForce;
                        attack.position = clapTransform.position;
                        attack.radius = clapRadius;
                        attack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                        attack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                        attack.Fire();

                    }
                    UnityEngine.Object.Instantiate(clapEffect, clapTransform.position, clapTransform.rotation);
                    rockController.enabled = false;
                }
                hasFired = true;
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
