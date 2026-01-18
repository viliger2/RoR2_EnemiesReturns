using EnemiesReturns.Projectiles;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Special
{
    [RegisterEntityState]
    public class FireRingsWithClones : BaseState
    {
        public static int baseTimesToFire => Configuration.General.ProvidenceP1SpecialTimesToFire.Value;

        public static int baseRingToFire => Configuration.General.ProvidenceP1SpecialRingsToFire.Value;

        public static GameObject cloneEffectPrefab;

        public static int additionalRingsMin = 1;

        public static int additionalRingsMax = 2;

        public static int additionalTimesMin = 1;

        public static int additionalTimesMax = 3;

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

        private int timesToFire;

        private int ringsToFire;

        private bool spawnedClone;

        private ChildLocator modelChildLocator;

        private Transform muzzleFloor;

        public override void OnEnter()
        {
            base.OnEnter();
            locator = GetModelChildLocator();
            modelTransform = GetModelTransform();
            overlapAttack = SetupOverlapAttack();

            timesToFire = baseTimesToFire + (int)Mathf.Clamp(Util.Remap(healthComponent.health, healthComponent.fullHealth * 0.3f, healthComponent.fullHealth * 0.8f, (float)additionalTimesMax, (float)additionalTimesMin), 1, baseTimesToFire + additionalTimesMax);
            ringsToFire = baseRingToFire + (int)Mathf.Clamp(Util.Remap(healthComponent.health, healthComponent.fullHealth * 0.3f, healthComponent.fullHealth * 0.8f, (float)additionalRingsMax, (float)additionalRingsMin), 1, rngArray.Length);

            PlayAnimation();
            SetupNewRings();

            modelChildLocator = GetModelChildLocator();
            muzzleFloor = FindModelChild("MuzzleFloor");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(timesFired >= timesToFire)
            {
                outer.SetNextStateToMain();
            }
            if(oneRingTimer <= 0f && !ringFired)
            {
                FireRing();
                SetEffects(false);

                ringFired = true;
                inbetweenTimer = delayBetweenRings;
                timesFired++;
            }
            if (ringFired)
            {
                if(inbetweenTimer <= 0f)
                {
                    SetupNewRings();
                    PlayAnimation();
                    ringFired = false;
                }
                inbetweenTimer -= GetDeltaTime();
            }
            if(oneRingTimer <= baseOneRingDuration / 2f && !spawnedClone)
            {
                SpawnGhostEffect();
                spawnedClone = true;
            }
            oneRingTimer -= GetDeltaTime();
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

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

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private void SetupNewRings()
        {
            currentRings = rngArray.OrderBy(_ => RoR2.Run.instance.stageRng.Next()).Take(ringsToFire).ToArray();
            SetEffects(true);
            oneRingTimer += baseOneRingDuration;
            spawnedClone = false;
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
                    foreach(HurtBox box in hits)
                    {
                        if(!box || !box.healthComponent)
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
