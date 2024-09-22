using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pylon
{
    public class FireExplosion : BaseState
    {
        public static float duration = 1f; // just to be safe

        public static GameObject explosionPrefab;

        private BlastAttack blastAttack;

        private Transform fireball;
        private Transform areaIndicator;

        public override void OnEnter()
        {
            base.OnEnter();

            var childLocator = GetModelChildLocator();
            fireball = childLocator.FindChild("Fireball");
            areaIndicator = childLocator.FindChild("TeamAreaIndicator");
            if (NetworkServer.active)
            {
                if(explosionPrefab)
                {
                    EffectManager.SpawnEffect(explosionPrefab, new EffectData { origin = fireball ? fireball.position : gameObject.transform.position, scale = 5f }, true);
                }

                blastAttack = new BlastAttack();
                blastAttack.attacker = base.gameObject;
                blastAttack.radius = 30f; // TODO
                blastAttack.procCoefficient = 0f;
                blastAttack.position = transform.position;
                blastAttack.crit = false;
                blastAttack.baseDamage = 0.5f * damageStat;
                blastAttack.canRejectForce = false;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.baseForce = 3000f;
                blastAttack.teamIndex = characterBody.teamComponent.teamIndex;
                blastAttack.damageType = DamageType.Generic;
                blastAttack.attackerFiltering = AttackerFiltering.Default;
                blastAttack.Fire();
            }
            if(fireball)
            {
                fireball.gameObject.SetActive(false);
            }
            if(areaIndicator)
            {
                areaIndicator.gameObject.SetActive(false);
            }
            var modelTransform = GetModelTransform();
            if(modelTransform.gameObject.TryGetComponent<LineRenderer>(out var lineRenderer))
            {
                lineRenderer.enabled = false;
            }
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
