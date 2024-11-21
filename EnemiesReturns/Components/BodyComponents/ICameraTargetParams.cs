using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface ICameraTargetParams
    {
        protected bool NeedToAddCameraTargetParams();

        protected CharacterCameraParams GetCharacterCameraParams();

        protected CameraTargetParams AddCameraTargetParams(GameObject bodyPrefab, CharacterCameraParams cameraParams)
        {
            CameraTargetParams cameraTargetParams = null;
            if (NeedToAddCameraTargetParams())
            {
                cameraTargetParams = bodyPrefab.GetOrAddComponent<CameraTargetParams>();
                cameraTargetParams.cameraParams = cameraParams;
            }

            return cameraTargetParams;
        }
    }
}
