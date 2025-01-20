using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors
{
    public class HitboxAttackClientside : NetworkBehaviour
    {
        public ProjectileController projectileController;

        public ProjectileDamage projectileDamage;

        private List<HealthComponent> hitTargets = new List<HealthComponent>();

        public float force;

        private void Awake()
        {
            if (!projectileController)
            {
                projectileController = GetComponent<ProjectileController>();
            }

            if (!projectileDamage)
            {
                projectileDamage = GetComponent<ProjectileDamage>();
            }
        }

        private void Start()
        {
            var position = transform.position;
            var halfExtends = transform.lossyScale * 0.5f;
            var rotation = transform.rotation;

            Collider[] colliders;

            int num = HGPhysics.OverlapBox(out colliders, position, halfExtends, rotation, LayerIndex.entityPrecise.mask);
            for (int i = 0; i < num; i++)
            {
                if (!colliders[i])
                {
                    continue;
                }

                var hurtBox = colliders[i].GetComponent<HurtBox>();
                if(!hurtBox || !hurtBox.healthComponent || !hurtBox.healthComponent.body || hitTargets.Contains(hurtBox.healthComponent))
                {
                    continue;
                }

                var body = hurtBox.healthComponent.body;
                if(body.hasEffectiveAuthority && body.characterMotor && body.characterMotor.isGrounded)
                {
                    PerformDamage(body);
                    hitTargets.Add(body.healthComponent);
                }

            }
            HGPhysics.ReturnResults(colliders);
        }

        private void PerformDamage(CharacterBody body)
        {
            if (NetworkServer.active)
            {
                PerformDamageServer(body);
            }
            else
            {
                CmdPerformDamage(body.netId.Value);
            }
        }

        [Command]
        public void CmdPerformDamage(uint netId)
        {
            var gameObject = Util.FindNetworkObject(new NetworkInstanceId(netId));
            if (gameObject && gameObject.TryGetComponent<CharacterBody>(out var characterBody))
            {
                PerformDamageServer(characterBody);
            }
        }

        private void PerformDamageServer(CharacterBody body)
        {
            if (body.healthComponent)
            {
                if (FriendlyFireManager.ShouldDirectHitProceed(body.healthComponent, projectileController.teamFilter.teamIndex))
                {
                    var damageInfo = new DamageInfo
                    {
                        damage = projectileDamage.damage,
                        crit = projectileDamage.crit,
                        inflictor = gameObject,
                        attacker = projectileController.owner ? projectileController.owner.gameObject : null,
                        position = body.footPosition,
                        canRejectForce = true,
                        damageColorIndex = projectileDamage.damageColorIndex,
                        damageType = projectileDamage.damageType,
                        force = Vector3.up * force,
                        procChainMask = projectileController.procChainMask,
                        procCoefficient = projectileController.procCoefficient,
                    };

                    body.healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, body.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, body.gameObject);
                }
            }
        }
    }
}
