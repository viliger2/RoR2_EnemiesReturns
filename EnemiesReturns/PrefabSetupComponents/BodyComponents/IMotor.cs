using EnemiesReturns.Components.BodyComponents.CharacterMotor;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface IMotor : ICharacterMotor, IKinematicCharacterMotor
    {
        public void AddMotor(GameObject bodyPrefab, CharacterDirection direction)
        {
            var rigidBody = GetRigidbody(bodyPrefab);
            var motor = AddCharacterMotor(bodyPrefab, direction, GetCharacterMotorParams(), rigidBody ? rigidBody.mass : 100f);
            AddKinematicCharacterMotor(bodyPrefab, GetCapsuleCollider(bodyPrefab), rigidBody, motor, GetKinematicCharacterMotorParams());
        }

        private CapsuleCollider GetCapsuleCollider(GameObject bodyPrefab)
        {
            var capsule = bodyPrefab.GetComponent<CapsuleCollider>();
#if DEBUG || NOWEAVER
            if (!capsule)
            {
                Log.Warning($"Body {bodyPrefab} doesn't have main capsule collider!");
            }
#endif
            return capsule;
        }

        private Rigidbody GetRigidbody(GameObject bodyPrefab)
        {
            var rigidBody = bodyPrefab.GetComponent<Rigidbody>();
#if DEBUG || NOWEAVER
            if (!rigidBody)
            {
                Log.Warning($"Body {bodyPrefab} doesn't have RigidBody!");
            }
#endif
            return rigidBody;
        }

    }
}
