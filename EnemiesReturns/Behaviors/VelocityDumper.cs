using KinematicCharacterController;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
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
