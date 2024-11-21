using EnemiesReturns.Components.BodyComponents.CharacterMotor;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface IMotor : ICharacterMotor, IKinematicCharacterMotor
    {
        public void AddMotor(GameObject bodyPrefab, CharacterDirection direction)
        {
            var motor = AddCharacterMotor(bodyPrefab, direction, GetCharacterMotorParams());
            AddKinematicCharacterMotor(bodyPrefab, GetCapsuleCollider(bodyPrefab), GetRigidbody(bodyPrefab), motor, GetKinematicCharacterMotorParams());
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
