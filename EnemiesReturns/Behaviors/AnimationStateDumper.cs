using UnityEngine;

namespace EnemiesReturns.Helpers
{
    public class AnimationStateDumper : MonoBehaviour
    {
        private Animator animator;

        private float time;

        private void OnEnable()
        {
            animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            time += Time.deltaTime;
            if (animator)
            {
                for (int i = 0; i < animator.layerCount; i++)
                {
                    var clipInfo = animator.GetCurrentAnimatorClipInfo(i);
                    foreach (var clipInfo2 in clipInfo)
                    {
                        Log.Info($"{time} Layer {animator.GetLayerName(i)} is playing clip {clipInfo2.clip}");
                    }

                }
            }
        }

    }
}
