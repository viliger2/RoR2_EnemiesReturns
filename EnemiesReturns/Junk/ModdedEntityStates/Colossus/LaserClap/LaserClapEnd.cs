using EnemiesReturns.Enemies.Colossus;
using EntityStates;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace EnemiesReturns.Junk.ModdedEntityStates.Colossus.LaserClap
{
    public class LaserClapEnd : BaseState
    {
        public static GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanRockProjectile.prefab").WaitForCompletion();

        public static float baseDuration = 2.2f;

        public static float damageCoefficient = 3f;

        public static float forceMagnitude = 3000f;

        public static float speed = 50f;

        public static int laserCount = 30; // yeah, overkill, but we are testin

        private Animator modelAnimator;

        private float duration;

        private bool hasFired;

        private Transform laserStart;

        private Transform laserDirectionPoint;

        //private FloatingRocksController rockController;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            modelAnimator = GetModelAnimator();
            laserStart = modelLocator.modelTransform.Find("laserStartPoint");
            laserDirectionPoint = modelLocator.modelTransform.Find("laserStartPoint/laserDirectionPoint");
            //rockController = GetModelTransform().gameObject.GetComponent<FloatingRocksController>();
            PlayCrossfade("Gesture, Override", "ClapEnd", "Clap.playbackrate", duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (modelAnimator && modelAnimator.GetFloat("Clap.activate") >= 0.8f && !hasFired)
            {
                for (int i = 0; i < laserCount; i++)
                {
                    var position = laserStart.position;
                    laserDirectionPoint.localPosition = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(-1f, 1f));
                    var rotation = Quaternion.LookRotation(laserDirectionPoint.position - laserStart.position, Vector3.up);
                    ProjectileManager.instance.FireProjectile(projectilePrefab, position, rotation, gameObject, damageStat * damageCoefficient, forceMagnitude, RollCrit(), RoR2.DamageColorIndex.Default, null, speed);
                }


                //if(rockController)
                //{
                //    foreach (GameObject rock in rockController.floatingRocks)
                //    {
                //        var position = rock.transform.position;
                //        var rotation = Quaternion.LookRotation(rock.transform.position - modelLocator.modelTransform.position, Vector3.up);
                //        //var rotation = Quaternion.FromToRotation(modelLocator.modelTransform.position, rock.transform.position);
                //        ProjectileManager.instance.FireProjectile(projectilePrefab, position, rotation, base.gameObject, damageStat * damageCoefficient, forceMagnitude, RollCrit(), RoR2.DamageColorIndex.Default, null, speed);
                //    }
                //    rockController.enabled = false;
                //}
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
