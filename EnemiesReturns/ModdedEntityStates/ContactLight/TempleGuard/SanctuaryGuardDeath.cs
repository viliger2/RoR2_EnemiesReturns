using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.TempleGuard
{
    [RegisterEntityState]
    public class SanctuaryGuardDeath : Death
    {
        //public static GameObject bodyExplosionEffect = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Commando.OmniExplosionVFXCommandoGrenade_prefab).WaitForCompletion();
        public static GameObject bodyExplosionEffect = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Common_VFX.OmniExplosionVFXMegaDrone_prefab).WaitForCompletion();

        public static float scale = 15f;

        public static float destroyModel = 2f;

        private bool spawnedEffect;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(!spawnedEffect && fixedAge > destroyModel)
            {
                if (isAuthority)
                {
                    var data = new EffectData()
                    {
                        origin = FindModelChild("BodyConnect").position,
                        rotation = Quaternion.identity,
                        scale = scale,
                    };
                    EffectManager.SpawnEffect(bodyExplosionEffect, data, true);
                }
                DestroyModel();
                if (NetworkServer.active)
                {
                    DestroyBodyAsapServer();
                }
                spawnedEffect = true;
            }
        }

        public override void OnEnter()
        {
            base.DestroyModel();
            base.OnEnter();
        }
    }
}
