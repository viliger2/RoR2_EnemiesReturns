using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;

namespace EnemiesReturns.Components
{
    public interface INetworkIdentity
    {
        internal class NetworkIdentityParams
        {
            public bool serverOnly = false;
            public bool localPlayerAuthority = true;
        }

        public bool AddNetworkIdentity();

        internal NetworkIdentityParams GetNetworkIdentityParams();

        internal NetworkIdentity AddNetworkIdentity(GameObject bodyPrefab, NetworkIdentityParams neworkParams)
        {
            NetworkIdentity networkIdentity = null;
            if (AddNetworkIdentity())
            {
                networkIdentity = bodyPrefab.GetOrAddComponent<NetworkIdentity>();
                networkIdentity.serverOnly = neworkParams.serverOnly;
                networkIdentity.localPlayerAuthority = neworkParams.localPlayerAuthority;
            }

            return networkIdentity;
        }


    }
}
