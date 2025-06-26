using EnemiesReturns.Behaviors;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.Judgement.Arraign
{
    public class ArraignDamageController : NetworkBehaviour, IOnIncomingDamageServerReceiver, IOnTakeDamageServerReceiver
    {
        public static GameObject hitEffectPrefab;

        public CharacterBody body;

        private ChildLocator childLocator;

        public int segments;

        private uint segmentStatus = 0;

        private int currentSegment;

        private static HashSet<BodyIndex> bodiesToBypassArmor = new HashSet<BodyIndex>();

        public static void AddBodyToArmorBypass(BodyIndex bodyIndex)
        {
            if(bodyIndex != BodyIndex.None) bodiesToBypassArmor.Add(bodyIndex);
        }

        public static bool BodyCanBypassArmor(BodyIndex bodyIndex)
        {
            return bodiesToBypassArmor.Contains(bodyIndex);
        }

        private void OnEnable()
        {
            if (!body)
            {
                body = GetComponent<CharacterBody>();
            }
            childLocator = body.modelLocator.modelTransform.GetComponent<ChildLocator>();
            currentSegment = 0;
        }

        public void OnIncomingDamageServer(DamageInfo damageInfo)
        {
            if (!damageInfo.attacker)
            {
                return;
            }

            var attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();

            if (!attackerBody)
            {
                return;
            }

            var arraignIsImmune = body.HasBuff(Content.Buffs.ImmuneToAllDamageExceptHammer);
            var endGameBossWeaponDamage = damageInfo.damageType.HasModdedDamageType(Content.DamageTypes.EndGameBossWeapon);
            bool aeonianDamage = false;
            if(!endGameBossWeaponDamage && attackerBody && attackerBody.master && attackerBody.master.inventory)
            {
                aeonianDamage |= attackerBody.master.inventory.HasEquipment(Content.Equipment.EliteAeonian);
                aeonianDamage |= attackerBody.master.inventory.GetItemCount(Content.Items.HiddenAnointed) > 0;
                aeonianDamage |= bodiesToBypassArmor.Contains(attackerBody.bodyIndex);
            }
            if (arraignIsImmune && !(endGameBossWeaponDamage || aeonianDamage))
            {
                damageInfo.rejected = true;
                RenderDamageNumber(damageInfo.position);
            }

            if (arraignIsImmune && (endGameBossWeaponDamage || aeonianDamage))
            {
                body.RemoveBuff(Content.Buffs.ImmuneToAllDamageExceptHammer);
                if (childLocator)
                {
                    var effectData = new EffectData()
                    {
                        rootObject = base.gameObject,
                        modelChildIndex = (short)childLocator.FindChildIndex("Chest"),
                        scale = 3.5f
                    };

                    EffectManager.SpawnEffect(hitEffectPrefab, effectData, true);
                }
            }

            if (body.HasBuff(Content.Buffs.ImmuneToHammer) && endGameBossWeaponDamage)
            {
                damageInfo.rejected = true;
                RenderDamageNumber(damageInfo.position);
            }
        }

        public void RenderDamageNumber(Vector3 position)
        {
            if (ImmuneDamageNumbers.instance)
            {
                ImmuneDamageNumbers.instance.SpawnDamageNumber(position);
                RpcRenderDamageNumber(position);
            }
        }

        [ClientRpc]
        private void RpcRenderDamageNumber(Vector3 position)
        {
            if (ImmuneDamageNumbers.instance && !NetworkServer.active)
            {
                ImmuneDamageNumbers.instance.SpawnDamageNumber(position);
            }
        }

        public void OnTakeDamageServer(DamageReport damageReport)
        {
            if(currentSegment >= segments)
            {
                return;
            }
            var healthComponent = damageReport.victim;
            var segmentHealthSize = body.maxHealth / segments;
            var currentSegmentStatus = (segmentStatus & (uint)1 << currentSegment) == 0;

            if(damageReport.combinedHealthBeforeDamage - damageReport.damageDealt < body.maxHealth - (segmentHealthSize * (currentSegment + 1)) + 1 && currentSegmentStatus)
            {
                healthComponent.health = body.maxHealth - (segmentHealthSize * (currentSegment + 1)) + 1;
                damageReport.victimBody.AddBuff(Content.Buffs.ImmuneToAllDamageExceptHammer);
                segmentStatus |= (uint)1 << currentSegment;
                currentSegment++;
            }
        }

        private void OnValidate()
        {
            if(segments > 16)
            {
                segments = 16;
            }
        }
    }
}
