// RoR2.OnEnableEvent
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	


public class OnEnableEvent : MonoBehaviour
{
	public UnityEvent action;

	private void OnEnable()
	{
		action?.Invoke();
	}
}
}