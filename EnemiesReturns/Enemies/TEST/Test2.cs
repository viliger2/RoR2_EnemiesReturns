using EnemiesReturns.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Enemies.TEST
{
    public class Test2 : IBody
    {
        bool INetworkIdentity.AddNetworkIdentity() => true;

        INetworkIdentity.NetworkIdentityParams INetworkIdentity.GetNetworkIdentityParams() => new INetworkIdentity.NetworkIdentityParams();
    }
}
