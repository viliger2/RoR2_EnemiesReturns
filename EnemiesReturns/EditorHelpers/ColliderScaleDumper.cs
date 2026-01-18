using UnityEngine;

namespace EnemiesReturns.EditorHelpers
{
    public class ColliderScaleDumper : MonoBehaviour
    {
        private void FixedUpdate()
        {
            Log.Info($"collider scale: {gameObject.transform.localScale}, lossy scale: {gameObject.transform.lossyScale}");
        }
    }
}
