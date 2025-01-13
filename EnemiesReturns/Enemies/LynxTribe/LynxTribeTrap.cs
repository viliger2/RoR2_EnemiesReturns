using EnemiesReturns.Behaviors;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;

namespace EnemiesReturns.Enemies.LynxTribe
{
    // just do everything on server, who cares
    public class LynxTribeTrap : MonoBehaviour
    {
        public float checkInterval = 0.25f;

        public float spawnAfterTriggerInterval = 0.5f;

        public LynxTribeSpawner spawner;

        public DestroyOnTimer destroyOnTimer;

        public Transform hitBox;

        public Transform leaves;

        public Transform branches;

        public TeamMask teamFilter;

        public string initialTriggerSound;

        private float timer;

        private bool triggered;

        private bool spawned;

        private void Awake()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!hitBox)
            {
                Log.Warning($"LynxTribeTrap on {gameObject} doesn't have hitBox! Destroying itself.");
                NetworkServer.Destroy(this.gameObject);
                return;
            }
        }

        private void Start()
        {
            if (leaves)
            {
                foreach(Transform child in leaves)
                {
                    foreach (Transform child2 in child)
                    {
                        //Vector3 oldRotation = child.localRotation.eulerAngles;
                        child2.localRotation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));
                    }
                }
            }

            if (branches)
            {
                branches.localRotation = Quaternion.Euler(new Vector3(0f, UnityEngine.Random.Range(0f, 360f), 0f));
            }

            if (!NetworkServer.active)
            {
                this.enabled = false;
            }
        }

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if(timer > checkInterval && !triggered)
            {
                var position = hitBox.position;
                var halfExtends = hitBox.lossyScale * 0.5f;
                var rotation = transform.rotation;

                Collider[] colliders;
                int num = HGPhysics.OverlapBox(out colliders, position, halfExtends, rotation, LayerIndex.entityPrecise.mask);
                for(int i = 0; i < num; i++)
                {
                    if (!colliders[i])
                    {
                        continue;
                    }

                    var hurtBox = colliders[i].GetComponent<HurtBox>();
                    if(!hurtBox || !hurtBox.healthComponent || !hurtBox.healthComponent.body)
                    {
                        continue;
                    }

                    if (teamFilter.HasTeam(hurtBox.healthComponent.body.teamComponent.teamIndex))
                    {
                        if (!string.IsNullOrEmpty(initialTriggerSound))
                        {
                            EntitySoundManager.EmitSoundServer((AkEventIdArg)initialTriggerSound, base.gameObject);
                        }
                        triggered = true;
                        break;
                    }
                }
                HGPhysics.ReturnResults(colliders);
                // resetting the timer so we can spawn cards on new timer
                timer = triggered ? 0 : timer - checkInterval;
            }

            if(timer > spawnAfterTriggerInterval && triggered && !spawned)
            {
                if (spawner)
                {
                    spawner.SpawnLynxTribesmen(transform);
                }
                spawned = true;
                if (destroyOnTimer)
                {
                    destroyOnTimer.enabled = true;
                }
            }
        }
    }
}
