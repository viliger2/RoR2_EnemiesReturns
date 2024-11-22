using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface ICharacterBody
    {
        protected class CharacterBodyParams
        {
            public CharacterBodyParams(string nameToken, GameObject crosshair, Transform aimOrigin, Texture icon, EntityStates.SerializableEntityStateType initialState)
            {
                this.nameToken = nameToken;
                this.defaultCrosshairPrefab = crosshair;
                this.aimOrigin = aimOrigin;
                this.portraitIcon = icon;
                this.preferredInitialStateType = initialState;
            }

            public string nameToken;
            public string subtitleNameToken = "";
            public CharacterBody.BodyFlags bodyFlags = CharacterBody.BodyFlags.None;
            public bool rootMotionInMainState = false;
            public float mainRootSpeed = 33f;

            public float baseMaxHealth = 100f;
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

        protected bool NeedToAddCharacterBody();

        protected CharacterBodyParams GetCharacterBodyParams(Transform aimOrigin, Texture icon);

        protected CharacterBody AddCharacterBody(GameObject bodyPrefab, CharacterBodyParams bodyParams)
        {
            CharacterBody body = null;
            if (NeedToAddCharacterBody())
            {
                body = bodyPrefab.GetOrAddComponent<CharacterBody>();

                body.baseNameToken = bodyParams.nameToken;
                body.subtitleNameToken = bodyParams.subtitleNameToken;
                body.bodyFlags = bodyParams.bodyFlags;
                body.rootMotionInMainState = bodyParams.rootMotionInMainState;
                body.mainRootSpeed = bodyParams.mainRootSpeed;

                body.baseMaxHealth = bodyParams.baseMaxHealth;
                body.baseRegen = bodyParams.baseRegen;
                body.baseMaxShield = bodyParams.baseMaxShield;
                body.baseMoveSpeed = bodyParams.baseMoveSpeed;
                body.baseAcceleration = bodyParams.baseAcceleration;
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
                body.levelJumpPower = bodyParams.levelJumpPower;
                body.levelDamage = bodyParams.levelDamage;
                body.levelAttackSpeed = bodyParams.levelAttackSpeed;
                body.levelCrit = bodyParams.levelCrit;
                body.levelArmor = bodyParams.levelArmor;

                body.spreadBloomDecayTime = bodyParams.spreadBloomDecayTime;
                body._defaultCrosshairPrefab = bodyParams.defaultCrosshairPrefab;
                body.aimOriginTransform = bodyParams.aimOrigin;
                body.hullClassification = bodyParams.hullClassification;
                body.portraitIcon = bodyParams.portraitIcon;
                body.bodyColor = bodyParams.bodyColor;
                body.isChampion = bodyParams.isChampion;
                body.preferredInitialStateType = bodyParams.preferredInitialStateType;
            }
            return body;
        }
    }
}
