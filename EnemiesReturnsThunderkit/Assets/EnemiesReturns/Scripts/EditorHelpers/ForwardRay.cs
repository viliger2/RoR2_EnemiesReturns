using UnityEngine;

public class ForwardRay : MonoBehaviour
{
    void OnDrawGizmos()
    {
		Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 9999f);
    }
}
