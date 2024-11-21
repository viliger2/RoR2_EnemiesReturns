using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface IModelLocator
    {
        protected class ModelLocatorParams
        {
            public bool autoUpdateModelTransform = true;
            public bool dontDetachFromParent = false;

            public bool noCorpse = false;
            public bool dontReleaseModelOnDeath = false;
            public bool preserveModel = false;
            public bool forceCulled = false;

            public bool normalizeToFloor = false;
            public float normalSmoothdampTime = 0.1f;
            public float normalMaxAngleDelta = 90f;
        }

        protected bool NeedToAddModelLocator();

        protected ModelLocatorParams GetModelLocatorParams();

        protected ModelLocator AddModelLocator(GameObject bodyPrefab, Transform modelBase, Transform modelTransform, ModelLocatorParams modelLocatorParams)
        {
            ModelLocator modelLocator = null;
            if (NeedToAddModelLocator())
            {
                modelLocator = bodyPrefab.GetOrAddComponent<ModelLocator>();

                modelLocator.modelTransform = modelTransform;
                modelLocator.modelBaseTransform = modelBase;

                modelLocator.autoUpdateModelTransform = modelLocatorParams.autoUpdateModelTransform;
                modelLocator.dontDetatchFromParent = modelLocatorParams.dontDetachFromParent;

                modelLocator.noCorpse = modelLocatorParams.noCorpse;
                modelLocator.dontReleaseModelOnDeath = modelLocatorParams.dontReleaseModelOnDeath;
                modelLocator.preserveModel = modelLocatorParams.preserveModel;
                modelLocator.forceCulled = modelLocatorParams.forceCulled;

                modelLocator.normalizeToFloor = modelLocatorParams.normalizeToFloor;
                modelLocator.normalSmoothdampTime = modelLocatorParams.normalSmoothdampTime;
                modelLocator.normalMaxAngleDelta = modelLocatorParams.normalMaxAngleDelta;
            }
            return modelLocator;
        }
    }
}
