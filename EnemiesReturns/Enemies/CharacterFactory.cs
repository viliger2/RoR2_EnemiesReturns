using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Enemies
{
    public abstract class CharacterFactory
    {
        public class CharacterMotorParams
        {
            public CharacterMotorParams(CharacterDirection direction)
            {
                this.direction = direction;
            }

            public CharacterDirection direction;
            public bool muteWalkMotion = false;
            public float mass = 100f;
            public float airControl = 0.25f;
            public bool disableAirControl = false;
            public bool generateParametersOnAwake = true;
        }
        
        public class CharacterBodyParams
        {
            public CharacterBodyParams(string nameToken, GameObject crosshair, Transform aimOrigin, Texture icon, EntityStates.SerializableEntityStateType initialState)
            {
                this.nameToken = nameToken;
                this.defaultCrosshairPrefab = crosshair;
                this.aimOrigin= aimOrigin;
                this.portraitIcon= icon;
                this.preferredInitialStateType= initialState;
            }

            public string nameToken;
            public CharacterBody.BodyFlags flags= CharacterBody.BodyFlags.None;
            public bool rootMotionInMainState = false;
            public float mainRootSpeed = 33f;

            public float baseMaxHealth = 300f;
            public float baseRegen = 0f;
            public float baseMaxShield = 0f;
            public float baseMoveSpeed = 7f;
            public float baseAcceleration = 40f;
            public float baseJumpPower = 20f;
            public float baseDamage = 20f;
            public float baseAttackSpeed = 1f;
            public float baseCrit = 0f;
            public float baseArmor = 0f;
            public float baseVisionDistance = float.PositiveInfinity;
            public int baseJumpCount = 1;
            public float sprintingMultiplier = 1.45f;

            public float levelMaxHealth = 90f;
            public float levelRegen = 0f;
            public float levelMaxShield = 0f;
            public float levelMoveSpeed = 0f;
            public float levelJumpPower = 0f;
            public float levelDamage = 4f;
            public float levelAttackSpeed = 0f;
            public float levelCrit = 0f;
            public float levelArmor = 0f;

            public float spreadBloomDecayTime = 0.45f;
            public GameObject defaultCrosshairPrefab;
            public Transform aimOrigin;
            public HullClassification hullClassification = HullClassification.Human;
            public Texture portraitIcon;
            public Color bodyColor = Color.red;
            public bool isChampion = false;
            public EntityStates.SerializableEntityStateType preferredInitialStateType;

            public bool autoCalculateStats 
            { 
                get => _autoCalculateStats; 
                set 
                { 
                    if (value) 
                    { 
                        PerformAutoCalculateLevelStats();
                        _autoCalculateStats = value;
                    } 
                } 
            }

            private bool _autoCalculateStats = false;

            public void PerformAutoCalculateLevelStats()
            {
                levelMaxHealth = Mathf.Round(baseMaxHealth * 0.3f);
                levelMaxShield = Mathf.Round(baseMaxShield * 0.3f);
                levelRegen = baseRegen * 0.2f;
                levelMoveSpeed = 0f;
                levelJumpPower = 0f;
                levelDamage = baseDamage * 0.2f;
                levelAttackSpeed = 0f;
                levelCrit = 0f;
                levelArmor = 0f;
            }
        }

        public CharacterDirection AddCharacterDirection(GameObject bodyPrefab, Transform modelBase, float turnSpeed, Animator modelAnimator)
        {
            CharacterDirection direction;
            if (!bodyPrefab.TryGetComponent<CharacterDirection>(out direction))
            {
                direction = bodyPrefab.AddComponent<CharacterDirection>();
            }
            direction.targetTransform = modelBase;
            direction.turnSpeed = turnSpeed;
            direction.modelAnimator = modelAnimator;

            return direction;
        }

        public CharacterMotor AddCharacterMotor(GameObject bodyPrefab, CharacterMotorParams parameters)
        {
            CharacterMotor motor;
            if (!bodyPrefab.TryGetComponent<CharacterMotor>(out motor))
            {
                motor = bodyPrefab.AddComponent<CharacterMotor>();
            }
            motor.characterDirection = parameters.direction;
            motor.muteWalkMotion= parameters.muteWalkMotion;
            motor.mass = parameters.mass;
            motor.airControl = parameters.airControl;
            motor.disableAirControlUntilCollision = parameters.disableAirControl;
            motor.generateParametersOnAwake= parameters.generateParametersOnAwake;

            return motor;
        }

        public InputBankTest AddInputBankTest(GameObject bodyPrefab)
        {
            InputBankTest bankTest;
            if (!bodyPrefab.TryGetComponent<InputBankTest>(out bankTest))
            {
                bankTest = bodyPrefab.AddComponent<InputBankTest>();
            }
            return bankTest;
        }
        
        public CharacterBody AddCharacterBody(GameObject bodyPrefab, CharacterBodyParams bodyParams)
        {
            CharacterBody body;
            if (!bodyPrefab.TryGetComponent<CharacterBody>(out body))
            {
                body = bodyPrefab.AddComponent<CharacterBody>();
            }

            body.baseNameToken = bodyParams.nameToken;
            body.bodyFlags = bodyParams.flags;
            body.rootMotionInMainState = bodyParams.rootMotionInMainState;
            body.mainRootSpeed= bodyParams.mainRootSpeed;

            body.baseMaxHealth= bodyParams.baseMaxHealth;
            body.baseRegen= bodyParams.baseRegen;
            body.baseMaxShield = bodyParams.baseMaxShield;
            body.baseMoveSpeed= bodyParams.baseMoveSpeed;
            body.baseAcceleration= bodyParams.baseAcceleration;
            body.baseJumpPower = bodyParams.baseJumpPower;
            body.baseDamage = bodyParams.baseDamage;
            body.baseAttackSpeed = bodyParams.baseAttackSpeed;
            body.baseCrit = bodyParams.baseCrit;
            body.baseArmor = bodyParams.baseArmor;
            body.baseVisionDistance = bodyParams.baseVisionDistance;
            body.baseJumpCount = bodyParams.baseJumpCount;
            body.sprintingSpeedMultiplier = bodyParams.sprintingMultiplier;

            body.levelMaxHealth = bodyParams.levelMaxHealth;
            body.levelRegen = bodyParams.levelRegen;
            body.levelMaxShield = bodyParams.levelMaxShield;
            body.levelMoveSpeed = bodyParams.levelMoveSpeed;
            body.levelJumpPower= bodyParams.levelJumpPower;
            body.levelDamage= bodyParams.levelDamage;
            body.levelAttackSpeed= bodyParams.levelAttackSpeed;
            body.levelCrit= bodyParams.levelCrit;
            body.levelArmor = bodyParams.levelArmor;

            body.spreadBloomDecayTime = bodyParams.spreadBloomDecayTime;
            body._defaultCrosshairPrefab= bodyParams.defaultCrosshairPrefab;
            body.aimOriginTransform = bodyParams.aimOrigin;
            body.hullClassification= bodyParams.hullClassification;
            body.portraitIcon = bodyParams.portraitIcon;
            body.bodyColor=bodyParams.bodyColor;
            body.isChampion =bodyParams.isChampion;
            body.preferredInitialStateType= bodyParams.preferredInitialStateType;

            return body;
        }

        public CameraTargetParams AddCameraTargetParams(GameObject bodyPrefab, CharacterCameraParams cameraParams)
        {
            CameraTargetParams cameraTargetParams;
            if(!bodyPrefab.TryGetComponent<CameraTargetParams>(out cameraTargetParams))
            {
                cameraTargetParams = bodyPrefab.AddComponent<CameraTargetParams>(); 
            }

            cameraTargetParams.cameraParams = cameraParams;

            return cameraTargetParams;
        }

        public void Test()
        {

        }
    }
}
