using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Items.LynxFetish
{
    public class LynxFetishFactory
    {
        public static DeployableSlot LynxFetishDeployable;

        public struct IndexToCards
        {
            public BodyIndex bodyIndex;
            public CharacterSpawnCard spawnCard;
        }

        public static IndexToCards[] spawnCards;

        private static BodyIndex[] bodiesToIgnore;

        public static void Hooks()
        {
            EnemiesReturns.Language.onCurrentLangaugeChanged += Language_onCurrentLangaugeChanged;
            RoR2.CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            LynxFetishDeployable = R2API.DeployableAPI.RegisterDeployableSlot(GetFriendlyLynxTribeCount);
            IL.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
        }

        private static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (NetworkServer.active)
            {
                // we ignore summoned tribesmen, otherwise it causes a chain reaction where each summoned tribesman gets lynx fetish which in turn summons another tribesman, filling the stage with lynx
                if (body && bodiesToIgnore != null && !bodiesToIgnore.Contains(body.bodyIndex) && body.inventory)
                { 
                    body.AddItemBehavior<LynxFetishItemBehavior>(body.inventory.GetItemCount(Content.Items.LynxFetish));
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
                            if ((damageInfo.damageType & DamageSource.Special) == DamageSource.Special && attackerBody.HasBuff(Content.Buffs.LynxShamanSpecialDamage))
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
                if (sender.HasBuff(Content.Buffs.LynxHunterArmor)) args.armorAdd += EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishHunterArmorBuff.Value;
                if (sender.HasBuff(Content.Buffs.LynxArcherDamage)) args.damageMultAdd += EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishArcherDamageBuff.Value / 100f;
                if (sender.HasBuff(Content.Buffs.LynxScoutSpeed))
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
                var archerIndex = BodyCatalog.FindBodyIndex(Enemies.LynxTribe.Archer.ArcherBodyAlly.BodyPrefab);
                var hunterIndex = BodyCatalog.FindBodyIndex(Enemies.LynxTribe.Hunter.HunterBodyAlly.BodyPrefab);
                var scoutIndex = BodyCatalog.FindBodyIndex(Enemies.LynxTribe.Scout.ScoutBodyAlly.BodyPrefab);

                spawnCards = new IndexToCards[]
                {
                    new IndexToCards{ bodyIndex = archerIndex, spawnCard = Enemies.LynxTribe.Archer.ArcherBodyAlly.SpawnCards.cscLynxArcherAlly},
                    new IndexToCards{ bodyIndex = hunterIndex, spawnCard = Enemies.LynxTribe.Hunter.HunterBodyAlly.SpawnCards.cscLynxHunterAlly},
                    new IndexToCards{ bodyIndex = scoutIndex, spawnCard = Enemies.LynxTribe.Scout.ScoutBodyAlly.SpawnCards.cscLynxScoutAlly}
                };

                bodiesToIgnore = new BodyIndex[] { archerIndex, hunterIndex, scoutIndex };

                if (EnemiesReturns.Configuration.LynxTribe.LynxShaman.Enabled.Value)
                {
                    var shamanIndex = BodyCatalog.FindBodyIndex(Enemies.LynxTribe.Shaman.ShamanBodyAlly.BodyPrefab);
                    HG.ArrayUtils.ArrayAppend(ref spawnCards, new IndexToCards { bodyIndex = shamanIndex, spawnCard = Enemies.LynxTribe.Shaman.ShamanBodyAlly.SpawnCards.cscLynxShamanAlly });
                    HG.ArrayUtils.ArrayAppend(ref bodiesToIgnore, shamanIndex);
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
#pragma warning disable CS0618 // Type or member is obsolete
            itemDef.deprecatedTier = ItemTier.Boss;
            itemDef.name = "LynxFetish";
            itemDef.nameToken = "ENEMIES_RETURNS_ITEM_LYNX_FETISH_NAME";
            itemDef.pickupToken = "ENEMIES_RETURNS_ITEM_LYNX_FETISH_PICKUP";
            itemDef.descriptionToken = "ENEMIES_RETURNS_ITEM_LYNX_FETISH_DESCRIPTION";
            itemDef.loreToken = "ENEMIES_RETURNS_ITEM_LYNX_FETISH_LORE";
            itemDef.pickupModelPrefab = prefab;
#pragma warning restore CS0618 // Type or member is obsolete
            itemDef.canRemove = true;
            itemDef.pickupIconSprite = icon;
            itemDef.tags = new ItemTag[] { ItemTag.Utility, ItemTag.CannotCopy };

            return itemDef;
        }

        private static int GetFriendlyLynxTribeCount(CharacterMaster master, int countMultiplier)
        {
            return Mathf.Min(master.inventory.GetItemCount(Content.Items.LynxFetish), 4);
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
