using EnemiesReturns.Enemies.Spitter;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Spitter
{
    public class NormalSpit : GenericProjectileBaseState
    {
        private static GameObject spitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Vermin/MuzzleflashVerminSpit.prefab").WaitForCompletion();

        public override void OnEnter()
        {
            baseDelayBeforeFiringProjectile = 0.5f;
            attackSoundString = "";
            bloom = 0f;
            baseDuration = 1.5f;
            damageCoefficient = EnemiesReturnsConfiguration.Spitter.NormalSpitDamage.Value;
            targetMuzzle = "MuzzleMouth";
            recoilAmplitude = 0f;
            //projectilePrefab = SpitterFactory.normalSpitProjectile;
            projectilePitchBonus = -1f;
            minSpread = 0f;
            maxSpread = 1f;
            force = EnemiesReturnsConfiguration.Spitter.NormalSpitForce.Value;
            effectPrefab = spitEffectPrefab;

            base.OnEnter();

            Util.PlaySound("ER_Spiiter_Spit_Play", gameObject);

            if(characterBody)
            {
                characterBody.SetAimTimer(3f);
            }
        }

        public override void PlayAnimation(float duration)
        {
            base.PlayAnimation(duration);
            PlayAnimation("Gesture", "Spit", "Spit.playbackRate", duration);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
