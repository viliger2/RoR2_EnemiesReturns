using EnemiesReturns.Components;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.VoidlingWeapon
{
    [RegisterEntityState]
    public class Spawn : BaseNetworkedBodyAttachmentState
    {
        public static GameObject voidlingWeaponVisualsPrefab;

        public static float duration = 0.2083f;

        private GameObject weaponInstance;

        private CharacterBody body;

        private Transform target;

        public override void OnEnter()
        {
            base.OnEnter();

            body = bodyAttachment.attachedBody;

            if (equipmentSlot)
            {
                target = equipmentSlot.targetIndicator.targetTransform;
            }

            if (isAuthority)
            {
                weaponInstance = UnityEngine.Object.Instantiate(voidlingWeaponVisualsPrefab, bodyGameObject.transform);
                NetworkServer.SpawnWithClientAuthority(weaponInstance, body.networkIdentity.clientAuthorityOwner);
                var aimRay = body.inputBank.GetAimRay();
                var angleFromForward = Vector3.SignedAngle(Vector3.forward, new Vector3(aimRay.direction.x, 0, aimRay.direction.z), Vector3.up); // we find how far are we from forward ignoring y axis, so it doesn't affect the angle from forward
                var newRight = Quaternion.AngleAxis(angleFromForward, Vector3.up) * Vector3.right; // using the angle we find our new right to our aim direction
                weaponInstance.transform.position = body.corePosition + newRight * body.bestFitActualRadius;

                if (target)
                {
                    weaponInstance.transform.LookAt(target);
                }
                else
                {
                    weaponInstance.transform.forward = body.inputBank.aimDirection;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > duration && isAuthority)
            {
                var nextState = new Fire();
                nextState.weaponInstance = weaponInstance;
                nextState.target = target;
                outer.SetNextState(nextState);
            }
        }

        public override void Update()
        {
            base.Update();
            var aimRay = body.inputBank.GetAimRay();
            var angleFromForward = Vector3.SignedAngle(Vector3.forward, new Vector3(aimRay.direction.x, 0, aimRay.direction.z), Vector3.up); // we find how far are we from forward ignoring y axis, so it doesn't affect the angle from forward
            var newRight = Quaternion.AngleAxis(angleFromForward, Vector3.up) * Vector3.right; // using the angle we find our new right to our aim direction
            weaponInstance.transform.position = body.corePosition + newRight * body.bestFitActualRadius;
            if (target)
            {
                weaponInstance.transform.LookAt(target);
            }
            else
            {
                weaponInstance.transform.forward = body.inputBank.aimDirection;
            }
        }

    }
}
