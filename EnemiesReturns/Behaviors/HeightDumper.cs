using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class HeightDumper : MonoBehaviour
    {

        private void FixedUpdate()
        {
            if(Physics.Raycast(transform.position, Vector3.down, out var hitInfo, 1000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
            {
                Log.Info("distance to ground is :" + hitInfo.distance);
            }
        }


    }
}
