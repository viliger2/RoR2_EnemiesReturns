using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.SwordHilt
{
    [RegisterEntityState]
    public class SpawnPortal : BaseState
    {
        public static GameObject portalContactLight;

        public override void OnEnter()
        {
            base.OnEnter();
            if(portalContactLight && NetworkServer.active)
            {
                var newObject = UnityEngine.Object.Instantiate(portalContactLight, transform.position, transform.rotation);
                NetworkServer.Spawn(newObject);
            }

        }

    }
}
