using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Components.GeneralComponents
{
    public interface INetworkIdentity
    {
        protected class NetworkIdentityParams
        {
            public bool serverOnly = false;
            public bool localPlayerAuthority = true;
        }

        public bool NeedToAddNetworkIdentity();

        protected NetworkIdentityParams GetNetworkIdentityParams();

        protected NetworkIdentity AddNetworkIdentity(GameObject bodyPrefab, NetworkIdentityParams neworkParams)
        {
            NetworkIdentity networkIdentity = null;
            if (NeedToAddNetworkIdentity())
            {
                networkIdentity = bodyPrefab.GetOrAddComponent<NetworkIdentity>();
                networkIdentity.serverOnly = neworkParams.serverOnly;
                networkIdentity.localPlayerAuthority = neworkParams.localPlayerAuthority;
            }

            return networkIdentity;
        }


    }
}
