using UnityEngine;
using UnityEngine.Events;
namespace RoR2.EntityLogic {
public class DelayedEvent : MonoBehaviour
{
	public enum TimeStepType
	{
		Time,
		UnscaledTime,
		FixedTime
	}

	public UnityEvent action;

	public TimeStepType timeStepType;

	public void CallDelayed(float timer)
	{

	}

	public void CallDelayedIfActiveAndEnabled(float timer)
	{

	}

	private void Call()
	{

	}
}
}
