using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.SandCrab.Snip
{
    [RegisterEntityState]
    public class FireSnip : BaseState
    {
        public static float damageCoefficient => 5f;

        public static float forceMagnitude => 200f;

        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFXSlash.prefab").WaitForCompletion();

        public static float baseDuration = 1.458f;

        public static GameObject snipEffectPrefab;

        private OverlapAttack attack;

        private Animator modelAnimator;

        private float duration;

        private bool hasSnipped;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            modelAnimator = GetModelAnimator();
            Transform modelTransform = GetModelTransform();
            attack = new OverlapAttack();
            attack.attacker = gameObject;
            attack.inflictor = gameObject;
            attack.teamIndex = TeamComponent.GetObjectTeam(attack.attacker);
            attack.damage = damageCoefficient * damageStat;
            attack.hitEffectPrefab = hitEffectPrefab;
            attack.isCrit = RollCrit();
            attack.damageType = DamageSource.Primary;
            if ((bool)modelTransform)
            {
                attack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (element) => element.groupName == "Snip");
            }
            PlayAnimation("Gesture, Override, Mask", "FireSnip", "FireSnip.playbackRate", duration);
            if ((bool)characterBody)
            {
                characterBody.SetAimTimer(2f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (modelAnimator && modelAnimator.GetFloat("Snip.hitBoxActive") > 0.9f)
            {
                if (!hasSnipped)
                {
                    EffectManager.SimpleMuzzleFlash(snipEffectPrefab, gameObject, "SnipSpot", false);
                    hasSnipped = true;
                }
                if (isAuthority)
                {
                    attack.forceVector = transform.forward * forceMagnitude;
                    attack.Fire();
                }
            }
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override, Mask", "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
