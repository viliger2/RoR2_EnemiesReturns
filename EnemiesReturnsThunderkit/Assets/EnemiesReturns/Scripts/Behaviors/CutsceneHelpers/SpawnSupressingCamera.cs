using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors.CutsceneHelpers
{
    public class SpawnSupressingCamera : MonoBehaviour
    {
        private GameObject camera;

        private void Awake()
        {
            // camera = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Core/Menu Main Camera.prefab").WaitForCompletion(), transform);
            // camera.transform.localRotation = Quaternion.identity;
            // var cameraRigController = camera.GetComponent<CameraRigController>();
            // cameraRigController.suppressPlayerCameras = true;
        }

        public void OnEnable()
        {
            //On.RoR2.CameraRigController.LateUpdate += CameraRigController_LateUpdate;
        }

        public void OnDisable()
        {
            //On.RoR2.CameraRigController.LateUpdate -= CameraRigController_LateUpdate;
        }

        // private void CameraRigController_LateUpdate(On.RoR2.CameraRigController.orig_LateUpdate orig, CameraRigController self)
        // {
        //     orig(self);
        //     CameraState state = new CameraState();
        //     state.position = transform.position;
        //     state.rotation = transform.rotation;
        //     state.fov = self.currentCameraState.fov;
        //     self.SetCameraState(state);
        // }
    }
}
