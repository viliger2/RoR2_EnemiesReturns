using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Helpers
{
    public class LineRendererToOwner : MonoBehaviour
    {
        private Deployable deployable;

        private LineRenderer lineRenderer;

        private void OnEnable()
        {
            var characterModel = GetComponent<CharacterModel>();
            if (characterModel && characterModel.body) 
            {
                deployable = characterModel.body.GetComponent<Deployable>();
            }
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            if(deployable && deployable.ownerMaster && lineRenderer)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, base.gameObject.transform.position);
                lineRenderer.SetPosition(1, deployable.ownerMaster.GetBody().transform.position);
            }
        }

    }
}
