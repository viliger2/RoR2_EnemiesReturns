using EnemiesReturns.ModCompats.PrefabAPICompat;
using R2API;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Items.ColossalKnurl
{
    public class ColossalKnurlFactory
    {
        public static ItemDef ItemDef;

        public static GameObject projectilePrefab;

        public static R2API.ModdedProcType ColossalFist;

        public ItemDef CreateItem(GameObject prefab, Sprite icon)
        {
            var modelPanelParameters = prefab.AddComponent<ModelPanelParameters>();
            modelPanelParameters.focusPointTransform = prefab.transform.Find("FocusPoint");
            modelPanelParameters.cameraPositionTransform = prefab.transform.Find("CameraPosition");
            modelPanelParameters.modelRotation = Quaternion.identity;
            modelPanelParameters.minDistance = 1f;
            modelPanelParameters.maxDistance = 3f;

            var itemDef = ScriptableObject.CreateInstance<ItemDef>();
            (itemDef as ScriptableObject).name = "ColossalKnurl";
            itemDef.tier = ItemTier.Boss;
            itemDef.deprecatedTier = ItemTier.Boss;
            itemDef.name = "ColossalKnurl";
            itemDef.nameToken = "ENEMIES_RETURNS_ITEM_COLOSSAL_KNURL_NAME";
            itemDef.pickupToken = "ENEMIES_RETURNS_ITEM_COLOSSAL_KNURL_PICKUP";
            itemDef.descriptionToken = "ENEMIES_RETURNS_ITEM_COLOSSAL_KNURL_DESCRIPTION";
            itemDef.loreToken = "ENEMIES_RETURNS_ITEM_COLOSSAL_KNURL_LORE";
            itemDef.pickupModelPrefab = prefab;
            itemDef.canRemove = true;
            itemDef.pickupIconSprite = icon;
            itemDef.tags = new ItemTag[] { ItemTag.Damage, ItemTag.CannotCopy };

            return itemDef;
        }

        public GameObject CreateFistGhostPrefab(GameObject prefab)
        {
            var vfxAttributes = prefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;

            prefab.AddComponent<ProjectileGhostController>().inheritScaleFromProjectile = true;

            return prefab;
        }

        public GameObject CreateFistProjectile(GameObject fistPrefab, GameObject fistGhostPrefab)
        {
            fistPrefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;

            fistPrefab.AddComponent<TeamFilter>();

            var projectileController = fistPrefab.AddComponent<ProjectileController>();
            projectileController.ghostPrefab = fistGhostPrefab;
            projectileController.cannotBeDeleted = true; // why would you allow captain to delete stone fist, that's just inapropriate
            projectileController.canImpactOnTrigger = false;
            projectileController.allowPrediction = true;
            projectileController.procCoefficient = EnemiesReturns.Configuration.Colossus.KnurlProcCoefficient.Value;
            projectileController.startSound = "ER_Knurl_Fist_Spawn_Play";

            var projectileDamage = fistPrefab.AddComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.Generic;
            projectileDamage.useDotMaxStacksFromAttacker = false;

            var projectileImpactExplosion = fistPrefab.AddComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            projectileImpactExplosion.blastRadius = 6f;
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = EnemiesReturns.Configuration.Colossus.KnurlProcCoefficient.Value;
            projectileImpactExplosion.blastAttackerFiltering = AttackerFiltering.Default;
            projectileImpactExplosion.canRejectForce = true;

            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.applyDot = false;

            projectileImpactExplosion.impactOnWorld = true;
            projectileImpactExplosion.lifetime = 0.65f; // matches with animation and sound, DO NOT TOUCH
            projectileImpactExplosion.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleGuardGroundSlam.prefab").WaitForCompletion();

            MyPrefabAPI.RegisterNetworkPrefab(fistPrefab);

            return fistPrefab;
        }

        public static void Hooks()
        {
            Language.onCurrentLangaugeChanged += Language_onCurrentLangaugeChanged;
        }

        public static void OnHitEnemy(DamageInfo damageInfo, CharacterBody attackerBody, GameObject victim)
        {
            if (!damageInfo.procChainMask.HasModdedProc(ColossalFist))
            {
                var itemCount = attackerBody.inventory.GetItemCount(ItemDef);
                if (itemCount > 0 && Util.CheckRoll(EnemiesReturns.Configuration.Colossus.KnurlProcChance.Value * damageInfo.procCoefficient, attackerBody.master))
                {
                    bool isFlying = true; // always assume that the target is flying, so we hit the target instead of trying to find ground beneath
                    if (victim.TryGetComponent<CharacterBody>(out var victimBody))
                    {
                        isFlying = victimBody.isFlying;
                    }

                    var position = damageInfo.position;
                    if (!isFlying && Physics.Raycast(new Ray(damageInfo.position, Vector3.down), out var hitInfo, 1000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                    {
                        position = hitInfo.point;
                    }

                    float damageCoef = EnemiesReturns.Configuration.Colossus.KnurlDamage.Value + EnemiesReturns.Configuration.Colossus.KnurlDamagePerStack.Value * (itemCount - 1);

                    var fireProjectileInfo = default(FireProjectileInfo);
                    fireProjectileInfo.projectilePrefab = projectilePrefab;
                    fireProjectileInfo.position = position;
                    fireProjectileInfo.rotation = Quaternion.identity;
                    fireProjectileInfo.owner = attackerBody.gameObject;
                    fireProjectileInfo.damage = damageInfo.damage * damageCoef;
                    fireProjectileInfo.force = EnemiesReturns.Configuration.Colossus.KnurlForce.Value;
                    fireProjectileInfo.crit = damageInfo.crit;
                    fireProjectileInfo.procChainMask = damageInfo.procChainMask;
                    fireProjectileInfo.procChainMask.AddModdedProc(ColossalFist);
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
            }
        }

        private static void Language_onCurrentLangaugeChanged(RoR2.Language language, List<KeyValuePair<string, string>> output)
        {
            var keyPair = output.Find(item => item.Key == "ENEMIES_RETURNS_ITEM_COLOSSAL_KNURL_DESCRIPTION");
            if (!keyPair.Equals(default(KeyValuePair<string, string>)))
            {
                string description = string.Format(
                    keyPair.Value,
                    (EnemiesReturns.Configuration.Colossus.KnurlProcChance.Value / 100f).ToString("###%"),
                    (EnemiesReturns.Configuration.Colossus.KnurlDamage.Value).ToString("###%"),
                    (EnemiesReturns.Configuration.Colossus.KnurlDamagePerStack.Value).ToString("###%")
                    );
                language.SetStringByToken("ENEMIES_RETURNS_ITEM_COLOSSAL_KNURL_DESCRIPTION", description);
            }
        }
    }
}
