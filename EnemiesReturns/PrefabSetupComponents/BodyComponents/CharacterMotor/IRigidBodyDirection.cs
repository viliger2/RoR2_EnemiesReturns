using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface IRigidBodyDirection
    {
        protected class RigidbodyDirectionParams
        {
            public Vector3 aimDirection = Vector3.one;
            public bool freezeXRotation = false;
            public bool freezeYRotation = false;
            public bool freezeZRotation = false;

            public string animatorXCycle = "";
            public string animatorYCycle = "";
            public string animatorZCycle = "";
            public float animatorTorqueScale = 0f;

            public VectorPIDParams torquePID;
            public QuaternionPIDParams angularVelocityPID;
        }

        protected bool NeedToAddRigidBodyDirection();

        protected RigidbodyDirectionParams GetRigidBodyDirectionParams();

        protected RigidbodyDirection AddRigidbodyDirection(GameObject bodyPrefab, Rigidbody rigidBody, RigidbodyDirectionParams directionParams)
        {
            RigidbodyDirection direction = null;
            if (NeedToAddRigidBodyDirection())
            {
                if (directionParams.torquePID == null)
                {
                    Log.Warning($"TorquePIDParams is null when creating RigidbodyDirection for body {bodyPrefab}!");
                    return null;
                }
                VectorPID vectorPID = bodyPrefab.AddComponent<VectorPID>();
                vectorPID.customName = directionParams.torquePID.customName;
                vectorPID.PID = directionParams.torquePID.PID;
                vectorPID.isAngle = directionParams.torquePID.isAngle;
                vectorPID.gain = directionParams.torquePID.gain;

                if (directionParams.angularVelocityPID == null)
                {
                    Log.Warning($"QuaternionPIDParams is null when creating RigidbodyDirection for body {bodyPrefab}!");
                    return null;
                }
                QuaternionPID quaternionPID = bodyPrefab.AddComponent<QuaternionPID>();
                quaternionPID.customName = directionParams.angularVelocityPID.customName;
                quaternionPID.PID = directionParams.angularVelocityPID.PID;
                quaternionPID.inputQuat = directionParams.angularVelocityPID.inputQuat;
                quaternionPID.targetQuat = directionParams.angularVelocityPID.targetQuat;
                quaternionPID.gain = directionParams.angularVelocityPID.gain;

                direction = bodyPrefab.AddComponent<RigidbodyDirection>();
                direction.aimDirection = directionParams.aimDirection;
                direction.rigid = rigidBody;
                direction.angularVelocityPID = quaternionPID;
                direction.torquePID = vectorPID;
                direction.freezeXRotation = directionParams.freezeXRotation;
                direction.freezeYRotation = directionParams.freezeYRotation;
                direction.freezeZRotation = directionParams.freezeZRotation;
                direction.animatorXCycle = directionParams.animatorXCycle;
                direction.animatorYCycle = directionParams.animatorYCycle;
                direction.animatorZCycle = directionParams.animatorZCycle;
                direction.animatorTorqueScale = directionParams.animatorTorqueScale;
            }

            return direction;
        }
    }
}
