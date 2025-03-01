﻿using RoR2;
using UnityEngine;

namespace EnemiesReturns.Behaviors.Garbage
{
    public class DunnoRaycasterOrSomething : MonoBehaviour
    {
        private Transform ledgeChecker;

        private void Awake()
        {
            var childthing = GetComponent<ChildLocator>();
            ledgeChecker = childthing.FindChild("LedgeHandling");

            var lineRenderer = gameObject.AddComponent<LineRenderer>();

            if (ledgeChecker)
            {
                GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                primitive.transform.parent = ledgeChecker;
                primitive.transform.localPosition = Vector3.zero;
            }
        }

        private void FixedUpdate()
        {
            if (ledgeChecker)
            {
                var ray = new Ray(ledgeChecker.position, Vector3.down);
                if (Physics.Raycast(ray, out var hitinfo, 1000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                {
                    Log.Info("Distance: " + hitinfo.distance);
                }
            }
        }
    }
}
