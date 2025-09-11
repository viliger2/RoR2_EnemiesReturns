using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public int projectileCount = 4;

    public float angle = 120f;

    public Vector3 aimDirection;

    private void OnDrawGizmos()
    {
        var angle = this.angle / (projectileCount - 1);

        var normalizerAimDirection = aimDirection.normalized;
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, normalizerAimDirection);

        Gizmos.color = Color.white;


        var startingDirection = Quaternion.AngleAxis(-this.angle * 0.5f, normalizerAimDirection) * Vector3.up;
        Gizmos.DrawRay(transform.position, startingDirection);
        var rotation = Quaternion.AngleAxis(angle, new Vector3(normalizerAimDirection.x, 0f, normalizerAimDirection.z));
        for (int i = 0; i < projectileCount; i++)
        {
            Gizmos.DrawCube(transform.position + startingDirection * 5, Vector3.one);
            startingDirection = rotation * startingDirection;
        }
    }
}
