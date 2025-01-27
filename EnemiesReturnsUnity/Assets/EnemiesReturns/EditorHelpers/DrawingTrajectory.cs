using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingTrajectory : MonoBehaviour
{
    public Transform targetElevationTransform;

	public Vector3 jumpVelocity;

	public float time;

    private void OnDrawGizmos(){
        int num = 20;
		float num2 = time / (float)num;
		Vector3 vector = base.transform.position;
		Vector3 vector2 = jumpVelocity;
		Gizmos.color = Color.yellow;
		for (int i = 0; i <= num; i++)
		{
			Vector3 vector3 = vector + vector2 * num2;
			vector2 += Physics.gravity * num2;
			Gizmos.DrawLine(vector3, vector);
			vector = vector3;
		}
    }

	private void OnValidate()
	{
		var t = targetElevationTransform;
		var myT = base.transform;
		float yInitSpeed = CalculateInitialYSpeed(time, t.position.y - myT.position.y, -30f);
		float xOffset = t.position.x - myT.position.x;
		float zOffset = t.position.z - myT.position.z;
		jumpVelocity = new Vector3
		{
			x = xOffset / time,
			y = yInitSpeed,
			z = zOffset / time
		};
	}

	public static float CalculateInitialYSpeed(float timeToTarget, float destinationYOffset, float gravity)
	{
		return (destinationYOffset + 0.5f * (0f - gravity) * timeToTarget * timeToTarget) / timeToTarget;
	}
}
