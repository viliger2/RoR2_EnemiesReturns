using EnemiesReturns.Components;
using EnemiesReturns.Items.LunarFlower;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Items.AdrenalineCore
{
    public class AdrenalineCoreMasterComponent : NetworkBehaviour
    {
        public class AdrenalineCoreOnKilledOther : MonoBehaviour, IOnKilledOtherServerReceiver
        {
            public static float healthCheckFreq => Configuration.ContactLight.AdrenalineCore.HealthFreqCheck.Value;

            public static bool transendanceCheck => Configuration.ContactLight.AdrenalineCore.TransendanceSupport.Value;

            public static float criticalDamage => Configuration.ContactLight.AdrenalineCore.HealthThreshold.Value;

            private HealthComponent healthComponent;

            private CharacterBody characterBody;

            private AdrenalineCoreMasterComponent masterComponent;

            private float previousHp;

            private float stopwatch;

            private void Awake()
            {
                characterBody = GetComponent<CharacterBody>();
                healthComponent = GetComponent<HealthComponent>();

                if(characterBody && characterBody.master)
                {
                    masterComponent = characterBody.master.GetComponent<AdrenalineCoreMasterComponent>();
                }
            }

            private void FixedUpdate()
            {
                if (!NetworkServer.active)
                {
                    return;
                }

                if (!masterComponent)
                {
                    return;
                }

                stopwatch += Time.fixedDeltaTime;
                if(stopwatch > healthCheckFreq)
                {
                    return;
                }

                var shieldCheck = transendanceCheck && characterBody.inventory.GetItemCountEffective(RoR2Content.Items.ShieldOnly) > 0;

                // check for losing items
                if (shieldCheck)
                {
                    previousHp = Mathf.Min(previousHp, healthComponent.fullShield);
                } else
                {
                    previousHp = Mathf.Min(previousHp, healthComponent.fullHealth);
                }

                bool hpCheck = shieldCheck
                    ? (previousHp - healthComponent.shield) > healthComponent.fullShield * (criticalDamage / 100f)
                    : (previousHp - healthComponent.health) > healthComponent.fullHealth * (criticalDamage / 100f);

                if(hpCheck && masterComponent && masterComponent.currentPoints > 0)
                {
                    masterComponent.TakeCriticalDamage();
                }

                previousHp = shieldCheck ? healthComponent.shield : healthComponent.health;
                stopwatch -= healthCheckFreq;
            }

            public void OnKilledOtherServer(DamageReport damageReport)
            {
                var masterComponent = damageReport.attackerMaster.GetComponent<AdrenalineCoreMasterComponent>();
                if (masterComponent)
                {
                    masterComponent.OnKilledOtherServer(damageReport);
                }
            }

        }

        public static GameObject levelUpEffect;

        public static GameObject levelDownEffect;

        public static GameObject protectionDestroyedEffect;

        public static int defaultMaxLevel => Configuration.ContactLight.AdrenalineCore.MaxLevel.Value;

        public static int perStackMaxLevel => Configuration.ContactLight.AdrenalineCore.MaxLevelPerStack.Value;

        public static float attackSpeedPerLevel => Configuration.ContactLight.AdrenalineCore.AttackSpeed.Value;

        public static float movementSpeedPerLevel => Configuration.ContactLight.AdrenalineCore.MovementSpeed.Value;

        public static float critChanceAtLevel5 => Configuration.ContactLight.AdrenalineCore.CritChance.Value;

        public static float critDamageAtLevel5 => Configuration.ContactLight.AdrenalineCore.CritDamage.Value;

        public static float itemCountModifier => Configuration.ContactLight.AdrenalineCore.PointPerLevelReduction.Value;

        public static float championPointReward => Configuration.ContactLight.AdrenalineCore.ChampionReward.Value;

#if DEBUG || NOWEAVER 
        public static int normalPointReward => 24;
#else
        public static int normalPointReward => Configuration.ContactLight.AdrenalineCore.NormalReward.Value;
#endif

        public static float tier1EliteModifier => Configuration.ContactLight.AdrenalineCore.T1EliteModifier.Value;

        public static float tier2EliteModifier => Configuration.ContactLight.AdrenalineCore.T2EliteModifier.Value;

        public static float pointsPerLevel => Configuration.ContactLight.AdrenalineCore.PointsPerLevel.Value;

        [SyncVar]
        private float currentPoints;

        [SyncVar]
        private float currentPointsPerLevel;

        public int currentMaxLevel => defaultMaxLevel + (perStackMaxLevel * (itemCount - 1));

        public int currentLevel { get; private set; }

        public bool useShields;

        private int itemCount;

        private CharacterMaster master;

        private bool uiAttached;

        private bool gotProtectionBuff;

        private void Awake()
        {
            master = GetComponent<CharacterMaster>();
            this.enabled = false;
        }

        private void Update()
        {
            if (!uiAttached)
            {
                EnableUI();
            }
        }

        public void Enable()
        {
            if (!master)
            {
                return;
            }

            var bodyObject = master.GetBodyObject();
            if (!bodyObject)
            {
                return;
            }

            currentPoints = 0;

            bodyObject.AddComponent<AdrenalineCoreOnKilledOther>();
            master.onBodyStart += Master_onBodyStart;
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            currentPointsPerLevel = pointsPerLevel;

            EnableUI();

            this.enabled = true;
        }

        private void EnableUI()
        {
            if (!Configuration.ContactLight.AdrenalineCore.EnableUI.Value)
            {
                return;
            }

            var instance = AdrenalineCoreUI.FindInstance(master);
            if (instance)
            {
                instance.Enable(this);
                uiAttached = true;
            }           
        }

        public void Disable()
        {
            var bodyObject = master.GetBodyObject();
            if (bodyObject)
            {
                var adrenalineComponent = bodyObject.GetComponent<AdrenalineCoreOnKilledOther>();
                if (adrenalineComponent)
                {
                    UnityEngine.Object.Destroy(adrenalineComponent);
                }
            }

            var body = bodyObject.GetComponent<CharacterBody>();
            if (body)
            {
                body.RemoveBuff(Content.Buffs.AdrenalineCoreProtection);
                body.SetBuffCount(Content.Buffs.AdrenalineCoreLevels.buffIndex, 0);
            }

            currentPoints = 0f;
            currentLevel = 0;

            master.onBodyStart -= Master_onBodyStart;
            R2API.RecalculateStatsAPI.GetStatCoefficients -= RecalculateStatsAPI_GetStatCoefficients;

            DisableUI();

            this.enabled = false;
        }

        private void DisableUI()
        {
            if (!Configuration.ContactLight.AdrenalineCore.EnableUI.Value)
            {
                return;
            }

            var instance = AdrenalineCoreUI.FindInstance(master);
            if (instance)
            {
                instance.Disable();
                uiAttached = false;
            }
        }

        public void SetItemCount(int itemCount)
        {
            if (itemCount > 0)
            {
                currentPointsPerLevel = pointsPerLevel * (1f - Util.ConvertAmplificationPercentageIntoReductionNormalized((itemCountModifier / 100f) * (itemCount - 1)));
            }

            this.itemCount = itemCount;
        }

        public void TakeCriticalDamage()
        {
            var body = master.GetBody();
            if(body.GetBuffCount(Content.Buffs.AdrenalineCoreProtection) > 0)
            {
                body.RemoveBuff(Content.Buffs.AdrenalineCoreProtection);
                if (protectionDestroyedEffect)
                {
                    EffectData effectData = new EffectData
                    {
                        origin = transform.position,
                        color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.5f)
                    };
                    if (body.mainHurtBox)
                    {
                        effectData.origin = body.mainHurtBox.transform.position;
                        effectData.SetHurtBoxReference(body.gameObject);
                        effectData.scale = body.radius;
                    }
                    EffectManager.SpawnEffect(protectionDestroyedEffect, effectData, transmit: true);
                }
                return;
            }

            if (levelDownEffect)
            {
                EffectData effectData = new EffectData
                {
                    origin = transform.position,
                    color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.5f)
                };
                if (body.mainHurtBox)
                {
                    effectData.origin = body.mainHurtBox.transform.position;
                    effectData.SetHurtBoxReference(body.gameObject);
                    effectData.scale = body.radius;
                }
                EffectManager.SpawnEffect(levelDownEffect, effectData, transmit: true);
            }

            body.SetBuffCount(Content.Buffs.AdrenalineCoreLevels.buffIndex, 0);

            gotProtectionBuff = false;

            currentPoints = 0f;
            currentLevel = 0;
        }

        public void OnKilledOtherServer(DamageReport damageReport)
        {
            if (currentLevel < currentMaxLevel)
            {
                if ((damageReport.victimBody.bodyFlags & CharacterBody.BodyFlags.Masterless) == CharacterBody.BodyFlags.Masterless)
                {
                    return;
                }

                float pointReward;
                if (damageReport.victimIsChampion)
                {
                    pointReward = championPointReward;
                }
                else
                {
                    pointReward = normalPointReward;
                }

                if (damageReport.victimIsElite)
                {
                    var rewardModifier = tier1EliteModifier; // give tier1 reward for elites that could not be found
                    if (damageReport.victimMaster && damageReport.victimMaster.inventory)
                    {
                        var equipmentState = damageReport.victimMaster.inventory.GetActiveEquipment();
                        if (equipmentState.equipmentDef)
                        {
                            var eliteDef = EliteCatalog.GetEliteDefFromEquipmentIndex(equipmentState.equipmentIndex);
                            if (eliteDef)
                            {
                                foreach (var eliteTierDef in CombatDirector.eliteTiers)
                                {
                                    if (Array.Find(eliteTierDef.eliteTypes, item => item == eliteDef))
                                    {
                                        rewardModifier = (int)(eliteTierDef.costMultiplier > CombatDirector.baseEliteCostMultiplier ? tier2EliteModifier : tier1EliteModifier);
                                    }
                                }
                            }
                        }
                    }
                    pointReward *= rewardModifier;
                }

                AddPoints(damageReport.attackerBody, pointReward);
            }
        }

        private void AddPoints(CharacterBody ownerBody, float pointReward)
        {
            currentPoints = Mathf.Min(currentPoints + pointReward, pointsPerLevel * currentMaxLevel);
            if (currentLevel != (int)(currentPoints / currentPointsPerLevel))
            {
                currentLevel = (int)(currentPoints / currentPointsPerLevel);
                if (currentLevel > 0)
                {
                    if (levelUpEffect)
                    {
                        EffectData effectData = new EffectData
                        {
                            origin = transform.position,
                            color = new Color(Color.white.r, Color.white.g, Color.white.b, 0.5f)

                        };
                        if (ownerBody.mainHurtBox)
                        {
                            effectData.origin = ownerBody.mainHurtBox.transform.position;
                            effectData.SetHurtBoxReference(ownerBody.gameObject);
                            effectData.scale = ownerBody.radius;
                        }
                        EffectManager.SpawnEffect(levelUpEffect, effectData, transmit: true);
                    }
                }
                ownerBody.SetBuffCount(Content.Buffs.AdrenalineCoreLevels.buffIndex, currentLevel);

                if (currentLevel >= defaultMaxLevel && !gotProtectionBuff)
                {
                    ownerBody.AddBuff(Content.Buffs.AdrenalineCoreProtection);
                    gotProtectionBuff = true;
                }
                ownerBody.MarkAllStatsDirty();
            }
        }

        public float GetCurrentPointsPerLevel()
        {
            return currentPointsPerLevel;
        }

        public float GetCurrentPoints()
        {
            return currentPoints;
        }

        public static void Hooks()
        {
            if (Configuration.General.EnableAdrenalineCore.Value)
            {
                EnemiesReturns.Language.onCurrentLangaugeChanged += Language_onCurrentLangaugeChanged;
            }
        }

        private static void Language_onCurrentLangaugeChanged(RoR2.Language language, List<KeyValuePair<string, string>> output)
        {
            var keyPair = output.Find(item => item.Key == "ENEMIES_RETURNS_CONTACTLIGHT_ITEM_ADRENALINECORE_DESC");
            if (!keyPair.Equals(default(KeyValuePair<string, string>)))
            {
                string description = string.Format(
                    keyPair.Value,
                    Configuration.ContactLight.AdrenalineCore.MaxLevel.Value,
                    Configuration.ContactLight.AdrenalineCore.MaxLevelPerStack.Value,
                    (Configuration.ContactLight.AdrenalineCore.AttackSpeed.Value / 100f).ToString("###%"),
                    (Configuration.ContactLight.AdrenalineCore.MovementSpeed.Value / 100f).ToString("###%"),
                    (Configuration.ContactLight.AdrenalineCore.CritChance.Value / 100f).ToString("###%"),
                    (Configuration.ContactLight.AdrenalineCore.CritDamage.Value / 100f).ToString("###%"),
                    Configuration.ContactLight.AdrenalineCore.PointsPerLevel.Value,
                    (Configuration.ContactLight.AdrenalineCore.PointPerLevelReduction.Value / 100f).ToString("###%"),
                    Configuration.ContactLight.AdrenalineCore.NormalReward.Value,
                    Configuration.ContactLight.AdrenalineCore.ChampionReward.Value,
                    Configuration.ContactLight.AdrenalineCore.T1EliteModifier.Value,
                    Configuration.ContactLight.AdrenalineCore.T2EliteModifier.Value,
                    (Configuration.ContactLight.AdrenalineCore.HealthThreshold.Value / 100f).ToString("###%"));

                language.SetStringByToken("ENEMIES_RETURNS_CONTACTLIGHT_ITEM_ADRENALINECORE_DESC", description);
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if(sender.master == master)
            {
                args.attackSpeedMultAdd += (attackSpeedPerLevel / 100f) * currentLevel;
                args.moveSpeedMultAdd += (movementSpeedPerLevel / 100f) * currentLevel;
                if(currentLevel > 5)
                {
                    args.critAdd += critChanceAtLevel5;
                    args.critDamageMultAdd += (critDamageAtLevel5 / 100f);
                }
            }
        }

        private void Master_onBodyStart(CharacterBody obj)
        {
            if(!obj.gameObject.TryGetComponent<AdrenalineCoreOnKilledOther>(out _))
            {
                obj.gameObject.AddComponent<AdrenalineCoreOnKilledOther>();
            }
            uiAttached = false; // basically using this as transition between stages
        }

        public static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (Configuration.General.EnableAdrenalineCore.Value)
            {
                if(body && body.master && body.master.TryGetComponent<AdrenalineCoreMasterComponent>(out var component))
                {
                    var itemCount = body.inventory.GetItemCountEffective(Content.Items.AdrenalineCore);
                    if(itemCount > 0)
                    {
                        if (!component.enabled)
                        {
                            component.Enable();
                        }

                        component.SetItemCount(itemCount);
                    } else
                    {
                        if (component.enabled)
                        {
                            component.Disable();
                            component.SetItemCount(0);
                        }
                    }
                }
            }
        }

        internal static void InitMasterCatalog()
        {
            if (Configuration.General.EnableAdrenalineCore.Value)
            {
                // I am sorry for I have sinned
                for (int i = 0; i < MasterCatalog.masterPrefabs.Length; i++)
                {
                    var masterObject = MasterCatalog.masterPrefabs[i];
                    if (masterObject.GetComponent<CharacterMaster>())
                    {
                        var component = masterObject.AddComponent<AdrenalineCoreMasterComponent>();
                        component.enabled = false;
                    }
                }
            }
        }
    }
}
