using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Special;
using EnemiesReturns.Projectiles;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseRings
{
    public abstract class BaseFireRings : BaseState
    { 
        public abstract int baseTimesToFire { get; }

        public abstract int baseRingToFire { get; }

        public abstract int additionalRingsMin { get; }

        public abstract int additionalRingsMax { get; }

        public abstract int additionalTimesMin { get; }

        public abstract int additionalTimesMax { get; }

        public abstract bool spawnShadowEffect { get; }

        public static GameObject cloneEffectPrefab;

        public static float baseSingleRingDuration = 1.666f;

        public static float baseEffectDelayDuration = 0.3f;

        public static float baseShadowEffectDelay = 0.8f;

        public static string[] effectList = new string[]
        {
        "Ring1", "Ring2", "Ring3", "Ring4", "Ring5"
        };

        public static string[] hitboxList = new string[]
        {
        "FirstRing", "SecondRing", "ThirdRing", "FourthRing", "FifthRing"
        };

        public static float baseDamage = 2f;

        private static int[][] rngTable = new int[][]
        {
            new int[] {3, 0, 4, 2},
            new int[] {2, 4, 1, 0},
            new int[] {1, 3, 2, 0},
            new int[] {3, 0, 1, 4},
            new int[] {4, 2, 3, 1},
            new int[] {1, 4, 0, 3},
            new int[] {2, 4, 3, 0},
            new int[] {4, 0, 2, 1},
            new int[] {0, 3, 4, 1},
            new int[] {2, 0, 3, 1},
            new int[] {4, 1, 0, 3},
            new int[] {3, 1, 4, 2},
            new int[] {0, 3, 2, 4},
            new int[] {1, 0, 4, 3},
            new int[] {2, 4, 0, 1}
        };

        private Transform modelTransform;

        private ChildLocator locator;

        private OverlapAttackAuthority overlapAttack;

        private int[] currentRings;

        private float oneRingTimer;

        private float effectDelayBetweenAttacksTimer;

        private float shadowEffectDelayTimer;

        private bool attackFired;

        private int timesFired;

        private int timesToFire;

        private int ringsToFire;

        private int startingArray;

        private bool shadowEffectSpawned;

        private ChildLocator modelChildLocator;

        private Transform muzzleFloor;

        public override void OnEnter()
        {
            base.OnEnter();

            if (isAuthority)
            {
                startingArray = UnityEngine.Random.Range(0, rngTable.Length);
                timesToFire = baseTimesToFire + (int)Mathf.Clamp(Util.Remap(healthComponent.health, healthComponent.fullHealth * 0.3f, healthComponent.fullHealth * 0.8f, (float)additionalTimesMax, (float)additionalTimesMin), 1, baseTimesToFire + additionalTimesMax);
                ringsToFire = baseRingToFire + (int)Mathf.Clamp(Util.Remap(healthComponent.health, healthComponent.fullHealth * 0.3f, healthComponent.fullHealth * 0.8f, (float)additionalRingsMax, (float)additionalRingsMin), 1, rngTable[0].Length);
            }

            locator = GetModelChildLocator();
            modelTransform = GetModelTransform();
            overlapAttack = SetupOverlapAttack();
            PlayAnimation();
            SetupNewRings();

            modelChildLocator = GetModelChildLocator();

            oneRingTimer += baseSingleRingDuration;
            muzzleFloor = FindModelChild("MuzzleFloor");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            oneRingTimer -= GetDeltaTime();
            effectDelayBetweenAttacksTimer -= GetDeltaTime();
            shadowEffectDelayTimer -= GetDeltaTime();
            if (oneRingTimer <= 0)
            {
                FireRing();
                SetEffects(false);
                PlayAnimation();

                attackFired = true;
                oneRingTimer += baseSingleRingDuration;
                effectDelayBetweenAttacksTimer = baseEffectDelayDuration;

                shadowEffectDelayTimer = baseShadowEffectDelay;
                shadowEffectSpawned = false;

                timesFired++;
            }
            if(spawnShadowEffect && !shadowEffectSpawned && shadowEffectDelayTimer <= 0)
            {
                SpawnGhostEffect();
                shadowEffectSpawned = true;
            }
            if (attackFired && effectDelayBetweenAttacksTimer <= 0)
            {
                SetupNewRings();
                attackFired = false;
            }
            if (timesFired >= timesToFire && isAuthority)
            {
                SetNextStateAuthority();
            }
        }

        public abstract void SetNextStateAuthority();

        private void SpawnGhostEffect()
        {
            var effectData = new EffectData()
            {
                rootObject = base.gameObject,
                modelChildIndex = (short)modelChildLocator.FindChildIndex(muzzleFloor),
                origin = muzzleFloor.position
            };

            EffectManager.SpawnEffect(cloneEffectPrefab, effectData, false);
        }

        public override void OnExit()
        {
            base.OnExit();
            SetEffects(false);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)startingArray);
            writer.Write((byte)timesToFire);
            writer.Write((byte)ringsToFire);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            startingArray = reader.ReadByte();
            timesToFire = reader.ReadByte();
            ringsToFire = reader.ReadByte();
        }

        private OverlapAttackAuthority SetupOverlapAttack()
        {
            return new OverlapAttackAuthority()
            {
                attacker = base.gameObject,
                attackerFiltering = RoR2.AttackerFiltering.NeverHitSelf,
                inflictor = base.gameObject,
                teamIndex = TeamComponent.GetObjectTeam(gameObject),
                damage = baseDamage * damageStat,
                isCrit = RollCrit(),
                retriggerTimeout = 1f
            };
        }

        private void PlayAnimation()
        {
            PlayCrossfade("Gesture, Override", "RingLoopStart", 0.1f);
        }

        private void SetupNewRings()
        {
            currentRings = rngTable[startingArray].Take(ringsToFire).ToArray();
            SetEffects(true);
            startingArray = (startingArray + 1) % rngTable.Length;
        }

        private void SetEffects(bool active)
        {
            for (int i = 0; i < currentRings.Length; i++)
            {
                var child = locator.FindChild(effectList[currentRings[i]]);
                if (child)
                {
                    child.gameObject.SetActive(active);
                }
            }
        }

        private void FireRing()
        {
            var hitBoxes = modelTransform.GetComponents<HitBoxGroup>();
            List<HurtBox> hits = new List<HurtBox>();
            for (int i = 0; i < currentRings.Length; i++)
            {
                int number = currentRings[i];

                overlapAttack.hitBoxGroup = Array.Find(hitBoxes, (element) => element.groupName == hitboxList[number]);

                if (overlapAttack.Fire(hits))
                {
                    foreach (HurtBox box in hits)
                    {
                        if (!box || !box.healthComponent)
                        {
                            continue;
                        }
                        overlapAttack.addIgnoredHitList(box.healthComponent);
                    }
                    hits.Clear();
                }
            }
        }
    }
}
