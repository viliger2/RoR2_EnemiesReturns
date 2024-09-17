// RoR2.StartEvent
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2 {
public class StartEvent : MonoBehaviour
{
	public bool runOnServerOnly;

	public UnityEvent action;

	private void Start()
	{

	}
}
}
