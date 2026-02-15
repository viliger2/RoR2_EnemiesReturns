using EnemiesReturns.Behaviors;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.ContactLight.Providence
{
    public class ProvidenceDamageController : NetworkBehaviour, IOnIncomingDamageServerReceiver, IOnTakeDamageServerReceiver
    {
        public CharacterBody body;

        public int segments = 2;

        public float damageImmunityBuffDuration = 5f;

        private uint segmentStatus = 0;

        private int currentSegment;

        public static Material immuneToAllDamageOverlay;

        public static void AddDamageImmuneOverlay(CharacterModel self)
        {
            if (self.body && self.activeOverlayCount < RoR2.CharacterModel.maxOverlays && self.body.HasBuff(Content.Buffs.ProvidenceImmuneToDamage))
            {
                self.currentOverlays[self.activeOverlayCount++] = immuneToAllDamageOverlay;
            }
        }

        private void Awake()
        {
            if (!body)
            {
                body = GetComponent<CharacterBody>();
            }
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

            var endGameBossWeaponDamage = damageInfo.damageType.HasModdedDamageType(Content.DamageTypes.EndGameBossWeapon);

            var proviHasBuff = body.HasBuff(Content.Buffs.ProvidenceImmuneToDamage);
            if (endGameBossWeaponDamage && proviHasBuff)
            {
                body.RemoveBuff(Content.Buffs.ProvidenceImmuneToDamage);
            }

            if (proviHasBuff)
            {
                damageInfo.damage = 1f;
            }
        }

        public void OnTakeDamageServer(DamageReport damageReport)
        {
            if (currentSegment >= segments)
            {
                return;
            }

            var segmentHealthSize = body.maxHealth / segments;
            var currentSegmentStatus = (segmentStatus & (uint)1 << currentSegment) == 0;

            if (damageReport.combinedHealthBeforeDamage - damageReport.damageDealt < body.maxHealth - (segmentHealthSize * (currentSegment + 1)) + 1 && currentSegmentStatus)
            {
                body.AddTimedBuff(Content.Buffs.ProvidenceImmuneToDamage, damageImmunityBuffDuration);
                segmentStatus |= (uint)1 << currentSegment;
                currentSegment++;
            }
        }

        private void OnValidate()
        {
            if (segments > 16)
            {
                segments = 16;
            }
        }
    }
}
