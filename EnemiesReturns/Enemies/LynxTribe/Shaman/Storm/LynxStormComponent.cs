using KinematicCharacterController;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace EnemiesReturns.Enemies.LynxTribe.Shaman.Storm
{
    public class LynxStormComponent : MonoBehaviour
    {
        public static float baseDuration => EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonStormGrabDuration.Value;

        // magic numbers are used in logariphmic spiral calculation
        // so grabbed targets stay approximately within storm's default radius 
        public static float baseA = 0.7f; // magic number 1

        public static float baseB = 0.2f; // magic number 2

        public static float baseForce => EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonStormThrowForce.Value;

        public float yHeight = 8f;

        public GameObject storm;

        private float timer;

        private float duration;
        private float force;
        private float a;
        private float b;

        private KinematicCharacterMotor kinematicCharacterMotor;
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
            kinematicCharacterMotor = GetComponent<KinematicCharacterMotor>();
            characterMotor = GetComponent<CharacterMotor>();
            rigidbody = GetComponent<Rigidbody>();
            characterMotor.useGravity = false;
            moveTarget = new GameObject();
            if (storm)
            {
                moveTarget.transform.parent = storm.transform;
                moveTarget.transform.localPosition = Vector3.zero;
            }
        }

        public void SetStormTransform(GameObject storm)
        {
            this.storm = storm;
            moveTarget.transform.parent = storm.transform;
            moveTarget.transform.localPosition = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (!characterBody.hasEffectiveAuthority)
            {
                return;
            }

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

            if (!kinematicCharacterMotor || !characterBody || !characterMotor)
            {
                return;
            }

            previousPosition = characterMotor.previousPosition;
            timer += Time.fixedDeltaTime;
            var angle = Mathf.PI * timer;
            var r = a * Mathf.Pow((float)Math.E, b * angle);
            moveTarget.transform.localPosition = new Vector3(r * Mathf.Cos(angle), 2f + yHeight * (timer / baseDuration), r * Mathf.Sin(angle));
            kinematicCharacterMotor.SetPosition(moveTarget.transform.position, false);
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
            }
        }
    }
}
