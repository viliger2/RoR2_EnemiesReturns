using EnemiesReturns.Components;
using EnemiesReturns.Items.LunarFlower;
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
            public static float healthCheckFreq => 0.1f;

            public static bool transendanceCheck => Configuration.ContactLight.AdrenalineCore.TransendanceSupport.Value;

            public static float criticalDamage => 0.1f;

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
                    ? (previousHp - healthComponent.shield) > healthComponent.fullShield * (criticalDamage)
                    : (previousHp - healthComponent.health) > healthComponent.fullHealth * (criticalDamage);

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

        public const int MAX_LEVEL = 5;

        public static GameObject levelUpEffect;

        public static GameObject levelDownEffect;

        private static Dictionary<int, Color> levelColors = new Dictionary<int, Color>()
        {
            {0, new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.5f) },
            {1, new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.5f) },
            {2, new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.5f) },
            {3, new Color(Color.white.r, Color.white.g, Color.white.b, 0.5f) },
            {4, new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.5f) },
            {5, new Color(Color.red.r, Color.red.g, Color.red.b, 0.5f) }
        };

        public static float itemCountModifier => 0.1f;

        public static int championPointReward => 5;

        public static int normalPointReward => 24;

        public static float tier1EliteModifier => 2f;

        public static float tier2EliteModifier => 3f;

        public static float pointsPerLevel => 25;

        [SyncVar]
        public float currentPoints;

        [SyncVar]
        private float currentPointsPerLevel;

        public int currentLevel { get; private set; }

        public bool useShields;

        private int itemCount;

        private CharacterMaster master;

        private bool uiAttached;

        private void Awake()
        {
            master = GetComponent<CharacterMaster>();
            this.enabled = false;
        }

        private void Update()
        {
            if (!uiAttached && Configuration.ContactLight.AdrenalineCore.EnableUI.Value)
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
                currentPointsPerLevel = pointsPerLevel * (1f - Util.ConvertAmplificationPercentageIntoReductionNormalized(itemCountModifier * (itemCount - 1)));
            }

            this.itemCount = itemCount;
        }

        public void TakeCriticalDamage()
        {
            var body = master.GetBody();
            if(body.GetBuffCount(Content.Buffs.AdrenalineCoreProtection) > 0)
            {
                body.RemoveBuff(Content.Buffs.AdrenalineCoreProtection);
                // TODO: play sound and\or effect
                return;
            }

            if (levelDownEffect)
            {
                EffectData effectData = new EffectData
                {
                    origin = transform.position,
                    color = levelColors.GetValueOrDefault(currentLevel)
                };
                if (body.mainHurtBox)
                {
                    effectData.origin = body.mainHurtBox.transform.position;
                    effectData.SetHurtBoxReference(body.gameObject);
                    effectData.scale = body.radius;
                }
                EffectManager.SpawnEffect(levelDownEffect, effectData, transmit: true);
            }

            currentPoints = 0f;
            currentLevel = 0;
        }

        public void OnKilledOtherServer(DamageReport damageReport)
        {
            if (currentLevel < MAX_LEVEL)
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
            currentPoints = Mathf.Min(currentPoints + pointReward, pointsPerLevel * MAX_LEVEL);
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
                            color = levelColors.GetValueOrDefault(currentLevel)

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
                if (currentLevel == MAX_LEVEL)
                {
                    ownerBody.AddBuff(Content.Buffs.AdrenalineCoreProtection);
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

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if(sender.master == master)
            {
                // TODO: values
                args.attackSpeedMultAdd += ((15f / 100) + ((10f / 100) * (itemCount - 1))) * ((currentLevel >= 1) ? 1 : 0);
                args.moveSpeedMultAdd += ((15f / 100) + ((10f / 100) * (itemCount - 1))) * ((currentLevel >= 2) ? 1 : 0);
                args.baseHealthAdd += (25f + (15f * (itemCount - 1))) * ((currentLevel >= 3) ? 1 : 0);
                args.baseShieldAdd += ((sender.maxHealth * 0.1f) + (sender.maxHealth * 0.05f) * (itemCount - 1)) * ((currentLevel >= 4) ? 1 : 0);
                args.baseHealthAdd += (10f + (5f * (itemCount - 1))) * ((currentLevel >= 5) ? 1 : 0);
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
            if (Configuration.General.EnableContactLight.Value && Configuration.General.EnableAdrenalineCore.Value)
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

        [SystemInitializer(new Type[] { typeof(MasterCatalog) })]
        public static void Init()
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
