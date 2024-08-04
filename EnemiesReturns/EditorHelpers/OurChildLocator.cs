using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EnemiesReturns.EditorHelpers
{
	public class OurChildLocator : MonoBehaviour
	{
		[Serializable]
		public struct NameTransformPair
		{
			public string name;

			public Transform transform;
		}

		[SerializeField]
		public NameTransformPair[] transformPairs = Array.Empty<NameTransformPair>();
	}
}
