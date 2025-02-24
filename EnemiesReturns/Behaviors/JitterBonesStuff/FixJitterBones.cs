using RoR2;
using System;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.Behaviors.JitterBonesStuff
{
    // I hate JitterBones so much its an entire engine
    public class FixJitterBones : MonoBehaviour
    {
        public string[] bonesToFix;

        private int childrenCountPrev;

        private bool bonesFixed = false;

        private void Awake()
        {
            childrenCountPrev = transform.childCount;
            ApplyFix();
        }

        private void FixedUpdate()
        {
            if (!bonesFixed)
            {
                if (childrenCountPrev != transform.childCount)
                {
                    ApplyFix();
                    childrenCountPrev = transform.childCount;
                }
            }
        }

        private void ApplyFix()
        {
            var jitterBones = transform.GetComponentsInChildren<JitterBones>();
            if (jitterBones.Length > 0)
            {
                bonesFixed = true;
                for (int i = jitterBones.Length; i != 0; i--)
                {
                    for (int k = 0; k < jitterBones[i - 1].bones.Length; k++)
                    {
                        var bone = jitterBones[i - 1].bones[k];
                        if (bonesToFix.Contains(bone.transform.name))
                        {
                            jitterBones[i - 1].bones[k] = new JitterBones.BoneInfo
                            {
                                transform = bone.transform,
                                isHead = false,
                                isRoot = true
                            };
                        }
                    }
                }
                enabled = false;
            }
        }
    }
}
