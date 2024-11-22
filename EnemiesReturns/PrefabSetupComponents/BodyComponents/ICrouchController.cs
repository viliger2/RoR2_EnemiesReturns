using RoR2.Mecanim;
using UnityEngine;

namespace EnemiesReturns.PrefabSetupComponents.BodyComponents
{
    public interface ICrouchMecanim
    {
        protected class CrouchMecanimParams
        {
            public float duckHeight;
            public float smoothdamp = 0.3f;
            public float initialverticalOffset = 0f;
        }

        protected CrouchMecanimParams GetCrouchMecanimParams();

        protected bool NeedToAddCrouchMecanim();

        protected CrouchMecanim AddCrouchMecanim(GameObject crouchGameObject, Animator animator, CrouchMecanimParams crouchMecanimParams)
        {
            CrouchMecanim crouchController = null;
            if (NeedToAddCrouchMecanim())
            {
                crouchController = crouchGameObject.GetOrAddComponent<CrouchMecanim>();
                crouchController.duckHeight = crouchMecanimParams.duckHeight;
                crouchController.smoothdamp = crouchMecanimParams.smoothdamp;
                crouchController.initialVerticalOffset = crouchMecanimParams.initialverticalOffset;
                crouchController.animator = animator;
            }
            return crouchController;
        }
    }
}
