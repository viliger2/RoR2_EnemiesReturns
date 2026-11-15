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
        public static GameObject bodyExplosionEffect = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Commando.OmniExplosionVFXCommandoGrenade_prefab).WaitForCompletion();

        public static float destroyModel = 2f;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > destroyModel)
            {
                if (isAuthority)
                {
                    var data = new EffectData()
                    {
                        origin = FindModelChild("BodyConnect").position,
                        rotation = Quaternion.identity,
                    };
                    EffectManager.SpawnEffect(bodyExplosionEffect, data, true);
                }
                DestroyModel();
                if (NetworkServer.active)
                {
                    DestroyBodyAsapServer();
                }
            }
        }

        public override void OnEnter()
        {
            base.DestroyModel();
            base.OnEnter();
        }
    }
}
