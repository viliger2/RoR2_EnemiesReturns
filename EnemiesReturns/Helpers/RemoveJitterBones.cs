using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Helpers
{
    public class RemoveJitterBones : MonoBehaviour
    {
        private int childrenCountPrev;

        private void Awake()
        {
            childrenCountPrev = base.transform.childCount;
            RemoveBones();
        }

        private void FixedUpdate()
        {
            if (childrenCountPrev != base.transform.childCount)
            {
                RemoveBones();
                childrenCountPrev = base.transform.childCount;
            }
        }

        private void RemoveBones()
        {
            var jitterBones = base.transform.GetComponentsInChildren<JitterBones>();
            for(int i = jitterBones.Length; i != 0; i--)
            {
                UnityEngine.GameObject.Destroy(jitterBones[i - 1]);
            }
        }
    }
}
