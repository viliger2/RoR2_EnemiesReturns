using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.PlayerLoop;
using RoR2;

namespace EnemiesReturns.Behaviors
{
    public class DunnoRaycasterOrSomething : MonoBehaviour
    {
        private void Awake()
        {

            GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            primitive.transform.parent = gameObject.transform;
            primitive.transform.localPosition = Vector3.zero;
        }

        private void FixedUpdate()
        {

            if(Physics.Raycast(gameObject.transform.position, Vector3.down, out var hitinfo))
            {
                Debug.Log(hitinfo.distance);
                Debug.DrawLine(gameObject.transform.position, hitinfo.point);
                
                //Log.Info("Distance: " + hitinfo.distance);
            }

        }
    }
}
