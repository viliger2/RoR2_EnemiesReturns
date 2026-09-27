using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class MusicDisabler : MonoBehaviour
    {
        public float disableDistance;

        private GameObject target;

        private CameraRigController targetCamera;

        private void OnEnable()
        {
            targetCamera = ((CameraRigController.readOnlyInstancesList.Count > 0) ? CameraRigController.readOnlyInstancesList[0] : null);
            target = (targetCamera ? targetCamera.target : null);
        }

        private void LateUpdate()
        {
            if (!target)
            {
                return;
            }

            var body = target.GetComponent<CharacterBody>();
            if (!body)
            {
                return;
            }

            Log.Info(Vector3.Distance(body.transform.position, this.transform.position));
        }

        private void OnDisable()
        {
            RestoreMusic();
        }

        private void RestoreMusic()
        {
            if (RoR2.MusicController.Instance)
            {
                //AkSoundEngine.PostEvent("Unpause_Music", RoR2.MusicController.Instance.gameObject);
            }
        }

        private void PauseMusic()
        {
            if (RoR2.MusicController.Instance)
            {
                //AkSoundEngine.PostEvent("Pause_Music", RoR2.MusicController.Instance.gameObject);
            }
        }
    }
}
