using EnemiesReturns.Enemies.Colossus;
using EntityStates;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace EnemiesReturns.ModdedEntityStates.Colossus.RockClap
{
    public class RockClapEnd : BaseState
    {
        public static GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Grandparent/GrandparentMiniBoulder.prefab").WaitForCompletion();

        public static float baseDuration = 2.2f;

        public static float damageCoefficient = 3f;

        public static float forceMagnitude = 3000f;

        public static float speed = 50f;

        private Animator modelAnimator;

        private float duration;

        private bool hasFired;

        private FloatingRocksController rockController;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            modelAnimator = GetModelAnimator();
            rockController = GetModelTransform().gameObject.GetComponent<FloatingRocksController>();
            PlayCrossfade("Gesture, Override", "ClapEnd", "Clap.playbackrate", duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (modelAnimator && modelAnimator.GetFloat("Clap.activate") >= 0.8f && !hasFired)
            {
                if (rockController)
                {
                    foreach (GameObject rock in rockController.floatingRocks)
                    {
                        var position = rock.transform.position;
                        var rotation = Quaternion.LookRotation(rock.transform.position - modelLocator.modelTransform.position, Vector3.up);
                        ProjectileManager.instance.FireProjectile(projectilePrefab, position, rotation, gameObject, damageStat * damageCoefficient, forceMagnitude, RollCrit(), RoR2.DamageColorIndex.Default, null, speed);
                    }
                    rockController.enabled = false;
                }
                hasFired = true;
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
