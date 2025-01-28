using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Junk.ModdedEntityStates.LynxTribe.Hunter
{
    public class Stab : BaseState
    {
        public static float baseDuration => 2f;

        public static float damageCoefficient => Configuration.LynxTribe.LynxHunter.StabDamage.Value;

        public static float procCoefficient => Configuration.LynxTribe.LynxHunter.StabProcCoefficient.Value;

        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFX.prefab").WaitForCompletion();

        public static GameObject wooshEffect;

        public static GameObject coneEffect;

        private static readonly int strikeHash = Animator.StringToHash("Attack.Strike");

        private float duration;

        private OverlapAttack overlapAttack;

        private Animator animator;

        private bool effectsSpawned;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;

            animator = GetModelAnimator();

            var modelTransform = GetModelTransform();
            var hitboxes = modelTransform.GetComponents<HitBoxGroup>();
            var spearHitbox = Array.Find(hitboxes, (element) => element.groupName == "Spear");

            overlapAttack = new OverlapAttack();
            overlapAttack.attacker = gameObject;
            overlapAttack.inflictor = gameObject;
            overlapAttack.teamIndex = GetTeam();
            overlapAttack.damage = damageCoefficient * damageStat;
            overlapAttack.isCrit = RollCrit();
            overlapAttack.hitBoxGroup = spearHitbox;
            overlapAttack.hitEffectPrefab = hitEffectPrefab;
            overlapAttack.procCoefficient = procCoefficient;
            overlapAttack.damageType = DamageType.Generic;

            PlayAnimation("Gesture", "Attack", "Attack.playbackRate", duration);
            if (characterBody)
            {
                characterBody.SetAimTimer(3f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (animator && animator.GetFloat(strikeHash) > 0.1f)
            {
                if (isAuthority)
                {
                    overlapAttack.Fire();
                }

                if (!effectsSpawned)
                {
                    EffectManager.SpawnEffect(wooshEffect, new EffectData
                    {
                        rootObject = gameObject,
                        modelChildIndex = (short)GetModelChildLocator().FindChildIndex("Weapon")
                    }, false);

                    EffectManager.SpawnEffect(coneEffect, new EffectData
                    {
                        rootObject = gameObject,
                        modelChildIndex = (short)GetModelChildLocator().FindChildIndex("SpearTip")
                    }, false);

                    effectsSpawned = true;
                }
            }

            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture", "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
