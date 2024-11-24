using KinematicCharacterController;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents.CharacterMotor
{
    public interface IKinematicCharacterMotor
    {
        protected class KinemacitCharacterMotorParams
        {
            public float GroundDetectionExtraDistance = 0f;
            public float MaxStableSlopeAngle = 55f;
            public LayerMask StableGroundLayers = Physics.AllLayers;
            public bool DiscreteCollisionEvents = false;

            public StepHandlingMethod StepHandling = StepHandlingMethod.Standard;
            public float MaxStepHeight = 0.2f;
            public bool AllowSteppingWithoutStableGrounding = false;
            public float MinRequiredStepDepth = 0.1f;

            public bool LedgeAndDenivelationHandling = true;
            public float MaxStableDistanceFromLedge = 0.5f;
            public float MaxVelocityForLedgeSnap = 0f;
            public float MaxStableDenivelationAngle = 55f;

            public bool InteractiveRigidbodyHandling = true;
            public RigidbodyInteractionType RigidbodyInteractionType = RigidbodyInteractionType.None;
            public float SimulatedCharacterMass = 1f;
            public bool PreserveAttachedRigidbodyMomentum = true;

            public bool HasPlanarConstraint = false;
            public Vector3 PlanarConstraintAxis = Vector3.forward;

            public bool CheckMovementInitialOverlaps = true;
            public bool KillVelocityWhenExceedMaxMovementIterations = true;
            public bool KillRemainingMovementWhenExceedMaxMovementIterations = true;
            public float timeUntilUpdate = 0f;
            public bool playerCharacter = false;

        }

        protected KinemacitCharacterMotorParams GetKinematicCharacterMotorParams();

        protected bool NeedToAddKinematicCharacterMotor();

        protected KinematicCharacterMotor AddKinematicCharacterMotor(GameObject bodyPrefab, CapsuleCollider capsule, Rigidbody rigidBody, ICharacterController characterController, KinemacitCharacterMotorParams kcmParams)
        {
            KinematicCharacterMotor kinematicCharacterMotor = null;

            if (NeedToAddKinematicCharacterMotor())
            {
                kinematicCharacterMotor = bodyPrefab.GetOrAddComponent<KinematicCharacterMotor>();

                if (capsule.radius < 0.5f)
                {
                    Log.Warning($"CapsuleCollider {capsule} has radius less than a beetle (0.5f), this WILL result in pathfinding issues for AIs.");
                };
                if (capsule.height < 1.82f)
                {
                    Log.Warning($"CapsuleCollider {capsule} has height less than a beetle (1.82f), this WILL result in pathfinding issues for AIs.");
                };
                if (capsule.center != Vector3.zero)
                {
                    Log.Warning($"CapsuleCollider {capsule} has non-zero center, this WILL result in pathfinding issues for AIs.");
                };

                kinematicCharacterMotor.CharacterController = characterController;
                kinematicCharacterMotor.Capsule = capsule;
                kinematicCharacterMotor._attachedRigidbody = rigidBody;
                kinematicCharacterMotor.CapsuleRadius = capsule.radius;
                kinematicCharacterMotor.CapsuleHeight = capsule.height;
                kinematicCharacterMotor.CapsuleYOffset = 0f;

                kinematicCharacterMotor.GroundDetectionExtraDistance = kcmParams.GroundDetectionExtraDistance;
                kinematicCharacterMotor.MaxStableSlopeAngle = kcmParams.MaxStableSlopeAngle;
                kinematicCharacterMotor.StableGroundLayers = kcmParams.StableGroundLayers;
                kinematicCharacterMotor.DiscreteCollisionEvents = kcmParams.DiscreteCollisionEvents;

                kinematicCharacterMotor.StepHandling = kcmParams.StepHandling;
                kinematicCharacterMotor.MaxStepHeight = kcmParams.MaxStepHeight;
                kinematicCharacterMotor.AllowSteppingWithoutStableGrounding = kcmParams.AllowSteppingWithoutStableGrounding;
                kinematicCharacterMotor.MinRequiredStepDepth = kcmParams.MinRequiredStepDepth;

                kinematicCharacterMotor.LedgeAndDenivelationHandling = kcmParams.LedgeAndDenivelationHandling;
                kinematicCharacterMotor.MaxStableDistanceFromLedge = kcmParams.MaxStableDistanceFromLedge;
                kinematicCharacterMotor.MaxVelocityForLedgeSnap = kcmParams.MaxVelocityForLedgeSnap;
                kinematicCharacterMotor.MaxStableDenivelationAngle = kcmParams.MaxStableDenivelationAngle;

                kinematicCharacterMotor.InteractiveRigidbodyHandling = kcmParams.InteractiveRigidbodyHandling;
                kinematicCharacterMotor.RigidbodyInteractionType = kcmParams.RigidbodyInteractionType;
                kinematicCharacterMotor.SimulatedCharacterMass = kcmParams.SimulatedCharacterMass;
                kinematicCharacterMotor.PreserveAttachedRigidbodyMomentum = kcmParams.PreserveAttachedRigidbodyMomentum;

                kinematicCharacterMotor.HasPlanarConstraint = kcmParams.HasPlanarConstraint;
                kinematicCharacterMotor.PlanarConstraintAxis = kcmParams.PlanarConstraintAxis;

                kinematicCharacterMotor.CheckMovementInitialOverlaps = kcmParams.CheckMovementInitialOverlaps;
                kinematicCharacterMotor.KillVelocityWhenExceedMaxMovementIterations = kcmParams.KillVelocityWhenExceedMaxMovementIterations;
                kinematicCharacterMotor.KillRemainingMovementWhenExceedMaxMovementIterations = kcmParams.KillRemainingMovementWhenExceedMaxMovementIterations;
                kinematicCharacterMotor.timeUntilUpdate = kcmParams.timeUntilUpdate;
                kinematicCharacterMotor.playerCharacter = kcmParams.playerCharacter;
            }
            return kinematicCharacterMotor;
        }
    }
}
