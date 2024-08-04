using EnemiesReturns.Enemies.Spitter;
using EntityStates;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Spitter
{
    public class Bite : BaseState
    {
        public static float baseDuration = 1f;

        public static float damageCoefficient = EnemiesReturnsConfiguration.Spitter.BiteDamageModifier.Value;

        public static float forceMagnitude = EnemiesReturnsConfiguration.Spitter.BiteDamageForce.Value;

        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFXSlash.prefab").WaitForCompletion();

        public static GameObject biteEffectPrefab = SpitterFactory.biteEffectPrefab;

        public static string attackString = "ER_Spitter_Bite_Play";

        private OverlapAttack attack;

        private Animator modelAnimator;

        private float duration;

        private bool hasBit;

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
            attack.isCrit = Util.CheckRoll(critStat, base.characterBody.master);
            Util.PlayAttackSpeedSound(attackString, base.gameObject, attackSpeedStat);
            if ((bool)modelTransform)
            {
                attack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Bite");
            }
            if ((bool)modelAnimator)
            {
                PlayAnimation("Gesture", "Bite", "Bite.playbackRate", duration);
            }
            if ((bool)base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active && (bool)modelAnimator && modelAnimator.GetFloat("Bite.hitBoxActive") > 0.1f)
            {
                if (!hasBit)
                {
                    EffectManager.SimpleMuzzleFlash(biteEffectPrefab, base.gameObject, "BiteSpot", transmit: true);
                    hasBit = true;
                }
                attack.forceVector = base.transform.forward * forceMagnitude;
                attack.Fire();
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
