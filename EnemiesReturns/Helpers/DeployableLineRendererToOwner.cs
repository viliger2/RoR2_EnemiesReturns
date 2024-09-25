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

        public string ownerTargetName;

        private Deployable deployable;

        private LineRenderer lineRenderer;

        private Transform originPoint;

        private Transform targetPoint;

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
                }
            }

            var characterModel = GetComponent<CharacterModel>();
            if (characterModel && characterModel.body) 
            {
                deployable = characterModel.body.GetComponent<Deployable>();
            }
            if(originPoint)
            {
                lineRenderer = originPoint.gameObject.GetComponent<LineRenderer>();
            }
        }

        private void Update()
        {
            if (!targetPoint)
            {
                CharacterBody ownerBody = null;
                if(deployable && deployable.ownerMaster)
                {
                    ownerBody = deployable.ownerMaster.GetBody();
                }

                ChildLocator ownerChildLocator = null;
                if(ownerBody)
                {
                    ownerChildLocator = ownerBody.modelLocator?.modelTransform?.gameObject.GetComponent<ChildLocator>() ?? null;
                }

                if (ownerChildLocator)
                {
                    targetPoint = ownerChildLocator.FindChild("Chest");
                }
                else if (ownerBody)
                {
                    targetPoint = ownerBody.transform;
                }
            }

            if (deployable && deployable.ownerMaster && lineRenderer)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, originPoint.position);
                lineRenderer.SetPosition(1, targetPoint?.position ?? originPoint.position);
            }
        }

    }
}
