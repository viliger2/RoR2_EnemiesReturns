using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Scout
{
    public class DoubleSlash : BaseState
    {
        public static float baseDuration => 1.8f;

        public static float damageCoefficient => EnemiesReturns.Configuration.LynxTribe.LynxScout.DoubleSlashDamage.Value;

        public static float procCoefficient => EnemiesReturns.Configuration.LynxTribe.LynxScout.DoubleSlashProcCoefficient.Value;

        public static string LeftHitbox = "LeftSlash";

        public static string RightHitbox = "RightSlash";

        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFXSlash.prefab").WaitForCompletion(); // TODO: fine for now

        public static GameObject clawEffectLeft;

        public static GameObject clawEffectRight;

        public static GameObject slashEffectLeft;

        public static GameObject slashEffectRight;

        private float duration;

        private Animator animator;

        private OverlapAttack lefOverlapAttack;

        private OverlapAttack rightOverlapAttack;

        private static int LeftSlashHash = Animator.StringToHash("Attack.LeftSlashStrike");

        private static int RightSlashHash = Animator.StringToHash("Attack.RightSlashStrike");

        private bool spawnedEffectLeft;

        private bool spawnedEffectRight;

        private bool spawnedEffectRightSlash;

        private ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            animator = GetModelAnimator();
            var modelTransform = GetModelTransform();
            var hitboxes = modelTransform.GetComponents<HitBoxGroup>();
            Util.PlayAttackSpeedSound("ER_Scout_Attack_Play", gameObject, attackSpeedStat);

            lefOverlapAttack = SetupAttack(Array.Find(hitboxes, (HitBoxGroup element) => element.groupName == LeftHitbox));
            rightOverlapAttack = SetupAttack(Array.Find(hitboxes, (HitBoxGroup element) => element.groupName == RightHitbox));

            childLocator = GetModelChildLocator();

            PlayAnimation("Gesture, Mask", "Attack", "Attack.playbackRate", duration);

            if ((bool)base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(animator && animator.GetFloat(LeftSlashHash) > 0.1f)
            {
                if (isAuthority)
                {
                    lefOverlapAttack.Fire();
                }
                if (!spawnedEffectLeft)
                {
                    if (clawEffectLeft) {
                        EffectManager.SpawnEffect(clawEffectLeft, new EffectData
                        {
                            rootObject = base.gameObject,
                            modelChildIndex = (short)childLocator.FindChildIndex("HandL")
                        }, false);
                    }

                    if (slashEffectLeft) {
                        EffectManager.SimpleMuzzleFlash(slashEffectLeft, base.gameObject, "LeftSwingEffect", false);
                    }

                    spawnedEffectLeft = true;
                }
            }

            if(animator && animator.GetFloat(RightSlashHash) > 0.1f)
            {
                if(isAuthority)
                {
                    rightOverlapAttack.Fire();
                }
                if (!spawnedEffectRight)
                {
                    if (clawEffectRight)
                    {
                        EffectManager.SpawnEffect(clawEffectRight, new EffectData
                        {
                            rootObject = base.gameObject,
                            modelChildIndex = (short)childLocator.FindChildIndex("HandR")
                        }, false);
                        spawnedEffectRight = true;
                    }
                }
                if (!spawnedEffectRightSlash && animator.GetFloat(RightSlashHash) > 0.9f)
                {
                    EffectManager.SimpleMuzzleFlash(slashEffectRight, base.gameObject, "RightSwingEffect", false);
                    spawnedEffectRightSlash = true;
                }
            }

            if(fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Mask", "BufferEmpty", 0.1f);
        }

        private OverlapAttack SetupAttack(HitBoxGroup hitBoxGroup)
        {
            var attack = new OverlapAttack();
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = damageCoefficient * damageStat; // TODO: maybe separate damage for slashes?
            attack.isCrit = RollCrit();
            attack.hitBoxGroup = hitBoxGroup;
            attack.hitEffectPrefab = hitEffectPrefab;
            attack.procCoefficient = procCoefficient;
            attack.damageType = DamageSource.Primary;

            return attack;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
