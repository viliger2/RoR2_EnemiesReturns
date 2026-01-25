using EnemiesReturns.Projectiles;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3
{
    [RegisterEntityState]
    public class FireRings : BaseState
    {
        public static int timesToFire = 1;

        public static int ringToFire = 2;

        public static string[] effectList = new string[]
        {
            "Ring1", "Ring2", "Ring3", "Ring4", "Ring5"
        };

        public static string[] hitboxList = new string[]
        {
            "FirstRing", "SecondRing", "ThirdRing", "FourthRing", "FifthRing"
        };

        public static float delayBetweenRings => 1f;

        public static float baseOneRingDuration => 1f;

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
                    ringFired = false;
                }
                inbetweenTimer -= GetDeltaTime();
            }
            oneRingTimer -= GetDeltaTime();
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
