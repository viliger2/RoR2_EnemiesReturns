using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.EditorHelpers
{
    public class EnemiesReturnsEventFucntions : MonoBehaviour
    {
        public void SetNetworkIdentityPingable(bool isPingable)
        {
            if(this.gameObject.TryGetComponent<NetworkIdentity>(out var identity))
            {
                identity.isPingable = isPingable;
            }
        }
    }
}
