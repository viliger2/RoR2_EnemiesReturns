using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.CharacterBody;

namespace EnemiesReturns.Behaviors
{
#pragma warning disable CS0618 // Type or member is obsolete
    public abstract class BossDamageEquipmentOnDamageDealtServerReciever : ItemBehavior, IOnDamageDealtServerReceiver
#pragma warning restore CS0618 // Type or member is obsolete
    {
        public readonly static HashSet<BodyIndex> whiteListedBodies = new HashSet<BodyIndex>();

        public abstract EquipmentIndex equipmentIndex { get; }

        public abstract float damageCoefficient { get; }

        public abstract DamageColorIndex damageColor { get; }

        public static void AddWhitelistedBodies()
        {
            whiteListedBodies.Add(BodyCatalog.FindBodyIndex("EquipmentDroneBody"));
        }

        private void Start()
        {
            if (body && body.master)
            {
                body.master.onBodyDeath.AddListener(OnBodyDeath);
            }
        }

        private void OnDisable()
        {
            if (body && body.master)
            {
                body.master.onBodyDeath.RemoveListener(OnBodyDeath);
            }
        }

        private void OnBodyDeath()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!body || !body.master || !body.inventory)
            {
                return;
            }

            if (!body.master.IsDeadAndOutOfLivesServer())
            {
                return;
            }

            if (!(body.isPlayerControlled || whiteListedBodies.Contains(body.bodyIndex)))
            {
                return;
            }

            var vector = Vector3.up * 20f + transform.forward * 2f;
            PickupDropletController.CreatePickupDroplet(new UniquePickup(PickupCatalog.FindPickupIndex(equipmentIndex)), body.transform.position, vector, false);

            var location = body.inventory.GetEquipmentStateAndLocation(equipmentIndex).Item2;
            if (!location.Equals(EquipmentLocation.invalid))
            {
                var equipmentState = new EquipmentState(EquipmentIndex.None, Run.FixedTimeStamp.now, 0);
                body.inventory.SetEquipment(equipmentState, location);
            }
        }

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
                        damage = damageInfo.damage * damageCoefficient,
                        damageColorIndex = damageColor,
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
