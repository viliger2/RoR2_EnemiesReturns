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
    public class FireRings : BaseState
    {
        public static int timesToFire => 3;

        public static int ringToFire => 2;

        public static float baseSingleRingDuration = 1.666f;

        public static float baseEffectDelayDuration = 0.3f;

        public static string[] effectList = new string[]
        {
            "Ring1", "Ring2", "Ring3", "Ring4", "Ring5"
        };

        public static string[] hitboxList = new string[]
        {
            "FirstRing", "SecondRing", "ThirdRing", "FourthRing", "FifthRing"
        };

        public static float baseDamage = 2f;

        private static int[] rngArray = new int[] { 0, 1, 2, 3, 4 };

        private Transform modelTransform;

        private ChildLocator locator;

        private OverlapAttackAuthority overlapAttack;

        private int[] currentRings;

        private float oneRingTimer;

        private float effectDelayBetweenAttacksTimer;

        private bool attackFired;

        private int timesFired;

        private ulong seed;

        private Xoroshiro128Plus rngInstance;

        public override void OnEnter()
        {
            base.OnEnter();

            if (isAuthority)
            {
                seed = (ulong)DateTime.UtcNow.Ticks;
            }
            rngInstance = new Xoroshiro128Plus(seed);

            locator = GetModelChildLocator();
            modelTransform = GetModelTransform();
            overlapAttack = SetupOverlapAttack();
            PlayAnimation();
            SetupNewRings();

            oneRingTimer += baseSingleRingDuration;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            oneRingTimer -= GetDeltaTime();
            effectDelayBetweenAttacksTimer -= GetDeltaTime();
            if (oneRingTimer <= 0)
            {
                FireRing();
                SetEffects(false);
                PlayAnimation();

                attackFired = true;
                oneRingTimer += baseSingleRingDuration;
                effectDelayBetweenAttacksTimer = baseEffectDelayDuration;
                timesFired++;
            }
            if(attackFired && effectDelayBetweenAttacksTimer <= 0)
            {
                SetupNewRings();
                attackFired = false;
            }
            if(timesFired >= timesToFire && isAuthority)
            {
                outer.SetNextState(new OutroFireRings());
            }
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
            writer.Write(seed);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            seed = reader.ReadUInt64();
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
            currentRings = rngArray.OrderBy(_ => rngInstance.Next()).Take(ringToFire).ToArray();
            SetEffects(true);
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

        //public static int timesToFire => Configuration.General.ProvidenceP1SpecialTimesToFire.Value;

        //public static int ringToFire => Configuration.General.ProvidenceP1SpecialRingsToFire.Value;

        //public static string[] effectList = new string[]
        //{
        //    "Ring1", "Ring2", "Ring3", "Ring4", "Ring5"
        //};

        //public static string[] hitboxList = new string[]
        //{
        //    "FirstRing", "SecondRing", "ThirdRing", "FourthRing", "FifthRing"
        //};

        //public static float delayBetweenRings => Configuration.General.ProvidenceP1SpecialDelayBetweenRings.Value;

        //public static float baseOneRingDuration => Configuration.General.ProvidenceP1SpecialOneRingDuration.Value;

        //public static float initialAnimationDuration = 0.875f;

        //public static float baseDamage = 2f;

        //private static int[] rngArray = new int[] { 0, 1, 2, 3, 4 };

        //private int timesFired;

        //private ChildLocator locator;

        //private int[] currentRings;

        //private float oneRingTimer;

        //private float inbetweenTimer;

        //private bool ringFired;

        //private Transform modelTransform;

        //private OverlapAttackAuthority overlapAttack;

        //public override void OnEnter()
        //{
        //    base.OnEnter();
        //    locator = GetModelChildLocator();
        //    modelTransform = GetModelTransform();
        //    overlapAttack = SetupOverlapAttack();
        //    PlayInitialAnimation();
        //    SetupNewRings();

        //    oneRingTimer += initialAnimationDuration;
        //}

        //public override void FixedUpdate()
        //{
        //    base.FixedUpdate();
        //    if (timesFired >= timesToFire)
        //    {
        //        outer.SetNextState(new OutroFireRings());
        //    }
        //    if (oneRingTimer <= 0f && !ringFired)
        //    {
        //        FireRing();
        //        SetEffects(false);

        //        ringFired = true;
        //        inbetweenTimer = delayBetweenRings;
        //        timesFired++;
        //    }
        //    if (ringFired)
        //    {
        //        if (inbetweenTimer <= 0f)
        //        {
        //            SetupNewRings();
        //            PlayAnimation();
        //            ringFired = false;
        //        }
        //        inbetweenTimer -= GetDeltaTime();
        //    }
        //    oneRingTimer -= GetDeltaTime();
        //}

        //public override void OnExit()
        //{
        //    base.OnExit();
        //    PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        //}

        //public override InterruptPriority GetMinimumInterruptPriority()
        //{
        //    return InterruptPriority.Death;
        //}

        //private void SetupNewRings()
        //{
        //    currentRings = rngArray.OrderBy(_ => RoR2.Run.instance.stageRng.Next()).Take(ringToFire).ToArray();
        //    SetEffects(true);
        //    oneRingTimer += baseOneRingDuration;
        //}

        //private void SetEffects(bool active)
        //{
        //    for (int i = 0; i < currentRings.Length; i++)
        //    {
        //        var child = locator.FindChild(effectList[currentRings[i]]);
        //        if (child)
        //        {
        //            child.gameObject.SetActive(active);
        //        }
        //    }
        //}

        //private void PlayAnimation()
        //{
        //    PlayAnimation("Gesture, Override", "FireRing");
        //}

        //private void PlayInitialAnimation()
        //{
        //    PlayAnimation("Gesture, Override", "IntroFireRing");
        //}

        //private OverlapAttackAuthority SetupOverlapAttack()
        //{
        //    return new OverlapAttackAuthority()
        //    {
        //        attacker = base.gameObject,
        //        attackerFiltering = RoR2.AttackerFiltering.NeverHitSelf,
        //        inflictor = base.gameObject,
        //        teamIndex = TeamComponent.GetObjectTeam(gameObject),
        //        damage = baseDamage * damageStat,
        //        isCrit = RollCrit(),
        //        retriggerTimeout = 1f
        //    };
        //}

        //private void FireRing()
        //{
        //    var hitBoxes = modelTransform.GetComponents<HitBoxGroup>();
        //    List<HurtBox> hits = new List<HurtBox>();
        //    for (int i = 0; i < currentRings.Length; i++)
        //    {
        //        int number = currentRings[i];

        //        overlapAttack.hitBoxGroup = Array.Find(hitBoxes, (element) => element.groupName == hitboxList[number]);

        //        if (overlapAttack.Fire(hits))
        //        {
        //            foreach (HurtBox box in hits)
        //            {
        //                if (!box || !box.healthComponent)
        //                {
        //                    continue;
        //                }
        //                overlapAttack.addIgnoredHitList(box.healthComponent);
        //            }
        //            hits.Clear();
        //        }
        //    }
        //}
    }
}
