using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawBoundingBox : MonoBehaviour
{
    // Start is called before the first frame update
    void OnDrawGizmos()
    {
        OnDrawGizmosSelected();
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var bounds = transform.GetComponent<Renderer>().bounds;
        Gizmos.DrawSphere(bounds.center, 0.1f);  //center sphere
        if (transform.GetComponent<Renderer>() != null)
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
