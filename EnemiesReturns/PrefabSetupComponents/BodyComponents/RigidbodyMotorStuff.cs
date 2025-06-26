using UnityEngine;
using UnityEngine.Serialization;

namespace EnemiesReturns.Components.BodyComponents
{
    public class VectorPIDParams
    {
        [Tooltip("Just a field for user naming. Doesn't do anything.")]
        [FormerlySerializedAs("name")]
        public string customName;

        [Tooltip("PID Constants.")]
        public Vector3 PID = new Vector3(1f, 0f, 0f);

        [Tooltip("This is an euler angle, so we need to wrap correctly")]
        public bool isAngle;

        public float gain = 1f;
    }

    public class QuaternionPIDParams
    {
        [FormerlySerializedAs("name")]
        [Tooltip("Just a field for user naming. Doesn't do anything.")]
        public string customName;

        [Tooltip("PID Constants.")]
        public Vector3 PID = new Vector3(1f, 0f, 0f);

        [Tooltip("The quaternion we are currently at.")]
        public Quaternion inputQuat = Quaternion.identity;

        [Tooltip("The quaternion we want to be at.")]
        public Quaternion targetQuat = Quaternion.identity;

        public float gain = 1f;
    }
}
