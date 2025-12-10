using EnemiesReturns.Components;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static RoR2.CharacterMaster;

namespace EnemiesReturns.Items.LunarFlower
{
    public class LunarFowerExtraLifeBehaviour : ExtraLifeServerBehavior
    {
        public static GameObject startEffect;

        public static GameObject endEffect;

        private static GameObject glassEffect;

        private void OnEnable()
        {
            if (!glassEffect)
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Common_VFX.BrittleDeath_prefab);
                if (handle.IsValid())
                {
                    handle.Completed += (obj) =>
                    {
                        glassEffect = obj.Result;
                        Addressables.Release(handle);
                    };
                }
            }
            Invoke("SpawnVoidFiendEffect", 1f);
        }

        private void SpawnVoidFiendEffect()
        {
            if (startEffect)
            {
                EffectManager.SpawnEffect(startEffect, new EffectData
                {
                    origin = characterMaster.deathFootPosition + Vector3.up * 2,
                    rotation = Quaternion.identity
                }, true);
            }
        }

        public static bool CanRevive(CharacterMaster master)
        {
            if (!NetworkServer.active)
            {
                return false;
            }

            var body = master.GetBody();
            if (!body)
            {
                return false;
            }

            var lunarFlowerComponent = body.GetComponent<LunarFlowerItemBehaviour>();
            if (!lunarFlowerComponent)
            {
                return false;
            }

            return lunarFlowerComponent.hasVoidDied;
        }

        public void LunarFlowerRevive()
        {
            var vector = characterMaster.deathFootPosition + Vector3.up * 2;
            characterMaster.Respawn(characterMaster.deathFootPosition, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f), true);
            var body = characterMaster.GetBody();
            body.AddTimedBuff(RoR2Content.Buffs.Immune, 3f);
            if (characterMaster.bodyInstanceObject)
            {
                EntityStateMachine[] components = characterMaster.bodyInstanceObject.GetComponents<EntityStateMachine>();
                foreach (EntityStateMachine obj in components)
                {
                    obj.initialStateType = obj.mainStateType;
                }
                if (characterMaster.gameObject)
                {
                    if (endEffect)
                    {
                        EffectManager.SpawnEffect(endEffect, new EffectData
                        {
                            origin = vector,
                            rotation = characterMaster.bodyInstanceObject.transform.rotation
                        }, transmit: true);
                    }
                    if (glassEffect)
                    {
                        EffectManager.SpawnEffect(glassEffect, new EffectData
                        {
                            origin = vector,
                            rotation = characterMaster.bodyInstanceObject.transform.rotation
                        }, transmit: true);
                    }
                }
            }
        }
    }
}
