using UnityEngine;

namespace EnemiesReturns.Behaviors.ContactLight
{
    public class SceneSpecificHooks : MonoBehaviour
    {
        private void OnEnable()
        {
            //On.RoR2.UI.HUDBossHealthBarController.LateUpdate += HUDBossHealthBarController_LateUpdate;
        }

        private void OnDisable()
        {
            //On.RoR2.UI.HUDBossHealthBarController.LateUpdate -= HUDBossHealthBarController_LateUpdate;
        }
    }
}
