// RoR2.DisableCollisionsBetweenColliders
using UnityEngine;

namespace RoR2{
public class DisableCollisionsBetweenColliders : MonoBehaviour
{
	public Collider[] collidersA;

	public Collider[] collidersB;

	public void Awake()
	{
		Collider[] array = collidersA;
		foreach (Collider collider in array)
		{
			Collider[] array2 = collidersB;
			foreach (Collider collider2 in array2)
			{
				Physics.IgnoreCollision(collider, collider2);
			}
		}
	}
}
}
