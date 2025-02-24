using RoR2;
using UnityEngine;

namespace EnemiesReturns.Behaviors.JitterBonesStuff
{
    public class RemoveJitterBones : MonoBehaviour
    {
        private int childrenCountPrev;

        private void Awake()
        {
            childrenCountPrev = transform.childCount;
            RemoveBones();
        }

        private void FixedUpdate()
        {
            if (childrenCountPrev != transform.childCount)
            {
                RemoveBones();
                childrenCountPrev = transform.childCount;
            }
        }

        private void RemoveBones()
        {
            var jitterBones = transform.GetComponentsInChildren<JitterBones>();
            for (int i = jitterBones.Length; i != 0; i--)
            {
                GameObject.Destroy(jitterBones[i - 1]);
            }
        }
    }
}
