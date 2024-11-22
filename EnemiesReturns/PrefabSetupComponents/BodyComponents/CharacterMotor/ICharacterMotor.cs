using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents.CharacterMotor
{
    public interface ICharacterMotor
    {
        protected class CharacterMotorParams
        {
            public bool muteWalkMotion = false;
            public float mass = 100f;
            public float airControl = 0.25f;
            public bool disableAirControl = false;
            public bool generateParametersOnAwake = true;
        }

        protected bool NeedToAddCharacterMotor();

        protected CharacterMotorParams GetCharacterMotorParams();

        protected RoR2.CharacterMotor AddCharacterMotor(GameObject bodyPrefab, CharacterDirection direction, CharacterMotorParams parameters)
        {
            RoR2.CharacterMotor motor = null;
            if (NeedToAddCharacterMotor())
            {
                motor = bodyPrefab.GetOrAddComponent<RoR2.CharacterMotor>();
                motor.characterDirection = direction;
                motor.muteWalkMotion = parameters.muteWalkMotion;
                motor.mass = parameters.mass;
                motor.airControl = parameters.airControl;
                motor.disableAirControlUntilCollision = parameters.disableAirControl;
                motor.generateParametersOnAwake = parameters.generateParametersOnAwake;
            }

            return motor;
        }
    }
}
