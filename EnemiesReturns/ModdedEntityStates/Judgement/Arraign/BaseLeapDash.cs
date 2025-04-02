using EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.LeapingDash;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign
{
    public abstract class BaseLeapDash : BaseState
    {
        public abstract float damageCoefficient { get; }

        public abstract float liftOffTimer { get; }

        public abstract float force { get; }

        public abstract float procCoefficient { get; }

        public abstract float blastAttackRadius { get; }

        public abstract float upwardVelocity { get; }

        public abstract float forwardVelocity { get; }

        public abstract float minimumY { get; }

        public abstract float aimVelocity { get; }

        public abstract float airControl { get; }

        public abstract float additionalGravity { get; }

        public abstract string layerName { get; }

        public abstract string animationStateName { get; }

        internal float previousAirControl;

        internal bool detonateNextFrame;

        internal bool liftedOff;

        public override void OnEnter()
        {
            base.OnEnter();
            previousAirControl = characterMotor.airControl;
            characterMotor.airControl = airControl;
            PlayCrossfade(layerName, animationStateName, 0.1f);
        }

        public override void OnExit()
        {
            characterMotor.airControl = previousAirControl;
            characterMotor.onMovementHit -= OnMovementHit;
            characterMotor.moveDirection = Vector3.zero;
            characterMotor.velocity = Vector3.zero;
            characterDirection.moveVector = Vector3.zero;
            PlayCrossfade(layerName, "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isAuthority || !characterMotor)
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
                    DoImpactAuthority();
                    SetNextStateAuthority();
                }
            }

            if (fixedAge >= liftOffTimer && !liftedOff)
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
                liftedOff = true;
            }
        }

        public abstract void SetNextStateAuthority();

        public virtual void DoImpactAuthority()
        {
            DetonateAuthority();
        }

        public BlastAttack.Result DetonateAuthority()
        {
            Vector3 footPosition = characterBody.footPosition;
            //EffectManager.SpawnEffect(blastEffectPrefab, new EffectData
            //{
            //    origin = footPosition,
            //    scale = blastRadius
            //}, transmit: true);
            return new BlastAttack
            {
                attacker = gameObject,
                baseDamage = damageStat * damageCoefficient,
                baseForce = force,
                bonusForce = new Vector3(0, force, 0),
                crit = RollCrit(),
                damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Utility),
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
