using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors
{
    public class CollisionChecker : MonoBehaviour
    {

        private void OnTriggerEnter(Collider collider)
        {
            //Log.Info("Enter: " + collider);
        }

        private void OnTriggetExit(Collider collider)
        {
            //Log.Info("Exit: " + collider);
        }

        private void OnTriggerStay(Collider collider)
        {
            //Log.Info("Stay: " + collider);
        }
    }
}
