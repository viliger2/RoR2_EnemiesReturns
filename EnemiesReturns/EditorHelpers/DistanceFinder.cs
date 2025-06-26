using RoR2;
using UnityEngine;

namespace EnemiesReturns.EditorHelpers
{
    public class DistanceFinder : MonoBehaviour
    {
        public Transform distanceTo;

        private void OnEnable()
        {
            ChildLocator component = SceneInfo.instance.GetComponent<ChildLocator>();
            if ((bool)component)
            {
                Transform transform = component.FindChild("CenterOfArena");
                if ((bool)transform)
                {
                    distanceTo = transform;
                }
            }
        }

        private void FixedUpdate()
        {
            if (distanceTo)
            {
                Log.Info($"distance to: {Vector3.Distance(distanceTo.position, transform.position)}");
            }
        }


    }
}
