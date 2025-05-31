using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static RoR2.CharacterBody;

namespace EnemiesReturns.Equipment.MithrixHammer
{
    public class MithrixHammerOnDamageDealtServerReciever : ItemBehavior, IOnDamageDealtServerReceiver
    {
        public void OnDamageDealtServer(DamageReport damageReport)
        {
            var damageInfo = damageReport.damageInfo;
            var body = damageReport.victimBody;

            if (body && body.healthComponent)
            {
                if (body.HasBuff(Content.Buffs.AffixAeoninan) && (damageInfo.damageType.damageSource & DamageSource.Equipment) == DamageSource.Equipment && damageInfo.damageType.HasModdedDamageType(Content.DamageTypes.EndGameBossWeapon))
                {
                    var newDamageInfo = new DamageInfo()
                    {
                        attacker = damageReport.attacker,
                        inflictor = null,
                        position = body.transform.position,
                        crit = false,
                        damage = damageInfo.damage * Configuration.Judgement.MithrixHammerAeonianBonusDamage.Value,
                        damageColorIndex = DamageColorIndex.Fragile,
                        procCoefficient = 0f,
                        force = Vector3.zero,
                        procChainMask = default,
                        damageType = DamageType.Generic
                    };

                    body.healthComponent.TakeDamage(newDamageInfo);
                }
            }
        }
    }
}
