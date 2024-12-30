using KinematicCharacterController;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace EnemiesReturns.Enemies.LynxTribe.Storm
{
    public class LynxStormComponent : MonoBehaviour
    {
        public static float baseDuration => Configuration.LynxTribe.LynxShaman.SummonStormGrabDuration.Value;

        // magic numbers are used in logariphmic spiral calculation
        // so grabbed targets stay approximately within storm's default radius 
        public static float baseA = 0.9f; // magic number 1

        public static float baseB = 0.3f; // magic number 2

        public static float baseForce => Configuration.LynxTribe.LynxShaman.SummonStormThrowForce.Value;

        public static float pullStr = 10f;

        public static float velocitySpeed = 10f;

        public float yHeight = 8f;

        public float immunityDuration => Configuration.LynxTribe.LynxShaman.SummonStormImmunityDuration.Value;

        public GameObject storm;

        private float timer;

        private float duration;
        private float force;
        private float a;
        private float b;

        private CharacterMotor characterMotor;
        private CharacterBody characterBody;
        private Rigidbody rigidbody;
        private GameObject moveTarget;
        private Vector3 previousPosition;

        private void Awake()
        {
            duration = baseDuration + UnityEngine.Random.Range(-0.2f, 0.2f);
            force = baseForce + UnityEngine.Random.Range(-300f, 300f);
            a = baseA + UnityEngine.Random.Range(-0.05f, 0.05f);
            b = baseB + UnityEngine.Random.Range(-0.05f, 0.05f);

            characterBody = GetComponent<CharacterBody>();
            if (!characterBody || !characterBody.hasEffectiveAuthority)
            {
                Destroy(this);
                return;
            }
            characterMotor = GetComponent<CharacterMotor>();
            if (!characterMotor)
            {
                Destroy(this);
                return;
            }
            rigidbody = GetComponent<Rigidbody>();
            characterMotor.Motor.ForceUnground();
            characterMotor.useGravity = false;
            characterMotor.disableAirControlUntilCollision = true;
            moveTarget = new GameObject();
            if (storm)
            {
                moveTarget.transform.parent = storm.transform;
                moveTarget.transform.localPosition = Vector3.zero;
            }
            characterMotor.velocity = Vector3.zero;
            Log.Info("grabbed");
        }

        public void SetStormTransform(GameObject storm)
        {
            this.storm = storm;
            moveTarget.transform.parent = storm.transform;
            moveTarget.transform.localPosition = Vector3.zero;
        }

        private void Update()
        {
            //Log.Info($"velocity: {characterMotor.velocity}, combined velocity: {characterMotor.velocity.sqrMagnitude}, root motion: {characterMotor.rootMotion}");
            //Log.Info($"distance: {Vector3.Distance(storm.transform.position, characterMotor.Motor.transform.position)}");
        }

        private void FixedUpdate()
        {
            if (!characterBody.hasEffectiveAuthority)
            {
                return;
            }

            if (!storm)
            {
                // just release without anything for now
                Destroy(this);
                return;
            }

            //if (Vector3.Distance() > )
            //{
            //    Destroy(this);
            //    return;
            //}

            if (timer > duration)
            {
                if (characterMotor)
                {
                    characterMotor.useGravity = true;
                    var normalized2 = (characterMotor.transform.position - previousPosition).normalized;
                    var forceVector = new Vector3(normalized2.x, 0f, normalized2.z) * force;
                    characterMotor.ApplyForce(forceVector, true, false);
                    if (rigidbody)
                    {
                        rigidbody.AddForce(forceVector, ForceMode.Impulse);
                    }
                }
                R2API.Networking.NetworkingHelpers.ApplyDot(characterBody.healthComponent, GetAttacker(), DotController.DotIndex.Bleed, 5f); // TODO
                Destroy(this);
                return;
            }

            previousPosition = characterMotor.previousPosition;
            timer += Time.fixedDeltaTime;
            var angle = Mathf.PI * timer;
            var r = a * Mathf.Pow((float)Math.E, b * angle);
            moveTarget.transform.localPosition = new Vector3(r * Mathf.Cos(angle), 2f + yHeight * (timer / baseDuration), r * Mathf.Sin(angle));
            var target = (moveTarget.transform.position - characterMotor.previousPosition).normalized;
            characterMotor.disableAirControlUntilCollision = true;
            characterMotor.velocity = target * velocitySpeed;
        }

        private GameObject GetAttacker()
        {
            if (storm)
            {
                var stormBody = storm.GetComponent<CharacterBody>();

                if (stormBody.master)
                {
                    var aiOwnership = stormBody.master.gameObject.GetComponent<AIOwnership>();
                    if (aiOwnership && aiOwnership.ownerMaster)
                    {
                        var body = aiOwnership.ownerMaster.GetBody();
                        if (body)
                        {
                            return body.gameObject;
                        }
                    }
                }
            }
            return storm;
        }

        private void OnDisable()
        {
            if (characterMotor)
            {
                characterMotor.useGravity = true;
                characterMotor.disableAirControlUntilCollision = false;
            }
            if (characterBody)
            {
                R2API.Networking.NetworkingHelpers.ApplyBuff(characterBody, LynxStormStuff.StormImmunity.buffIndex, 1, immunityDuration);
            }
        }
    }
}
