using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace EnemiesReturns.Behaviors
{
    // I hate JitterBones so much its an entire engine
    public class FixJitterBones : MonoBehaviour
    {
        public string[] bonesToFix;

        private int childrenCountPrev;

        private bool bonesFixed = false;

        private void Awake()
        {
            childrenCountPrev = base.transform.childCount;
            ApplyFix();
        }

        private void FixedUpdate()
        {
            if (!bonesFixed)
            {
                if (childrenCountPrev != base.transform.childCount)
                {
                    ApplyFix();
                    childrenCountPrev = base.transform.childCount;
                }
            }
        }

        private void ApplyFix()
        {
            var jitterBones = base.transform.GetComponentsInChildren<JitterBones>();
            if(jitterBones.Length > 0)
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
                this.enabled = false;
            }
        }
    }
}
