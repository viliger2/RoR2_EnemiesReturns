using RoR2.Mecanim;
using UnityEngine;

namespace EnemiesReturns.PrefabSetupComponents.ModelComponents
{
    public interface IRandomBlinkController
    {
        protected class RandomBlinkParams
        {
            public RandomBlinkParams(string[] strings)
            {
                this.blinkTriggers = strings;
            }
            public float blinkChancePerUpdate = 10f;
            public string[] blinkTriggers;
        }

        protected RandomBlinkParams GetRandomBlinkParams();

        protected bool NeedToAddRandomBlinkController();

        protected RandomBlinkController AddRandomBlinkController(GameObject modelPrefab, Animator animator, RandomBlinkParams blinkParams)
        {
            RandomBlinkController blinks = null;
            if (NeedToAddRandomBlinkController())
            {
                blinks = modelPrefab.GetOrAddComponent<RandomBlinkController>();
                blinks.animator = animator;
                blinks.blinkChancePerUpdate = blinkParams.blinkChancePerUpdate;
                blinks.blinkTriggers = blinkParams.blinkTriggers;
            }
            return blinks;
        }
    }
}
