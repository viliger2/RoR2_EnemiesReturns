using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Helpers
{
    public class DeployableLineRendererToOwner : MonoBehaviour
    {
        public string childOriginName;

        private Deployable deployable;

        private LineRenderer lineRenderer;

        private Transform originPoint;

        private Transform targetPoint;

        private CharacterBody ownerBody;

        private void OnEnable()
        {
            originPoint = gameObject.transform;
            var childLocator = GetComponent<ChildLocator>();
            if (childLocator)
            {
                var child = childLocator.FindChild(childOriginName);
                if (child)
                {
                    originPoint = child;
                    targetPoint = child;
                }
            }

            var characterModel = GetComponent<CharacterModel>();
            if (characterModel && characterModel.body) 
            {
                deployable = characterModel.body.GetComponent<Deployable>();
            }
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            if(!ownerBody)
            {
                if (deployable && deployable.ownerMaster)
                {
                    ownerBody = deployable.ownerMaster.GetBody();
                }
            } else
            {
                targetPoint = ownerBody.transform;
            }
            if (deployable && deployable.ownerMaster && lineRenderer)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, originPoint.position);
                lineRenderer.SetPosition(1, targetPoint.position);
            }
        }

    }
}
