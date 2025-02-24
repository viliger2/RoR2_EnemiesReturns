using KinematicCharacterController;
using UnityEngine;

namespace EnemiesReturns.Behaviors.Garbage
{
    public class VelocityDumper : MonoBehaviour
    {
        private KinematicCharacterMotor kcm;

        private void OnEnable()
        {
            kcm = GetComponent<KinematicCharacterMotor>();
        }

        private void Update()
        {
            if (kcm)
            {
                Log.Info($"velocity: {kcm.Velocity}, rigidbodyvelocity: {kcm.AttachedRigidbodyVelocity}");
            }
        }

    }
}
