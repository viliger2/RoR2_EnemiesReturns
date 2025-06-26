using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Behaviors.CutsceneHelpers
{
    public class SpawnSupressingCamera : MonoBehaviour
    {
        public float fov = 60f;

        private GameObject camera;

        private CameraState cameraState;

        private void Awake()
        {
            camera = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Core/Menu Main Camera.prefab").WaitForCompletion(), transform);
            camera.transform.localRotation = Quaternion.identity;
            var cameraRigController = camera.GetComponent<CameraRigController>();
            cameraRigController.suppressPlayerCameras = true;
            cameraState = new CameraState();
        }

        public void OnEnable()
        {
            On.RoR2.CameraRigController.LateUpdate += CameraRigController_LateUpdate;
        }

        public void OnDisable()
        {
            On.RoR2.CameraRigController.LateUpdate -= CameraRigController_LateUpdate;
        }

        private void CameraRigController_LateUpdate(On.RoR2.CameraRigController.orig_LateUpdate orig, CameraRigController self)
        {
            orig(self);
            cameraState.position = transform.position;
            cameraState.rotation = transform.rotation;
            cameraState.fov = fov;
            self.SetCameraState(cameraState);
        }
    }
}
