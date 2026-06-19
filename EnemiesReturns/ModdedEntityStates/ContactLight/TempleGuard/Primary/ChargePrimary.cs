using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.TempleGuard.Primary
{
    [RegisterEntityState]
    public class ChargePrimary : BaseState
    {
        public static float baseDuration => 2f;

        public static GameObject effectPrefab;

        private float duration;

        private List<GameObject> chargeEffects = new List<GameObject>();

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            if (effectPrefab)
            {
                SpawnEffect("CannonR");
                SpawnEffect("CannonL");
            }

            PlayCrossfade("Gesture, Override", "PrepBarrage", 0.1f);

            void SpawnEffect(string childName)
            {
                var parent = FindModelChild(childName);
                if (parent)
                {
                    var newObject = UnityEngine.Object.Instantiate(effectPrefab, parent.position, parent.rotation);
                    newObject.transform.parent = parent;

                    ScaleParticleSystemDuration component2 = newObject.GetComponent<ScaleParticleSystemDuration>();
                    if ((bool)component2)
                    {
                        component2.newDuration = duration;
                    }
                    chargeEffects.Add(newObject);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new FirePrimary());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            for (int i = 0; i < chargeEffects.Count; i++)
            {
                if ((bool)chargeEffects[i])
                {
                    Destroy(chargeEffects[i]);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
