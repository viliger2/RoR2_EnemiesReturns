using EnemiesReturns.Projectiles;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Special
{
    [RegisterEntityState]
    public class FireRings : BaseState
    {
        public static int timesToFire => Configuration.General.ProvidenceP1SpecialTimesToFire.Value;

        public static int ringToFire => Configuration.General.ProvidenceP1SpecialRingsToFire.Value;

        public static string[] effectList = new string[]
        {
            "Ring1", "Ring2", "Ring3", "Ring4", "Ring5"
        };

        public static string[] hitboxList = new string[]
        {
            "FirstRing", "SecondRing", "ThirdRing", "FourthRing", "FifthRing"
        };

        public static float delayBetweenRings => Configuration.General.ProvidenceP1SpecialDelayBetweenRings.Value;

        public static float baseOneRingDuration => Configuration.General.ProvidenceP1SpecialOneRingDuration.Value;

        public static float baseDamage = 2f;

        private static int[] rngArray = new int[] { 0, 1, 2, 3, 4 };

        private int timesFired;

        private ChildLocator locator;

        private int[] currentRings;

        private float oneRingTimer;

        private float inbetweenTimer;

        private bool ringFired;

        private Transform modelTransform;

        private OverlapAttackAuthority overlapAttack;

        public override void OnEnter()
        {
            base.OnEnter();
            locator = GetModelChildLocator();
            modelTransform = GetModelTransform();
            overlapAttack = SetupOverlapAttack();
            PlayAnimation();
            SetupNewRings();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (timesFired >= timesToFire)
            {
                outer.SetNextStateToMain();
            }
            if (oneRingTimer <= 0f && !ringFired)
            {
                FireRing();
                SetEffects(false);

                ringFired = true;
                inbetweenTimer = delayBetweenRings;
                timesFired++;
            }
            if (ringFired)
            {
                if (inbetweenTimer <= 0f)
                {
                    SetupNewRings();
                    PlayAnimation();
                    ringFired = false;
                }
                inbetweenTimer -= GetDeltaTime();
            }
            oneRingTimer -= GetDeltaTime();
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private void SetupNewRings()
        {
            currentRings = rngArray.OrderBy(_ => RoR2.Run.instance.stageRng.Next()).Take(ringToFire).ToArray();
            SetEffects(true);
            oneRingTimer += baseOneRingDuration;
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

        private void PlayAnimation()
        {
            PlayAnimation("Gesture, Override", "Slash3", "combo.playbackRate", baseOneRingDuration);
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
