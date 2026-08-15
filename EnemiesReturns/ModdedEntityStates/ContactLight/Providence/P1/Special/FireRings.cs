using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseRings;
using EnemiesReturns.Projectiles;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Special
{
    [RegisterEntityState]
    public class FireRings : BaseFireRings
    {
        public override int baseTimesToFire => 3;

        public override int baseRingToFire => 2;

        public override int additionalRingsMin => 0;

        public override int additionalRingsMax => 0;

        public override int additionalTimesMin => 0;

        public override int additionalTimesMax => 0;

        public override bool spawnShadowEffect => false;

        public override void SetNextStateAuthority()
        {
            outer.SetNextState(new OutroFireRings());
        }
    }
}
//        public static int baseTimesToFire => 3;

//        public static int baseRingToFire => 2;

//        public static int additionalRingsMin = 0;

//        public static int additionalRingsMax = 0;

//        public static int additionalTimesMin = 0;

//        public static int additionalTimesMax = 0;

//        public static float baseSingleRingDuration = 1.666f;

//        public static float baseEffectDelayDuration = 0.3f;

//        public static string[] effectList = new string[]
//        {
//            "Ring1", "Ring2", "Ring3", "Ring4", "Ring5"
//        };

//        public static string[] hitboxList = new string[]
//        {
//            "FirstRing", "SecondRing", "ThirdRing", "FourthRing", "FifthRing"
//        };

//        public static float baseDamage = 2f;

//        private static int[][] rngTable = new int[][]
//        {
//            new int[] {0,2,3,4 },
//            new int[] {0,1,2,4 },
//            new int[] {0,1,2,3 },
//            new int[] {0,1,3,4 },
//            new int[] {1,2,3,4 },
//            new int[] {0,1,3,4 },
//            new int[] {0,2,3,4 },
//            new int[] {0,1,2,4 },
//            new int[] {0,1,3,4 },
//            new int[] {0,1,2,3 },
//            new int[] {0,1,3,4 },
//            new int[] {1,2,3,4 },
//            new int[] {0,2,3,4 },
//            new int[] {0,1,3,4 },
//            new int[] {0,1,2,4 }
//        };

//        private Transform modelTransform;

//        private ChildLocator locator;

//        private OverlapAttackAuthority overlapAttack;

//        private int[] currentRings;

//        private float oneRingTimer;

//        private float effectDelayBetweenAttacksTimer;

//        private bool attackFired;

//        private int timesFired;

//        private int timesToFire;

//        private int ringsToFire;

//        private int startingArray;

//        public override void OnEnter()
//        {
//            base.OnEnter();

//            if (isAuthority)
//            {
//                startingArray = UnityEngine.Random.Range(0, rngTable.Length);
//                timesToFire = baseTimesToFire + (int)Mathf.Clamp(Util.Remap(healthComponent.health, healthComponent.fullHealth * 0.3f, healthComponent.fullHealth * 0.8f, (float)additionalTimesMax, (float)additionalTimesMin), 1, baseTimesToFire + additionalTimesMax);
//                ringsToFire = baseRingToFire + (int)Mathf.Clamp(Util.Remap(healthComponent.health, healthComponent.fullHealth * 0.3f, healthComponent.fullHealth * 0.8f, (float)additionalRingsMax, (float)additionalRingsMin), 1, rngTable[0].Length);
//            }

//            locator = GetModelChildLocator();
//            modelTransform = GetModelTransform();
//            overlapAttack = SetupOverlapAttack();
//            PlayAnimation();
//            SetupNewRings();

//            oneRingTimer += baseSingleRingDuration;
//        }

//        public override void FixedUpdate()
//        {
//            base.FixedUpdate();
//            oneRingTimer -= GetDeltaTime();
//            effectDelayBetweenAttacksTimer -= GetDeltaTime();
//            if (oneRingTimer <= 0)
//            {
//                FireRing();
//                SetEffects(false);
//                PlayAnimation();

//                attackFired = true;
//                oneRingTimer += baseSingleRingDuration;
//                effectDelayBetweenAttacksTimer = baseEffectDelayDuration;
//                timesFired++;
//            }
//            if(attackFired && effectDelayBetweenAttacksTimer <= 0)
//            {
//                SetupNewRings();
//                attackFired = false;
//            }
//            if(timesFired >= timesToFire && isAuthority)
//            {
//                outer.SetNextState(new OutroFireRings());
//            }
//        }

//        public override void OnExit()
//        {
//            base.OnExit();
//            SetEffects(false);
//        }

//        public override InterruptPriority GetMinimumInterruptPriority()
//        {
//            return InterruptPriority.Death;
//        }

//        public override void OnSerialize(NetworkWriter writer)
//        {
//            base.OnSerialize(writer);
//            writer.Write((byte)startingArray);
//            writer.Write((byte)timesToFire);
//            writer.Write((byte)ringsToFire);
//        }

//        public override void OnDeserialize(NetworkReader reader)
//        {
//            base.OnDeserialize(reader);
//            startingArray = reader.ReadByte();
//            timesToFire = reader.ReadByte();
//            ringsToFire = reader.ReadByte();
//        }

//        private OverlapAttackAuthority SetupOverlapAttack()
//        {
//            return new OverlapAttackAuthority()
//            {
//                attacker = base.gameObject,
//                attackerFiltering = RoR2.AttackerFiltering.NeverHitSelf,
//                inflictor = base.gameObject,
//                teamIndex = TeamComponent.GetObjectTeam(gameObject),
//                damage = baseDamage * damageStat,
//                isCrit = RollCrit(),
//                retriggerTimeout = 1f
//            };
//        }

//        private void PlayAnimation()
//        {
//            PlayCrossfade("Gesture, Override", "RingLoopStart", 0.1f);        
//        }

//        private void SetupNewRings()
//        {
//            currentRings = rngTable[startingArray].OrderBy(_ => RoR2.Run.instance.stageRng.Next()).Take(ringsToFire).ToArray();
//            SetEffects(true);
//            startingArray = (startingArray + 1) % rngTable.Length;
//        }

//        private void SetEffects(bool active)
//        {
//            for (int i = 0; i < currentRings.Length; i++)
//            {
//                var child = locator.FindChild(effectList[currentRings[i]]);
//                if (child)
//                {
//                    child.gameObject.SetActive(active);
//                }
//            }
//        }

//        private void FireRing()
//        {
//            var hitBoxes = modelTransform.GetComponents<HitBoxGroup>();
//            List<HurtBox> hits = new List<HurtBox>();
//            for (int i = 0; i < currentRings.Length; i++)
//            {
//                int number = currentRings[i];

//                overlapAttack.hitBoxGroup = Array.Find(hitBoxes, (element) => element.groupName == hitboxList[number]);

//                if (overlapAttack.Fire(hits))
//                {
//                    foreach (HurtBox box in hits)
//                    {
//                        if (!box || !box.healthComponent)
//                        {
//                            continue;
//                        }
//                        overlapAttack.addIgnoredHitList(box.healthComponent);
//                    }
//                    hits.Clear();
//                }
//            }
//        }
//    }
//}
