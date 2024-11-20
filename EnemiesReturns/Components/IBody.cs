using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Components
{
    public interface IBody : INetworkIdentity
    {
        public GameObject CreateBody(GameObject body)
        {
            AddNetworkIdentity(body, GetNetworkIdentityParams());

            return body;
        }


    }
}
