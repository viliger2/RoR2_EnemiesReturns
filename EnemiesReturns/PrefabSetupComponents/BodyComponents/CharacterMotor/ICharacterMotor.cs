﻿using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents.CharacterMotor
{
    public interface ICharacterMotor
    {
        protected class CharacterMotorParams
        {
            public bool muteWalkMotion = false;
            public float airControl = 0.25f;
            public bool disableAirControl = false;
            public bool generateParametersOnAwake = true;
            public bool doNotTriggerJumpVolumes = false;
        }

        protected bool NeedToAddCharacterMotor();

        protected CharacterMotorParams GetCharacterMotorParams();

        protected RoR2.CharacterMotor AddCharacterMotor(GameObject bodyPrefab, CharacterDirection direction, CharacterMotorParams parameters, float mass)
        {
            RoR2.CharacterMotor motor = null;
            if (NeedToAddCharacterMotor())
            {
                motor = bodyPrefab.GetOrAddComponent<RoR2.CharacterMotor>();
                motor.characterDirection = direction;
                motor.muteWalkMotion = parameters.muteWalkMotion;
                motor.mass = mass;
                motor.airControl = parameters.airControl;
                motor.disableAirControlUntilCollision = parameters.disableAirControl;
                motor.generateParametersOnAwake = parameters.generateParametersOnAwake;
                motor.doNotTriggerJumpVolumes = parameters.doNotTriggerJumpVolumes;
            }

            return motor;
        }
    }
}
