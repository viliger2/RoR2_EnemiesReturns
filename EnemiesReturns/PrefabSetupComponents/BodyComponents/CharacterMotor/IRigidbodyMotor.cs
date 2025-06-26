using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface IRigidbodyMotor
    {
        protected class RigidbodyMotorParams
        {
            public VectorPIDParams forcePID;
            public Vector3 centerOfMassOffset = Vector3.zero;
            public string animatorForward = "";
            public string animatorRight = "";
            public string animatorUp = "";
            public bool enableOverrideMoveVectorInLocalSpace = false;
            public bool canTakeImpactDamage = true;
            public Vector3 overrideMoveVectorInLocalSpace = Vector3.zero;
        }

        protected bool NeedToAddRigidbodyMotor();

        protected RigidbodyMotorParams GetRigidBodyMotorParams();

        protected RigidbodyMotor AddRigidbodyMotor(GameObject bodyPrefab, Rigidbody rigidBody, RigidbodyMotorParams motorParams)
        {
            RigidbodyMotor motor = null;
            if (NeedToAddRigidbodyMotor())
            {
                if (motorParams.forcePID == null)
                {
                    Log.Warning($"ForcePIDParams is null when creating RigidbodyMotor for body {bodyPrefab}!");
                    return null;
                }

                VectorPID forcePID = bodyPrefab.AddComponent<VectorPID>();
                forcePID.customName = motorParams.forcePID.customName;
                forcePID.PID = motorParams.forcePID.PID;
                forcePID.isAngle = motorParams.forcePID.isAngle;
                forcePID.gain = motorParams.forcePID.gain;

                motor = bodyPrefab.AddComponent<RigidbodyMotor>();
                motor.rigid = rigidBody;
                motor.forcePID = forcePID;
                motor.centerOfMassOffset = motorParams.centerOfMassOffset;
                motor.animatorForward = motorParams.animatorForward;
                motor.animatorRight = motorParams.animatorRight;
                motor.animatorUp = motorParams.animatorUp;
                motor.enableOverrideMoveVectorInLocalSpace = motorParams.enableOverrideMoveVectorInLocalSpace;
                motor.canTakeImpactDamage = motorParams.canTakeImpactDamage;
                motor.overrideMoveVectorInLocalSpace = motorParams.overrideMoveVectorInLocalSpace;
            }
            return motor;
        }
    }

}
