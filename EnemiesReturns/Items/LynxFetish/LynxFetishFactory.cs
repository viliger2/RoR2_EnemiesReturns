using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Items.LynxFetish
{
    public class LynxFetishFactory
    {
        public static ItemDef ItemDef;

        public static DeployableSlot LynxFetishDeployable;

        public struct IndexToCards
        {
            public BodyIndex bodyIndex;
            public CharacterSpawnCard spawnCard;
        }

        public static IndexToCards[] spawnCards;

        public static void Hooks()
        {
            EnemiesReturns.Language.onCurrentLangaugeChanged += Language_onCurrentLangaugeChanged;
            RoR2.CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            LynxFetishDeployable = R2API.DeployableAPI.RegisterDeployableSlot(GetFriendlyLyxTribeCount);
            IL.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
        }

        private static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (NetworkServer.active)
            {
                if (body && body.inventory)
                {
                    body.AddItemBehavior<LynxFetishItemBehavior>(body.inventory.GetItemCount(ItemDef));
                }
            }
        }

        private static void HealthComponent_TakeDamageProcess(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchStloc(7)))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate<Func<float, HealthComponent, DamageInfo, float>>((origDamage, victimHealth, damageInfo) =>
                {
                    float newDamage = origDamage;
                    CharacterBody victimBody = victimHealth.body;
                    if (victimBody && damageInfo.attacker)
                    {
                        CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                        if (attackerBody)
                        {
                            if ((damageInfo.damageType & DamageSource.Special) == DamageSource.Special && attackerBody.HasBuff(Enemies.LynxTribe.Shaman.ShamanBodyAlly.LynxShamanSpecial))
                            {
                                newDamage *= 1 + (EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishShamanSpecialBuff.Value / 100f);
                            }
                        }
                    }
                    return newDamage;
                });
            }
            else
            {
                Log.Warning("ILHook failed: HealthComponent_TakeDamageProcess");
            }
        }

        public static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender)
            {
                if (sender.HasBuff(Enemies.LynxTribe.Hunter.HunterBodyAlly.LynxHunterArmor)) args.armorAdd += EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishHunterArmorBuff.Value;
                if (sender.HasBuff(Enemies.LynxTribe.Archer.ArcherBodyAlly.LynxArcherDamage)) args.damageMultAdd += EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishArcherDamageBuff.Value / 100f;
                if (sender.HasBuff(Enemies.LynxTribe.Scout.ScoutBodyAlly.LynxScoutSpeed))
                {
                    args.moveSpeedMultAdd += EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishScoutSpeedBuff.Value / 100f;
                    args.attackSpeedMultAdd += EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishScoutSpeedBuff.Value / 100f;
                }
            }
        }

        [SystemInitializer(new Type[] { typeof(BodyCatalog) })]
        public static void Init()
        {
            if (!RoR2.BodyCatalog.availability.available)
            {
                Log.Warning("Somehow got here without inialized BodyCatalog.");
            }

            if (EnemiesReturns.Configuration.LynxTribe.LynxTotem.Enabled.Value && EnemiesReturns.Configuration.LynxTribe.LynxTotem.ItemEnabled.Value)
            {
                spawnCards = new IndexToCards[]
                {
                    new IndexToCards{ bodyIndex = BodyCatalog.FindBodyIndex(Enemies.LynxTribe.Archer.ArcherBodyAlly.BodyPrefab), spawnCard = Enemies.LynxTribe.Archer.ArcherBodyAlly.SpawnCards.cscLynxArcherAlly},
                    new IndexToCards{ bodyIndex = BodyCatalog.FindBodyIndex(Enemies.LynxTribe.Hunter.HunterBodyAlly.BodyPrefab), spawnCard = Enemies.LynxTribe.Hunter.HunterBodyAlly.SpawnCards.cscLynxHunterAlly},
                    new IndexToCards{ bodyIndex = BodyCatalog.FindBodyIndex(Enemies.LynxTribe.Scout.ScoutBodyAlly.BodyPrefab), spawnCard = Enemies.LynxTribe.Scout.ScoutBodyAlly.SpawnCards.cscLynxScoutAlly}
                };

                if (EnemiesReturns.Configuration.LynxTribe.LynxShaman.Enabled.Value)
                {
                    HG.ArrayUtils.ArrayAppend(ref spawnCards, new IndexToCards { bodyIndex = BodyCatalog.FindBodyIndex(Enemies.LynxTribe.Shaman.ShamanBodyAlly.BodyPrefab), spawnCard = Enemies.LynxTribe.Shaman.ShamanBodyAlly.SpawnCards.cscLynxShamanAlly });
                }
            }
        }

        public ItemDef CreateItem(GameObject prefab, Sprite icon)
        {
            var modelPanelParameters = prefab.AddComponent<ModelPanelParameters>();
            modelPanelParameters.focusPointTransform = prefab.transform.Find("FocusPoint");
            modelPanelParameters.cameraPositionTransform = prefab.transform.Find("CameraPosition");
            modelPanelParameters.modelRotation = Quaternion.identity;
            modelPanelParameters.minDistance = 5f;
            modelPanelParameters.maxDistance = 9f;

            var itemDef = ScriptableObject.CreateInstance<ItemDef>();
            (itemDef as ScriptableObject).name = "LynxFetish";
            itemDef.tier = ItemTier.Boss;
            itemDef.deprecatedTier = ItemTier.Boss;
            itemDef.name = "LynxFetish";
            itemDef.nameToken = "ENEMIES_RETURNS_ITEM_LYNX_FETISH_NAME";
            itemDef.pickupToken = "ENEMIES_RETURNS_ITEM_LYNX_FETISH_PICKUP";
            itemDef.descriptionToken = "ENEMIES_RETURNS_ITEM_LYNX_FETISH_DESCRIPTION";
            itemDef.loreToken = "ENEMIES_RETURNS_ITEM_LYNX_FETISH_LORE";
            itemDef.pickupModelPrefab = prefab;
            itemDef.canRemove = true;
            itemDef.pickupIconSprite = icon;
            itemDef.tags = new ItemTag[] { ItemTag.Utility, ItemTag.CannotCopy, ItemTag.AIBlacklist };

            return itemDef;
        }

        private static int GetFriendlyLyxTribeCount(CharacterMaster master, int countMultiplier)
        {
            return Mathf.Min(master.inventory.GetItemCount(ItemDef), 4);
        }

        private static void Language_onCurrentLangaugeChanged(RoR2.Language language, List<KeyValuePair<string, string>> output)
        {
            var keyPair = output.Find(item => item.Key == "ENEMIES_RETURNS_ITEM_LYNX_FETISH_DESCRIPTION");
            if (!keyPair.Equals(default(KeyValuePair<string, string>)))
            {
                string description = string.Format(
                    keyPair.Value,
                    (EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBonusDamage.Value / 10f).ToString("###%"),
                    (EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBonusDamagePerStack.Value / 10f).ToString("###%"),
                    (EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBonusHP.Value / 10f).ToString("###%"),
                    (EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBonusHPPerStack.Value / 10f).ToString("###%"),
                    EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBuffWardRadius.Value,
                    (EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishArcherDamageBuff.Value / 100f).ToString("###%"),
                    EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishHunterArmorBuff.Value,
                    (EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishScoutSpeedBuff.Value / 100f).ToString("###%"),
                    (EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishShamanSpecialBuff.Value / 100f).ToString("###%"));
                language.SetStringByToken("ENEMIES_RETURNS_ITEM_LYNX_FETISH_DESCRIPTION", description);
            }
        }
    }
}
