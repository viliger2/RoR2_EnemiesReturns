using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Behaviors.CutsceneHelpers
{
    public class SpawnSupressingCamera : MonoBehaviour
    {
        public float fov = 60f;

        public Canvas barsCanvas;

        public Canvas textCanvas;

        private GameObject camera;

        private CameraState cameraState;

        private void Awake()
        {
            camera = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Core.Menu_Main_Camera_prefab).WaitForCompletion(), transform);
            camera.transform.localRotation = Quaternion.identity;
            var cameraRigController = camera.GetComponent<CameraRigController>();
            cameraRigController.suppressPlayerCameras = true;

            barsCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            barsCanvas.worldCamera = cameraRigController.uiCam;

            textCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            textCanvas.worldCamera = cameraRigController.uiCam; // it should be scene cam but it breaks the text for some reason

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
