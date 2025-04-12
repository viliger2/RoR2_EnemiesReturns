using EnemiesReturns.Behaviors;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.Judgement.Arraign
{
    public class ArraignDamageController : NetworkBehaviour, IOnIncomingDamageServerReceiver, IOnTakeDamageServerReceiver
    {
        public CharacterBody body;

        public int segments;

        private uint segmentStatus = 0;

        private float segmentHealthSize;

        private int currentSegment;

        private void OnEnable()
        {
            if (!body)
            {
                body = GetComponent<CharacterBody>();
            }
            currentSegment = 0;
        }

        private void Start()
        {
            segmentHealthSize = body.maxHealth / segments;
        }

        public void OnIncomingDamageServer(DamageInfo damageInfo)
        {
            var arraignIsImmune = body.HasBuff(Content.Buffs.ImmuneToAllDamageExceptHammer);
            var endGameBossWeaponDamage = damageInfo.damageType.HasModdedDamageType(Content.DamageTypes.EndGameBossWeapon);
            if (arraignIsImmune && !endGameBossWeaponDamage)
            {
                damageInfo.rejected = true;
                RenderDamageNumber(damageInfo.position);
            }

            if (arraignIsImmune && endGameBossWeaponDamage)
            {
                body.RemoveBuff(Content.Buffs.ImmuneToAllDamageExceptHammer);
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

            var currentSegmentStatus = (segmentStatus & (uint)1 << currentSegment) == 0;

            if(damageReport.combinedHealthBeforeDamage - damageReport.damageDealt < healthComponent.fullCombinedHealth - (segmentHealthSize * (currentSegment + 1)) + 1 && currentSegmentStatus)
            {
                healthComponent.health = healthComponent.fullCombinedHealth - (segmentHealthSize * (currentSegment + 1)) + 1;
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
