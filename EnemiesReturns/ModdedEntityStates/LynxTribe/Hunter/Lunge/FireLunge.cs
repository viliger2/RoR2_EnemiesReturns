using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Hunter.Lunge
{
    public class FireLunge : BaseState
    {
        public static float baseDuration = 0.8f;
        
        public static float damageCoefficient => EnemiesReturns.Configuration.LynxTribe.LynxHunter.StabDamage.Value;

        public static float procCoefficient => EnemiesReturns.Configuration.LynxTribe.LynxHunter.StabProcCoefficient.Value;

        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFX.prefab").WaitForCompletion(); // TODO: fine for now

        public static GameObject slideEffectPrefab;

        public static GameObject wooshEffect;

        public static GameObject coneEffect;

        public static float baseForceDuration = 0.7f;

        public static float baseAttackDuration = 0.4f;

        public static float maxLungeSpeedCoefficient = 3f;

        public static float maxLungeDistance = 20f;

        public static float turnSmoothTime = 0.1f;

        public static float turnSpeed = 40f;

        private float duration;

        private float attackDuration;

        private float calculatedLungeSpeed;

        private float forceDuration;

        private OverlapAttack overlapAttack;

        private Vector3 targetMoveVector;

        private Vector3 targetMoveVectorVelocity;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            attackDuration = baseAttackDuration / attackSpeedStat;
            forceDuration = baseForceDuration / attackSpeedStat;
            calculatedLungeSpeed = maxLungeSpeedCoefficient;

            var modelTransform = GetModelTransform();
            var hitboxes = modelTransform.GetComponents<HitBoxGroup>();
            var spearHitbox = Array.Find(hitboxes, (HitBoxGroup element) => element.groupName == "Spear");

            overlapAttack = new OverlapAttack();
            overlapAttack.attacker = base.gameObject;
            overlapAttack.inflictor = base.gameObject;
            overlapAttack.teamIndex = GetTeam();
            overlapAttack.damage = damageCoefficient * damageStat;
            overlapAttack.isCrit = RollCrit();
            overlapAttack.hitBoxGroup = spearHitbox;
            overlapAttack.hitEffectPrefab = hitEffectPrefab;
            overlapAttack.procCoefficient = procCoefficient;
            overlapAttack.damageType = DamageType.Generic;

            PlayAnimation("Gesture, Override", "LungeFire", "Attack.playbackRate", duration);

            Util.PlayAttackSpeedSound("ER_Hunter_FireLunge_Play", gameObject, attackSpeedStat);

            EffectManager.SpawnEffect(wooshEffect, new EffectData
            {
                rootObject = base.gameObject,
                modelChildIndex = (short)GetModelChildLocator().FindChildIndex("Weapon")
            }, false);

            EffectManager.SpawnEffect(coneEffect, new EffectData
            {
                rootObject = base.gameObject,
                modelChildIndex = (short)GetModelChildLocator().FindChildIndex("SpearTip")
            }, false);

            EffectManager.SpawnEffect(slideEffectPrefab, new EffectData
            {
                rootObject = base.gameObject,
                modelChildIndex = (short)GetModelChildLocator().FindChildIndex("SlideEffect")
            }, false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority) {

                if(fixedAge < forceDuration)
                {
                    targetMoveVector = Vector3.ProjectOnPlane(Vector3.SmoothDamp(targetMoveVector, base.inputBank.aimDirection, ref targetMoveVectorVelocity, turnSmoothTime, turnSpeed), Vector3.up).normalized;
                    base.characterDirection.moveVector = targetMoveVector;
                    base.characterMotor.rootMotion += calculatedLungeSpeed * moveSpeedStat * base.characterDirection.forward * GetDeltaTime();
                }

                if (fixedAge < attackDuration)
                {
                    overlapAttack.Fire();
                }

                if (fixedAge > duration)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
