// RoR2.DisableCollisionsBetweenColliders
using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;
namespace RoR2 {
public class DisableCollisionsBetweenColliders : MonoBehaviour
{
	public Collider[] collidersA = Array.Empty<Collider>();

	public Collider[] collidersB = Array.Empty<Collider>();

	[ContextMenu("Automatically Populate From Hierarchy")]
	private void AutomaticallyPopulateFromHierarchy()
	{
		List<Collider> list = new List<Collider>();
		List<Collider> list2 = new List<Collider>();
		Rigidbody componentInParent = GetComponentInParent<Rigidbody>();
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>(includeInactive: true);
		foreach (Collider collider in componentsInChildren)
		{
			((collider.GetComponentInParent<Rigidbody>(includeInactive: true) == componentInParent) ? list : list2).Add(collider);
		}
		LayerMask rootColliderCollisionMask = default(LayerMask);
		foreach (Collider item in list)
		{
			rootColliderCollisionMask = (int)rootColliderCollisionMask | (int)((LayerIndex)item.gameObject.layer).collisionMask;
		}
		collidersA = list.ToArray();
		collidersB = list2.Where(delegate(Collider childCollider)
		{
			LayerMask collisionMask = ((LayerIndex)childCollider.gameObject.layer).collisionMask;
			return ((int)rootColliderCollisionMask & (int)collisionMask) != 0;
		}).ToArray();
	}

	public void Apply()
	{
		RefreshColliders();
	}

	public void RefreshColliders()
	{
		Collider[] array = collidersA;
		foreach (Collider collider in array)
		{
			Collider[] array2 = collidersB;
			foreach (Collider collider2 in array2)
			{
				if (collider2 != null && collider != null)
				{
					Physics.IgnoreCollision(collider, collider2);
				}
			}
		}
	}

	protected void Awake()
	{
		Apply();
	}
}
}
