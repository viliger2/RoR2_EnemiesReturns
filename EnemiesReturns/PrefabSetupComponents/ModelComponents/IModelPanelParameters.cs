using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.ModelComponents
{
    public interface IModelPanelParameters
    {
        protected class ModelPanelParams
        {
            public Quaternion modelRotation = Quaternion.identity;
            public float minDistance = 1.5f;
            public float maxDistance = 6f;
        }

        protected bool NeedToAddModelPanelParameters();

        protected ModelPanelParams GetModelPanelParams();

        protected ModelPanelParameters AddModelPanelParameters(GameObject model, Transform focusPoint, Transform cameraPoint, ModelPanelParams modelPanelParams)
        {
            ModelPanelParameters modelPanel = null;
            if (NeedToAddModelPanelParameters())
            {
                modelPanel = model.GetOrAddComponent<ModelPanelParameters>();

                modelPanel.focusPointTransform = focusPoint;
                modelPanel.cameraPositionTransform = cameraPoint;
                modelPanel.modelRotation = modelPanelParams.modelRotation;
                modelPanel.minDistance = modelPanelParams.minDistance;
                modelPanel.maxDistance = modelPanelParams.maxDistance;
            }

            return modelPanel;
        }
    }
}
