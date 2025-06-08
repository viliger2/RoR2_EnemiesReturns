using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2.LeapingDash
{
    [RegisterEntityState]
    public class LeapDash : BaseState
    {
        public static float damageCoefficient = 6f;

        public static float force = 0f;

        public static float procCoefficient = 1f;

        public static float blastAttackRadius = 20f;

        public static float upwardVelocity = 30f;

        public static float forwardVelocity = 80f;

        public static float minimumY = 0.05f;

        public static float aimVelocity = 20f;

        public static float airControl = 10f;

        public static float additionalGravity = 0f;

        public static string layerName = "Gesture, Override";

        public static string animationStateName = "SwordFlipBegin";

        public static GameObject blastAttackEffect;

        public static GameObject projectilePrefab;

        public static float distanceBetweenProjectiles = 13f;

        public static float projectileDamage = 4f;

        private Vector3 lastPosition;

        private Animator modelAnimator;

        private float previousAirControl;

        private bool detonateNextFrame;

        private bool liftedOff;

        public override void OnEnter()
        {
            base.OnEnter();
            previousAirControl = characterMotor.airControl;
            characterMotor.airControl = airControl;
            modelAnimator = GetModelAnimator();
            PlayCrossfade(layerName, animationStateName, 0.1f);
            lastPosition = transform.position;
        }

        public void SetNextStateAuthority()
        {
            outer.SetNextState(new LeapDashExit());
        }

        public override void OnExit()
        {
            if (isAuthority)
            {
                characterMotor.airControl = previousAirControl;
                characterMotor.onMovementHit -= OnMovementHit;
                characterMotor.moveDirection = new Vector3(0f, characterMotor.moveDirection.y, 0f);
                characterMotor.velocity = new Vector3(0f, characterMotor.velocity.y, 0f);
                characterDirection.moveVector = new Vector3(0f, characterDirection.moveVector.y, 0f);
            }
            PlayCrossfade(layerName, "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!characterMotor)
            {
                outer.SetNextStateToMain();
            }

            if (!isAuthority)
            {
                return;
            }

            if (liftedOff)
            {
                Vector3 direction = GetAimRay().direction;
                characterMotor.velocity += new Vector3(direction.x * aimVelocity, 0f, direction.z * aimVelocity);
                characterMotor.moveDirection = inputBank.moveVector;
                characterDirection.moveVector = characterMotor.velocity;
                characterMotor.disableAirControlUntilCollision = false;
                characterMotor.velocity.y += additionalGravity * GetDeltaTime();
                if (detonateNextFrame || characterMotor.Motor.GroundingStatus.IsStableOnGround && !characterMotor.Motor.LastGroundingStatus.IsStableOnGround)
                {
                    DetonateAuthority();
                    SetNextStateAuthority();
                }
            }

            if (modelAnimator.GetFloat("swordFlip.liftOffCurve") > 0.9f && !liftedOff)
            {
                Vector3 direction = GetAimRay().direction;
                if (isAuthority)
                {
                    characterBody.isSprinting = true;
                    direction.y = Mathf.Max(direction.y, minimumY);
                    //Vector3 vector = direction.normalized * aimVelocity * moveSpeedStat;
                    Vector3 vector2 = Vector3.up * upwardVelocity;
                    Vector3 vector3 = new Vector3(direction.x, 0f, direction.z).normalized * forwardVelocity;
                    characterMotor.Motor.ForceUnground();
                    characterMotor.velocity = vector2 + vector3;
                    characterMotor.onMovementHit += OnMovementHit;
                }
                if (NetworkServer.active)
                {
                    Util.CleanseBody(base.characterBody, removeDebuffs: true, removeBuffs: false, removeCooldownBuffs: false, removeDots: true, removeStun: false, removeNearbyProjectiles: false);
                }
                liftedOff = true;
            }

            if (!liftedOff)
            {
                return;
            }

            if (isAuthority)
            {
                Vector3 newPosition;
                if (characterMotor.Motor.GroundingStatus.IsStableOnGround)
                {
                    newPosition = characterBody.footPosition;
                } else if (Physics.Raycast(transform.position, Vector3.down, out var hitInfo, 10000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                {
                    newPosition = hitInfo.point;
                } else
                {
                    newPosition = transform.position;
                }

                if (Vector3.Distance(newPosition, lastPosition) > distanceBetweenProjectiles)
                {

                    var projectileInfo = new FireProjectileInfo()
                    {
                        crit = RollCrit(),
                        owner = base.gameObject,
                        position = newPosition,
                        projectilePrefab = projectilePrefab,
                        rotation = Quaternion.identity,
                        damage = damageStat * projectileDamage,
                        damageTypeOverride = DamageTypeCombo.GenericUtility
                    };

                    ProjectileManager.instance.FireProjectile(projectileInfo);
                    lastPosition = newPosition;
                }
            }
        }

        public BlastAttack.Result DetonateAuthority()
        {
            Vector3 footPosition = characterBody.footPosition;
            EffectManager.SpawnEffect(blastAttackEffect, new EffectData
            {
                origin = footPosition,
                scale = 7f
            }, transmit: true);
            return new BlastAttack
            {
                attacker = gameObject,
                baseDamage = damageStat * damageCoefficient,
                baseForce = force,
                bonusForce = new Vector3(0, force, 0),
                crit = RollCrit(),
                damageType = DamageTypeCombo.GenericUtility,
                falloffModel = BlastAttack.FalloffModel.None,
                procCoefficient = procCoefficient,
                radius = blastAttackRadius,
                position = footPosition,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                //impactEffect = EffectCatalog.FindEffectIndexFromPrefab(blastImpactEffectPrefab),
                teamIndex = teamComponent.teamIndex
            }.Fire();
        }

        public void OnMovementHit(ref CharacterMotor.MovementHitInfo movementHitInfo)
        {
            detonateNextFrame = true;
        }
    }
}
