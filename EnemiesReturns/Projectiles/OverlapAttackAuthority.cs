using RoR2.Audio;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using static RoR2.OverlapAttack;
using UnityEngine;
using RoR2.Networking;
using UnityEngine.Networking;

namespace EnemiesReturns.Projectiles
{
    public class OverlapAttackAuthority
    {
        public GameObject attacker;

        public GameObject inflictor;

        public TeamIndex teamIndex;

        public AttackerFiltering attackerFiltering = AttackerFiltering.NeverHitSelf;

        public Vector3 forceVector = Vector3.zero;

        public float pushAwayForce;

        public float damage = 1f;

        public bool isCrit;

        public ProcChainMask procChainMask;

        public float procCoefficient = 1f;

        public HitBoxGroup hitBoxGroup;

        public GameObject hitEffectPrefab;

        public NetworkSoundEventIndex impactSound = NetworkSoundEventIndex.Invalid;

        public DamageColorIndex damageColorIndex;

        public DamageTypeCombo damageType;

        public int maximumOverlapTargets = 100;

        private readonly List<HealthComponent> ignoredHealthComponentList = new List<HealthComponent>();

        public float retriggerTimeout = float.PositiveInfinity;

        private Queue<(HealthComponent, float)> ignoredRemovalList = new Queue<(HealthComponent, float)>();

        private readonly List<OverlapInfo> overlapList = new List<OverlapInfo>();

        private static readonly OverlapAttackMessage incomingMessage = new OverlapAttackMessage();

        private static readonly OverlapAttackMessage outgoingMessage = new OverlapAttackMessage();

        public Vector3 lastFireAverageHitPosition { get; private set; }

        public GameObject lastHitObject { get; private set; }

        private bool HurtBoxPassesFilter(HurtBox hurtBox)
        {
            if (!hurtBox.healthComponent)
            {
                return false;
            }
            if (!Util.HasEffectiveAuthority(hurtBox.healthComponent.gameObject))
            {
                return false;
            }
            if (hurtBox.healthComponent.gameObject == attacker && attackerFiltering == AttackerFiltering.NeverHitSelf)
            {
                return false;
            }
            if (attacker == null && hurtBox.healthComponent.gameObject.GetComponent<MaulingRock>() != null)
            {
                return false;
            }
            if (ignoredHealthComponentList.Contains(hurtBox.healthComponent))
            {
                return false;
            }
            if (!FriendlyFireManager.ShouldDirectHitProceed(hurtBox.healthComponent, teamIndex))
            {
                return false;
            }
            return true;
        }

        public bool Fire(List<HurtBox> hitResults = null)
        {
            if (!hitBoxGroup)
            {
                return false;
            }
            HitBox[] hitBoxes = hitBoxGroup.hitBoxes;
            foreach (HitBox hitBox in hitBoxes)
            {
                if (!hitBox || !hitBox.enabled || !hitBox.gameObject || !hitBox.gameObject.activeInHierarchy || !hitBox.transform)
                {
                    continue;
                }
                Transform transform = hitBox.transform;
                Vector3 position = transform.position;
                Vector3 halfExtents = transform.lossyScale * 0.5f;
                Quaternion rotation = transform.rotation;
                if (float.IsInfinity(halfExtents.x) || float.IsInfinity(halfExtents.y) || float.IsInfinity(halfExtents.z))
                {
                    Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxHalfExtents are infinite.");
                    continue;
                }
                if (float.IsNaN(halfExtents.x) || float.IsNaN(halfExtents.y) || float.IsNaN(halfExtents.z))
                {
                    Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxHalfExtents are NaN.");
                    continue;
                }
                if (float.IsInfinity(position.x) || float.IsInfinity(position.y) || float.IsInfinity(position.z))
                {
                    Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxCenter is infinite.");
                    continue;
                }
                if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z))
                {
                    Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxCenter is NaN.");
                    continue;
                }
                if (float.IsInfinity(rotation.x) || float.IsInfinity(rotation.y) || float.IsInfinity(rotation.z) || float.IsInfinity(rotation.w))
                {
                    Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxRotation is infinite.");
                    continue;
                }
                if (float.IsNaN(rotation.x) || float.IsNaN(rotation.y) || float.IsNaN(rotation.z) || float.IsNaN(rotation.w))
                {
                    Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxRotation is NaN.");
                    continue;
                }
                Collider[] colliders;
                int num = HGPhysics.OverlapBox(out colliders, position, halfExtents, rotation, LayerIndex.entityPrecise.mask);
                cleanupRetriggerList();
                int num2 = 0;
                for (int j = 0; j < num; j++)
                {
                    if ((bool)colliders[j])
                    {
                        HurtBox component = colliders[j].GetComponent<HurtBox>();
                        if ((bool)component && HurtBoxPassesFilter(component) && (bool)component.transform)
                        {
                            Vector3 position2 = component.transform.position;
                            overlapList.Add(new OverlapInfo
                            {
                                hurtBox = component,
                                hitPosition = position2,
                                pushDirection = (position2 - position).normalized
                            });
                            lastHitObject = component.healthComponent.gameObject;
                            addIgnoredHitList(component.healthComponent);
                            hitResults?.Add(component);
                            num2++;
                        }
                        if (num2 >= maximumOverlapTargets)
                        {
                            break;
                        }
                    }
                }
                HGPhysics.ReturnResults(colliders);
            }
            ProcessHits(overlapList);
            bool result = overlapList.Count > 0;
            overlapList.Clear();
            return result;
        }

        public void addIgnoredHitList(HealthComponent component)
        {
            if (retriggerTimeout != float.PositiveInfinity)
            {
                ignoredRemovalList.Enqueue((component, Time.realtimeSinceStartup + retriggerTimeout));
            }
            ignoredHealthComponentList.Add(component);
        }

        public void cleanupRetriggerList()
        {
            while (ignoredRemovalList.Count > 0 && Time.realtimeSinceStartup > ignoredRemovalList.Peek().Item2)
            {
                (HealthComponent, float) tuple = ignoredRemovalList.Dequeue();
                ignoredHealthComponentList.Remove(tuple.Item1);
            }
        }

        [NetworkMessageHandler(msgType = 71, client = false, server = true)]
        public static void HandleOverlapAttackHits(NetworkMessage netMsg)
        {
            netMsg.ReadMessage(incomingMessage);
            PerformDamage(incomingMessage.attacker, incomingMessage.inflictor, incomingMessage.damage, incomingMessage.isCrit, incomingMessage.procChainMask, incomingMessage.procCoefficient, incomingMessage.damageColorIndex, incomingMessage.damageType, incomingMessage.forceVector, incomingMessage.pushAwayForce, incomingMessage.overlapInfoList);
        }

        private void ProcessHits(List<OverlapInfo> hitList)
        {
            if (hitList.Count == 0)
            {
                return;
            }
            Vector3 zero = Vector3.zero;
            float num = 1f / (float)hitList.Count;
            for (int i = 0; i < hitList.Count; i++)
            {
                OverlapInfo overlapInfo = hitList[i];
                if ((bool)hitEffectPrefab)
                {
                    Vector3 forward = -hitList[i].pushDirection;
                    EffectManager.SpawnEffect(hitEffectPrefab, new EffectData
                    {
                        origin = overlapInfo.hitPosition,
                        rotation = Util.QuaternionSafeLookRotation(forward),
                        networkSoundEventIndex = impactSound
                    }, transmit: true);
                }
                zero += overlapInfo.hitPosition * num;
                SurfaceDefProvider surfaceDefProvider = (overlapInfo.hurtBox ? overlapInfo.hurtBox.GetComponent<SurfaceDefProvider>() : null);
                if (!surfaceDefProvider || !surfaceDefProvider.surfaceDef)
                {
                    continue;
                }
                SurfaceDef objectSurfaceDef = SurfaceDefProvider.GetObjectSurfaceDef(hitList[i].hurtBox.collider, hitList[i].hitPosition);
                if ((bool)objectSurfaceDef)
                {
                    if ((bool)objectSurfaceDef.impactEffectPrefab)
                    {
                        EffectManager.SpawnEffect(objectSurfaceDef.impactEffectPrefab, new EffectData
                        {
                            origin = overlapInfo.hitPosition,
                            rotation = ((overlapInfo.pushDirection == Vector3.zero) ? Quaternion.identity : Util.QuaternionSafeLookRotation(overlapInfo.pushDirection)),
                            color = objectSurfaceDef.approximateColor,
                            scale = 2f
                        }, transmit: true);
                    }
                    if (objectSurfaceDef.impactSoundString != null && objectSurfaceDef.impactSoundString.Length != 0)
                    {
                        Util.PlaySound(objectSurfaceDef.impactSoundString, hitList[i].hurtBox.gameObject);
                    }
                }
            }
            lastFireAverageHitPosition = zero;
            if (NetworkServer.active)
            {
                PerformDamage(attacker, inflictor, damage, isCrit, procChainMask, procCoefficient, damageColorIndex, damageType, forceVector, pushAwayForce, hitList);
                return;
            }
            outgoingMessage.attacker = attacker;
            outgoingMessage.inflictor = inflictor;
            outgoingMessage.damage = damage;
            outgoingMessage.isCrit = isCrit;
            outgoingMessage.procChainMask = procChainMask;
            outgoingMessage.procCoefficient = procCoefficient;
            outgoingMessage.damageColorIndex = damageColorIndex;
            outgoingMessage.damageType = damageType;
            outgoingMessage.forceVector = forceVector;
            outgoingMessage.pushAwayForce = pushAwayForce;
            Util.CopyList(hitList, outgoingMessage.overlapInfoList);
            PlatformSystems.networkManager.client.connection.SendByChannel(71, outgoingMessage, QosChannelIndex.defaultReliable.intVal);
        }

        private static void PerformDamage(GameObject attacker, GameObject inflictor, float damage, bool isCrit, ProcChainMask procChainMask, float procCoefficient, DamageColorIndex damageColorIndex, DamageTypeCombo damageType, Vector3 forceVector, float pushAwayForce, List<OverlapInfo> hitList)
        {
            for (int i = 0; i < hitList.Count; i++)
            {
                OverlapInfo overlapInfo = hitList[i];
                if ((bool)overlapInfo.hurtBox)
                {
                    HealthComponent healthComponent = overlapInfo.hurtBox.healthComponent;
                    if ((bool)healthComponent)
                    {
                        DamageInfo damageInfo = new DamageInfo();
                        damageInfo.attacker = attacker;
                        damageInfo.inflictor = inflictor;
                        damageInfo.force = forceVector + pushAwayForce * overlapInfo.pushDirection;
                        damageInfo.damage = damage;
                        damageInfo.crit = isCrit;
                        damageInfo.position = overlapInfo.hitPosition;
                        damageInfo.procChainMask = procChainMask;
                        damageInfo.procCoefficient = procCoefficient;
                        damageInfo.damageColorIndex = damageColorIndex;
                        damageInfo.damageType = damageType;
                        damageInfo.ModifyDamageInfo(overlapInfo.hurtBox.damageModifier);
                        healthComponent.TakeDamage(damageInfo);
                        GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
                        GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
                    }
                }
            }
        }

        public void ResetIgnoredHealthComponents()
        {
            ignoredHealthComponentList.Clear();
            ignoredRemovalList.Clear();
        }

        public void Reset()
        {
            attacker = null;
            inflictor = null;
            teamIndex = TeamIndex.Neutral;
            forceVector = Vector3.zero;
            pushAwayForce = 0f;
            damage = 1f;
            isCrit = false;
            procChainMask.mask = 0u;
            procCoefficient = 1f;
            hitBoxGroup = null;
            hitEffectPrefab = null;
            damageColorIndex = DamageColorIndex.Default;
            damageType = DamageType.Generic;
            ignoredHealthComponentList.Clear();
            ignoredRemovalList.Clear();
        }


    }
}
