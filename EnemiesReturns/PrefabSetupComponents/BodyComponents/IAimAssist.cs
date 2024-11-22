using RoR2;
using UnityEngine;

namespace EnemiesReturns.PrefabSetupComponents.BodyComponents
{
    public interface IAimAssist
    {
        protected class AimAssistTargetParams
        {
            public string pathToPoint0;
            public string pathToPoint1;
            public float assistScale = 3f;
        }

        protected bool NeedToAddAimAssistTarget();

        protected AimAssistTargetParams GetAimAssistTargetParams();

        protected AimAssistTarget AddAimAssistTarget(Transform modelTransform, GameObject bodyPrefab, AimAssistTargetParams aimAssistTargetParams, HealthComponent healthComponent, TeamComponent teamComponent)
        {
            if (!NeedToAddAimAssistTarget())
            {
                return null;
            }

            var aimAssistObject = GetAimAssistObject(modelTransform);
            if (!aimAssistObject)
            {
                return null;
            }

            var point0 = bodyPrefab.transform.Find(aimAssistTargetParams.pathToPoint0);
#if DEBUG || NOWEAVER
            if (!point0)
            {
                Log.Warning($"For body {bodyPrefab} couldn't find point0 for AimAssistTarget at path {aimAssistTargetParams.pathToPoint0}");
            }
#endif
            var point1 = bodyPrefab.transform.Find(aimAssistTargetParams.pathToPoint1);
#if DEBUG || NOWEAVER
            if (!point1)
            {
                Log.Warning($"For body {bodyPrefab} couldn't find point1 for AimAssistTarget at path {aimAssistTargetParams.pathToPoint1}");
            }
#endif
            AimAssistTarget aimAssist = aimAssistObject.GetOrAddComponent<AimAssistTarget>();
            aimAssist.point0 = point0;
            aimAssist.point1 = point1;
            aimAssist.healthComponent = healthComponent;
            aimAssist.teamComponent = teamComponent;
            aimAssist.assistScale = aimAssistTargetParams.assistScale;

            return aimAssist;
        }

        private GameObject GetAimAssistObject(Transform modelTransform)
        {
            var aimAssist = modelTransform.Find("AimAssist");
#if DEBUG || NOWEAVER
            if (!aimAssist && NeedToAddAimAssistTarget())
            {
                Log.Warning($"Transform {modelTransform} doesn't have AimAssist!");
                return null;
            }
#endif
            return aimAssist.gameObject;
        }
    }
}
