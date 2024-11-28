using KinematicCharacterController;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace EnemiesReturns.Enemies.LynxTribe.Shaman.Storm
{
    public class LynxStormController : NetworkBehaviour
    {
        //[SyncVar]
        //public NetworkInstanceId victimObjectId;

        //[SyncVar]
        //public NetworkInstanceId attackerObjectId;

        //public GameObject victimObject
        //{
        //    get
        //    {
        //        if (!_victimObject)
        //        {
        //            if (NetworkServer.active)
        //            {
        //                _victimObject = NetworkServer.FindLocalObject(victimObjectId);
        //            } else if (NetworkClient.active)
        //            {
        //                _victimObject = ClientScene.FindLocalObject(victimObjectId);
        //            }
        //        }
        //        return _victimObject;
        //    }
        //    set
        //    {
        //        victimObjectId = value.GetComponent<NetworkIdentity>().netId;
        //    }
        //}

        //public GameObject attackerObject
        //{
        //    get
        //    {
        //        if (!_attackerObject)
        //        {
        //            if (NetworkServer.active)
        //            {
        //                _attackerObject = NetworkServer.FindLocalObject(attackerObjectId);
        //            }
        //            else if (NetworkClient.active)
        //            {
        //                _attackerObject = ClientScene.FindLocalObject(attackerObjectId);
        //            }
        //        }
        //        return _attackerObject;
        //    }
        //    set
        //    {
        //        attackerObjectId = value.GetComponent<NetworkIdentity>().netId;
        //    }
        //}

        //private GameObject _victimObject;

        //private GameObject _attackerObject;

        //public static float duration = 4f;

        //public static float a = 0.7f;

        //public static float b = 0.2f;

        //public static float force = 5000f;

        //public float yHeight = 8f;

        //private float timer;

        //private KinematicCharacterMotor kinematicCharacterMotor;
        //private CharacterMotor characterMotor;
        //private CharacterBody characterBody;
        //private Rigidbody rigidbody;
        //private GameObject moveTarget;
        //private Vector3 previousPosition;

        //private bool done;

        //private void Awake()
        //{
        //    //characterBody = GetComponent<CharacterBody>();
        //    //kinematicCharacterMotor = GetComponent<KinematicCharacterMotor>();
        //    //characterMotor = GetComponent<CharacterMotor>();
        //    //rigidbody = GetComponent<Rigidbody>();
        //    //characterMotor.useGravity = false;

        //    moveTarget = new GameObject();
        //    moveTarget.transform.parent = attackerObject.transform;
        //    moveTarget.transform.localPosition = Vector3.zero;
        //    if (victimObject)
        //    {
        //        characterBody = victimObject.GetComponent<CharacterBody>();
        //        kinematicCharacterMotor = victimObject.GetComponent<KinematicCharacterMotor>();
        //        characterMotor = victimObject.GetComponent<CharacterMotor>();
        //        rigidbody = victimObject.GetComponent<Rigidbody>();
        //        characterMotor.useGravity = false;
        //        if (NetworkServer.active)
        //        {

        //        }
        //    }
        //}

        //public void SetOwner(GameObject owner)
        //{
        //    this.attackerObject = owner;
        //    moveTarget.transform.parent = attackerObject.transform;
        //    moveTarget.transform.localPosition = Vector3.zero;
        //}

        //public void SetVictim(GameObject victim)
        //{
        //    this.victimObject = victim;
        //    characterBody = victimObject.GetComponent<CharacterBody>();
        //    kinematicCharacterMotor = victimObject.GetComponent<KinematicCharacterMotor>();
        //    characterMotor = victimObject.GetComponent<CharacterMotor>();
        //    rigidbody = victimObject.GetComponent<Rigidbody>();
        //    characterMotor.useGravity = false;
        //    if (NetworkServer.active)
        //    {

        //    }
        //}

        //private void FixedUpdate()
        //{
        //    if (!Util.HasEffectiveAuthority(victimObject))
        //    {
        //        return;
        //    }

        //    if (done)
        //    {
        //        return;
        //    }

        //    if (timer > duration)
        //    {
        //        if (characterMotor)
        //        {
        //            characterMotor.useGravity = true;
        //            var normalized2 = (characterMotor.transform.position - previousPosition).normalized;
        //            var forceVector = new Vector3(normalized2.x, 0f, normalized2.z) * force;
        //            characterMotor.ApplyForce(forceVector, true, false);
        //            if (rigidbody)
        //            {
        //                rigidbody.AddForce(forceVector, ForceMode.Impulse);
        //            }
        //            CmdApplyDoT();
        //            done = true;
        //        }
        //        return;
        //    }

        //    if (!kinematicCharacterMotor || !characterBody || !characterMotor)
        //    {
        //        return;
        //    }

        //    previousPosition = characterMotor.previousPosition;
        //    timer += Time.fixedDeltaTime;
        //    var angle = Mathf.PI * timer;
        //    var r = a * Mathf.Pow((float)Math.E, b * angle);
        //    moveTarget.transform.localPosition = new Vector3(r * Mathf.Cos(angle), 2f + yHeight * (timer / duration), r * Mathf.Sin(angle));
        //    kinematicCharacterMotor.SetPosition(moveTarget.transform.position, false);
        //}

        //[Command]
        //private void CmdApplyDoT()
        //{
        //    if(attackerObject && victimObject)
        //    {
        //        DotController.InflictDot(victimObject, attackerObject, DotController.DotIndex.Bleed, 4f, 5);
        //    }
        //    NetworkServer.Destroy(this.gameObject);
        //}

        //private void OnDisable()
        //{
        //    if (characterMotor)
        //    {
        //        characterMotor.useGravity = true;
        //    }
        //}
    }
}
