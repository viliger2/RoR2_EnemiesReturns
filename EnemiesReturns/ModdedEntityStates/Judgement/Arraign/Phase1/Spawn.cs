using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1
{
    // TODO: sound effect on him removing sword from the ground, maybe even an effect
    [RegisterEntityState]
    public class Spawn : GenericCharacterSpawnState
    {
        public static GameObject slamEffect;

        private Animator animator;

        public override void OnEnter()
        {
            animator = GetModelAnimator();
            if (animator)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 0f);
                animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 0f);
            }
            duration = 5.5f;
            base.OnEnter();
            if (characterMotor && isAuthority)
            {
                characterMotor.onHitGroundAuthority += CharacterMotor_onHitGroundAuthority;
                characterMotor.gravityScale = 3f;
            }
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 6f);
            }
        }

        private void CharacterMotor_onHitGroundAuthority(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            var effectData = new EffectData()
            {
                origin = hitGroundInfo.position,
                scale = 7f
            };
            EffectManager.SpawnEffect(slamEffect, effectData, true);
            EntitySoundManager.EmitSoundServer((AkEventIdArg)"Play_moonBrother_spawn", gameObject);
        }

        public override void OnExit()
        {
            if (characterMotor && isAuthority)
            {
                characterMotor.onHitGroundAuthority -= CharacterMotor_onHitGroundAuthority;
                characterMotor.gravityScale = 1f;
            }
            base.OnExit();
            if (animator)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 1f);
                animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 1f);
            }
            if (NetworkServer.active)
            {
                if (characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                {
                    characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
            }
        }
    }
}
