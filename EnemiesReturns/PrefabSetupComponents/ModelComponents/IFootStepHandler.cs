using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.ModelComponents
{
    public interface IFootStepHandler
    {
        protected class FootstepHandlerParams
        {
            public string baseFootstepString = "";
            public string baseFootliftString = "";
            public string sprintFootstepOverrideString = "";
            public string sprintFootliftOverrideString = "";
            public bool enableFootstepDust = false;
            public GameObject footstepDustPrefab;
        }

        protected bool NeedToAddFootstepHandler();

        protected FootstepHandlerParams GetFootstepHandlerParams();

        protected FootstepHandler AddFootstepHandler(GameObject model, FootstepHandlerParams footstepHandlerParams)
        {
            FootstepHandler footstepHandler = null;
            if (NeedToAddFootstepHandler())
            {
                footstepHandler = model.GetOrAddComponent<FootstepHandler>();

                footstepHandler.baseFootstepString = footstepHandlerParams.baseFootstepString;
                footstepHandler.baseFootliftString = footstepHandlerParams.baseFootliftString;
                footstepHandler.sprintFootstepOverrideString = footstepHandlerParams.sprintFootstepOverrideString;
                footstepHandler.sprintFootliftOverrideString = footstepHandlerParams.sprintFootliftOverrideString;
                footstepHandler.enableFootstepDust = footstepHandlerParams.enableFootstepDust;
                footstepHandler.footstepDustPrefab = footstepHandlerParams.footstepDustPrefab;
            }
            return footstepHandler;
        }
    }
}
