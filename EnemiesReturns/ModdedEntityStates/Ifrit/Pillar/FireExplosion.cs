using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar
{
    public class FireExplosion : BaseState
    {
        public static float duration = 0.1f; // just to be safe

        public static float damage => EnemiesReturnsConfiguration.Ifrit.PillarExplosionDamage.Value;

        public static float radius => EnemiesReturnsConfiguration.Ifrit.PillarExplosionRadius.Value;

        public static float force => EnemiesReturnsConfiguration.Ifrit.PillarExplosionForce.Value;

        public static GameObject explosionPrefab;

        private BlastAttack blastAttack;

        public override void OnEnter()
        {
            base.OnEnter();

            var childLocator = GetModelChildLocator();
            var fireball = childLocator.FindChild("Fireball");
            if (NetworkServer.active)
            {
                if(explosionPrefab)
                {
                    EffectManager.SpawnEffect(explosionPrefab, new EffectData { origin = fireball ? fireball.position : gameObject.transform.position, scale = 5f * (radius / 30f) }, true); // TODO
                }

                blastAttack = new BlastAttack();
                blastAttack.attacker = base.gameObject;
                blastAttack.radius = radius;
                blastAttack.procCoefficient = 0f;
                blastAttack.position = transform.position;
                blastAttack.crit = false;
                blastAttack.baseDamage = damage * damageStat;
                blastAttack.canRejectForce = false;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.baseForce = force;
                blastAttack.teamIndex = characterBody.teamComponent.teamIndex;
                blastAttack.damageType = DamageType.IgniteOnHit;
                blastAttack.attackerFiltering = AttackerFiltering.Default;
                blastAttack.Fire();
            }
            if(fireball)
            {
                fireball.gameObject.SetActive(false);
            }
            var areaIndicator = childLocator.FindChild("TeamAreaIndicator");
            if (areaIndicator)
            {
                areaIndicator.gameObject.SetActive(false);
            }
            var lineRenderer = childLocator.FindChild("LineOriginPoint");
            if (lineRenderer)
            {
                lineRenderer.gameObject.SetActive(false);
            }
            Util.PlaySound("ER_Ifrit_Pillar_Explosion_Play", gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active)
            {
                //blastAttack.Fire();
                if (fixedAge >= duration)
                {
                    base.healthComponent.Suicide();
                }
            }
        }
    }
}
