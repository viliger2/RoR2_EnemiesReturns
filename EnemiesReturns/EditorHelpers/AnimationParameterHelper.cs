using System.Collections.Generic;
using UnityEngine;

namespace EnemiesReturns.EditorHelpers
{
    public class AnimationParameterHelper : MonoBehaviour
    {
        public Animator animator;

        public string[] animationParameters;

        private Dictionary<string, int> animationParametersHashes = new Dictionary<string, int>();

        private void Awake()
        {
            foreach (string animationParameter in animationParameters)
            {
                animationParametersHashes.Add(animationParameter, Animator.StringToHash(animationParameter));
            }
        }

        private void Update()
        {
            // check the thing
            //Log.Info("walkSpeed: " + animator.GetFloat(AnimationParameters.walkSpeed));

            //set the thing
            foreach (var thing in animationParametersHashes.Keys)
            {
                if (thing.Equals("walkSpeedDebug"))
                {
                    animator.SetFloat(animationParametersHashes[thing], EnemiesReturnsConfiguration.DebugWalkSpeedValue.Value);
                }
            }

        }


    }
}
