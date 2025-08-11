using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.SandCrab
{
    internal class FireSnip : BaseState
    {
        public static float damageCoefficient => 2.5f;

        public static float forceMagnitude => 200f;

        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFXSlash.prefab").WaitForCompletion();

        public static float baseDuration = 1f;

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
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.teamIndex = TeamComponent.GetObjectTeam(attack.attacker);
            attack.damage = damageCoefficient * damageStat;
            attack.hitEffectPrefab = hitEffectPrefab;
            attack.isCrit = RollCrit();
            attack.damageType = DamageSource.Primary;
            if ((bool)modelTransform)
            {
                attack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Snip");
            }
            if ((bool)modelAnimator)
            {
                PlayAnimation("Gesture", "FireSnip", "FireSnip.playbackRate", duration);
            }
            if ((bool)base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
            }
            Debug.Log("AttackEntered");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active && (bool)modelAnimator && modelAnimator.GetFloat("Snip.hitBoxActive") > 0.9f)
            {
                Debug.Log("AttackFired");
                Fire();
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void Fire()
        {
            if (!hasSnipped)
            {
                EffectManager.SimpleMuzzleFlash(snipEffectPrefab, base.gameObject, "SnipSpot", transmit: true);
                hasSnipped = true;
            }
            attack.forceVector = base.transform.forward * forceMagnitude;
            attack.Fire();
            
        }

        public override void OnExit()
        {
            base.OnExit();
            Debug.Log("AttackExit");
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
