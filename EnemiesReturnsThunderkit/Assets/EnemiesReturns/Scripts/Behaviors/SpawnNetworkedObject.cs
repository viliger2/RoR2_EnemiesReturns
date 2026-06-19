using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors
{
    public class SpawnNetworkedObject : MonoBehaviour
    {
        public GameObject objectToSpawn;

        public void SpawnObject()
        {
            if (!objectToSpawn)
            {
                return;
            }

            if (!NetworkServer.active)
            {
                return;
            }

            if(!objectToSpawn.TryGetComponent<NetworkIdentity>(out _))
            {
                return;
            }

            var newObject = UnityEngine.Object.Instantiate(objectToSpawn, transform.position, transform.rotation);
            NetworkServer.Spawn(newObject);
        }
    }
}
