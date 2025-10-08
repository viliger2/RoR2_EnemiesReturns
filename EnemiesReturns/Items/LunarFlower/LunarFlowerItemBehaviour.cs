using EnemiesReturns.Equipment.MithrixHammer;
using RoR2;
using RoR2.Items;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static RoR2.CharacterBody;

namespace EnemiesReturns.Items.LunarFlower
{
    public class LunarFlowerItemBehaviour : ItemBehavior, IOnKilledServerReceiver
    {
        public bool hasVoidDied;

        public static GameObject startEffect;

        public static GameObject endEffect;

        private static GameObject glassEffect;

        private void Start()
        {
            hasVoidDied = false;
            //if (!endEffect)
            //{
            //    var handle = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.RoR2_DLC1_VoidSurvivor.VoidSurvivorCorruptDeathMuzzleflash_prefab);
            //    if (handle.IsValid())
            //    {
            //        handle.Completed += (obj) =>
            //        {
            //            endEffect = obj.Result;
            //            Addressables.Release(handle);
            //        };
            //    }
            //}
            if (!glassEffect)
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common_VFX.BrittleDeath_prefab);
                if (handle.IsValid())
                {
                    handle.Completed += (obj) =>
                    {
                        glassEffect = obj.Result;
                        Addressables.Release(handle);
                    };
                }
            }
        }

        public void OnKilledServer(DamageReport damageReport)
        {
            if(damageReport != null && damageReport.damageInfo != null && IsDamageVoidDeath(damageReport.damageInfo))
            {
                hasVoidDied = true;
            }
        }

        private bool IsDamageVoidDeath(DamageInfo damageInfo)
        {
            return (damageInfo.damageType.damageType & DamageType.VoidDeath) == DamageType.VoidDeath;
        }

        public static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (Configuration.Judgement.Judgement.Enabled.Value)
            {
                if (NetworkServer.active)
                {
                    if (body && body.inventory)
                    {
                        body.AddItemBehavior<LunarFlowerItemBehaviour>(body.inventory.GetItemCount(Content.Items.LunarFlower));
                    }
                }
            }
        }

        public static void Hooks()
        {
            if (Configuration.Judgement.Judgement.Enabled.Value)
            {
                ReviveAPI.ReviveAPI.AddCustomRevive(CanRevive, new ReviveAPI.ReviveAPI.PendingOnRevive[]
                {
                    new ReviveAPI.ReviveAPI.PendingOnRevive
                    {
                        onReviveDelegate = SpawnVoidFiendEffect,
                        timer = 1f,
                    },
                    new ReviveAPI.ReviveAPI.PendingOnRevive
                    {
                        onReviveDelegate = ReviveWithEffects,
                        timer = 2f,
                    }
                },
                9999);
            }
        }

        private static void ReviveWithEffects(CharacterMaster master)
        {
            var vector = master.deathFootPosition + Vector3.up * 2;
            master.Respawn(master.deathFootPosition, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f), true);
            var body = master.GetBody();
            body.AddTimedBuff(RoR2Content.Buffs.Immune, 3f);
            master.inventory.GiveItem(Content.Items.VoidFlower);
            CharacterMasterNotificationQueue.SendTransformNotification(master, Content.Items.LunarFlower.itemIndex, Content.Items.VoidFlower.itemIndex, CharacterMasterNotificationQueue.TransformationType.ContagiousVoid);
            if (master.bodyInstanceObject)
            {
                EntityStateMachine[] components = master.bodyInstanceObject.GetComponents<EntityStateMachine>();
                foreach (EntityStateMachine obj in components)
                {
                    obj.initialStateType = obj.mainStateType;
                }
                if (master.gameObject)
                {
                    if (endEffect)
                    {
                        EffectManager.SpawnEffect(endEffect, new EffectData
                        {
                            origin = vector,
                            rotation = master.bodyInstanceObject.transform.rotation
                        }, transmit: true);
                    }
                    if (glassEffect)
                    {
                        EffectManager.SpawnEffect(glassEffect, new EffectData
                        {
                            origin = vector,
                            rotation = master.bodyInstanceObject.transform.rotation
                        }, transmit: true);
                    }
                }
            }
            master.inventory.RemoveItem(Content.Items.LunarFlower, master.inventory.GetItemCount(Content.Items.LunarFlower));
        }

        private static void SpawnVoidFiendEffect(CharacterMaster master)
        {
            if (startEffect)
            {
                EffectManager.SpawnEffect(startEffect, new EffectData
                {
                    origin = master.deathFootPosition + Vector3.up * 2,
                    rotation = Quaternion.identity
                }, true);
            }
        }

        private static bool CanRevive(CharacterMaster master)
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
    }
}
