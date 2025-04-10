using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Junk.ModdedEntityStates.Spitter
{
    [RegisterEntityState]
    public class NormalSpit : GenericProjectileBaseState
    {
        public static GameObject normalSpitProjectile;

        private static GameObject spitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Vermin/MuzzleflashVerminSpit.prefab").WaitForCompletion();

        public override void OnEnter()
        {
            baseDelayBeforeFiringProjectile = 0.5f;
            attackSoundString = "";
            bloom = 0f;
            baseDuration = 1.5f;
            damageCoefficient = 2f;
            targetMuzzle = "MuzzleMouth";
            recoilAmplitude = 0f;
            projectilePrefab = normalSpitProjectile;
            projectilePitchBonus = -1f;
            minSpread = 0f;
            maxSpread = 1f;
            force = 1000f;
            effectPrefab = spitEffectPrefab;

            base.OnEnter();

            Util.PlaySound("ER_Spiiter_Spit_Play", gameObject);

            if (characterBody)
            {
                characterBody.SetAimTimer(3f);
            }
        }

        public override void ModifyProjectileInfo(ref FireProjectileInfo fireProjectileInfo)
        {
            fireProjectileInfo.damageTypeOverride = DamageSource.Primary;
        }

        public override void PlayAnimation(float duration)
        {
            base.PlayAnimation(duration);
            PlayAnimation("Gesture", "Spit", "Spit.playbackRate", duration);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
