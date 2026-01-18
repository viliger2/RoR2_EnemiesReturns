using RoR2;
using UnityEngine;

namespace EnemiesReturns.Behaviors.ContactLight
{
    public class ContactLightGateSetter : MonoBehaviour
    {
        public string[] gates;

        private string spawnPointGate;

        public static ContactLightGateSetter instance { get; private set; }

        public void SetSpawnPointGate(string spawnPointGate)
        {
            this.spawnPointGate = spawnPointGate;
        }

        private void OnEnable()
        {
            RoR2.SceneDirector.onPrePopulateSceneServer += SceneDirector_onPrePopulateSceneServer;
            RoR2.SceneDirector.onPostPopulateSceneServer += SceneDirector_onPostPopulateSceneServer;
            instance = SingletonHelper.Assign(instance, this);
        }

        private void OnDisable()
        {
            RoR2.SceneDirector.onPrePopulateSceneServer -= SceneDirector_onPrePopulateSceneServer;
            RoR2.SceneDirector.onPostPopulateSceneServer -= SceneDirector_onPostPopulateSceneServer;
            instance = SingletonHelper.Unassign(instance, this);
        }

        private void SceneDirector_onPostPopulateSceneServer(RoR2.SceneDirector obj)
        {
            foreach (var gate in gates)
            {
                if (string.Equals(gate, spawnPointGate))
                {
                    continue;
                }
                SceneInfo.instance.SetGateState(gate, false);
            }
        }

        private void SceneDirector_onPrePopulateSceneServer(RoR2.SceneDirector obj)
        {
            foreach (var gate in gates)
            {
                SceneInfo.instance.SetGateState(gate, true);
            }
        }
    }
}
