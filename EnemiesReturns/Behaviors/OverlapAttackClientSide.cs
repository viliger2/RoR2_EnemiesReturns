using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors
{
    public class OverlapAttackClientside : NetworkBehaviour
    {
        public float damage;

        public bool isCrit;

        public GameObject attacker;

        public DamageColorIndex damageColor;

        public DamageTypeCombo damageType;

        public Vector3 forceVector;

        public float procCoefficient;

        public TeamIndex attackerTeamIndex;

        private List<CharacterBody> affectedBodies = new List<CharacterBody>();

        private void OnEnable()
        {
            affectedBodies.Clear();
        }

        private void OnDisable()
        {
            affectedBodies.Clear();
        }

        private void OnTriggerEnter(Collider collider)
        {
            CharacterBody body = collider.GetComponent<CharacterBody>();
            if (body && body.hasEffectiveAuthority)
            {
                affectedBodies.Add(body);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            CharacterBody body = collider.GetComponent<CharacterBody>();
            if (body && affectedBodies.Contains(body))
            {
                affectedBodies.Remove(body);
            }
        }

        public void DealDamage()
        {
            foreach (var charBody in affectedBodies)
            {
                if (!charBody.hasEffectiveAuthority)
                {
                    continue;
                }

                if (NetworkServer.active)
                {
                    PerformDamageServer(charBody);
                }
                else
                {
                    CmdPerformDamage(charBody.netId.Value);
                }
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
                if (FriendlyFireManager.ShouldDirectHitProceed(body.healthComponent, attackerTeamIndex))
                {
                    var damageInfo = new DamageInfo
                    {
                        damage = damage,
                        crit = isCrit,
                        inflictor = gameObject,
                        attacker = gameObject,
                        position = body.footPosition,
                        canRejectForce = true,
                        damageColorIndex = damageColor,
                        damageType = damageType,
                        force = forceVector,
                        procCoefficient = procCoefficient,
                    };

                    body.healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, body.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, body.gameObject);
                }
            }
        }
    }
}
