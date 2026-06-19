using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.TempleGuard
{
    [RegisterEntityState]
    public class Death : GenericCharacterDeath
    {
        public static GameObject explosionEffect = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.OmniExplosionVFX_prefab).WaitForCompletion();

        public static float effectScale = 5f;

        private bool firstExplosion;

        private bool secondExplosion;

        private Animator animator;

        private static int LeftCannonExplosion = Animator.StringToHash("LeftCannon.explosion");

        private static int RightCannonExplosion = Animator.StringToHash("RightCannon.explosion");

        public override void OnEnter()
        {
            bodyPreservationDuration = 3.25f;
            base.OnEnter();
            animator = GetModelAnimator();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(animator.GetFloat(LeftCannonExplosion) > 0.9f && !firstExplosion)
            {
                var effectParams = new EffectData()
                {
                    origin = FindModelChild("CannonL").position,
                    scale = effectScale
                };
                EffectManager.SpawnEffect(explosionEffect, effectParams, false);
                firstExplosion = true;
            }
            if (animator.GetFloat(RightCannonExplosion) > 0.9f && !secondExplosion)
            {
                var effectParams = new EffectData()
                {
                    origin = FindModelChild("CannonR").position,
                    scale = effectScale
                };
                EffectManager.SpawnEffect(explosionEffect, effectParams, false); secondExplosion = true;
            }
        }

    }
}
