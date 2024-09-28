using UnityEngine;

namespace EnemiesReturns.EditorHelpers
{
    [CreateAssetMenu(menuName = "EnemiesReturns/AnimationCurveDef")]
    public class AnimationCurveDef : ScriptableObject
    {
        public AnimationCurve curve;
    }
}
