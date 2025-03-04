using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class MoveTowardsTargetAndDestroyItself : MonoBehaviour
    {
        public Transform target;

        public float duration;

        private float timer;

        private Vector3 initialPosition;

        private bool setToDestroy = false;

        private void OnEnable()
        {
            initialPosition = transform.position;
        }

        private void LateUpdate()
        {
            if (target)
            {
                timer += Time.deltaTime;
                this.transform.position = Vector3.Lerp(initialPosition, target.position, timer / duration);
                if (Vector3.Distance(this.transform.position, target.position) < 0.01f && !setToDestroy)
                {
                    Invoke("DestroyGameObject", 0.1f);
                    setToDestroy = true;
                }
            }
        }

        private void DestroyGameObject()
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }
}
