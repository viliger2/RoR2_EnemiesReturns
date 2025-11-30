using R2API;
using RoR2;
using RoR2.CharacterAI;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Junk.Items.ColossalKnurl
{
    public class ColossalKnurlFactoryJunk
    {
        public static DeployableSlot deployableSlot;

        public static CharacterSpawnCard cscGolemAlly;

        public GameObject CreateGolemAllyBody(Texture2D icon)
        {
            var clonedObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/GolemBody.prefab").WaitForCompletion().InstantiateClone("GolemAllyBody", true);

            var body = clonedObject.GetComponent<CharacterBody>();
            body.baseRegen = 0.6f;
            body.levelRegen = 0.12f;
            body.portraitIcon = icon;

            clonedObject.GetComponent<DeathRewards>().logUnlockableDef = null;

            var renderer = clonedObject.transform.Find("ModelBase/mdlGolem/golem").GetComponent<Renderer>();

            renderer.material = ContentProvider.GetOrCreateMaterial("matGolemAlly", CreateGolemAllyMaterial);

            var mdlGolem = clonedObject.transform.Find("ModelBase/mdlGolem").gameObject;

            var characterModel = mdlGolem.GetComponent<CharacterModel>();
            characterModel.baseRendererInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo()
                {
                    renderer = renderer,
                    defaultMaterial = renderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                }
            };

            UnityEngine.Object.DestroyImmediate(mdlGolem.GetComponent<ModelSkinController>());

            return clonedObject;
        }

        private Material CreateGolemAllyMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Golem/matGolem.mat").WaitForCompletion());
            material.name = "matGolemAlly";
            material.SetColor("_Color", new Color(0.92f, 0.92f, 1f)); // slightly blue

            return material;
        }

        // maybe reogranize AISkillDrivers
        public GameObject CreateGolemAllyMaster(GameObject body)
        {
            var clonedObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/GolemMaster.prefab").WaitForCompletion().InstantiateClone("GolemAllyMaster", true);

            clonedObject.GetComponent<CharacterMaster>().bodyPrefab = body;

            #region asdReturnToLeaderLeash
            var asdStomp = clonedObject.AddComponent<AISkillDriver>();
            asdStomp.customName = "ReturnToOwnerLeash";
            asdStomp.skillSlot = SkillSlot.None;

            asdStomp.requiredSkill = null;
            asdStomp.requireSkillReady = false;
            asdStomp.requireEquipmentReady = false;
            asdStomp.minUserHealthFraction = float.NegativeInfinity;
            asdStomp.maxUserHealthFraction = float.PositiveInfinity;
            asdStomp.minTargetHealthFraction = float.NegativeInfinity;
            asdStomp.maxTargetHealthFraction = float.PositiveInfinity;
            asdStomp.minDistance = 100f;
            asdStomp.maxDistance = float.PositiveInfinity;
            asdStomp.selectionRequiresTargetLoS = false;
            asdStomp.selectionRequiresOnGround = false;
            asdStomp.selectionRequiresAimTarget = false;
            asdStomp.maxTimesSelected = -1;

            asdStomp.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            asdStomp.activationRequiresTargetLoS = false;
            asdStomp.activationRequiresAimTargetLoS = false;
            asdStomp.activationRequiresAimConfirmation = false;
            asdStomp.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            asdStomp.moveInputScale = 1;
            asdStomp.aimType = AISkillDriver.AimType.AtCurrentLeader;
            asdStomp.ignoreNodeGraph = false;
            asdStomp.shouldSprint = false;
            asdStomp.shouldFireEquipment = false;
            asdStomp.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdStomp.driverUpdateTimerOverride = 3f;
            asdStomp.resetCurrentEnemyOnNextDriverSelection = true;
            asdStomp.noRepeat = false;
            asdStomp.nextHighPriorityOverride = null;
            #endregion

            #region asdReturnToLeader
            var asdReturnToLeader = clonedObject.AddComponent<AISkillDriver>();
            asdReturnToLeader.customName = "ReturnToLeader";
            asdReturnToLeader.skillSlot = SkillSlot.None;

            asdReturnToLeader.requiredSkill = null;
            asdReturnToLeader.requireSkillReady = false;
            asdReturnToLeader.requireEquipmentReady = false;
            asdReturnToLeader.minUserHealthFraction = float.NegativeInfinity;
            asdReturnToLeader.maxUserHealthFraction = float.PositiveInfinity;
            asdReturnToLeader.minTargetHealthFraction = float.NegativeInfinity;
            asdReturnToLeader.maxTargetHealthFraction = float.PositiveInfinity;
            asdReturnToLeader.minDistance = 15f;
            asdReturnToLeader.maxDistance = float.PositiveInfinity;
            asdReturnToLeader.selectionRequiresTargetLoS = false;
            asdReturnToLeader.selectionRequiresOnGround = false;
            asdReturnToLeader.selectionRequiresAimTarget = false;
            asdReturnToLeader.maxTimesSelected = -1;

            asdReturnToLeader.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            asdReturnToLeader.activationRequiresTargetLoS = false;
            asdReturnToLeader.activationRequiresAimTargetLoS = false;
            asdReturnToLeader.activationRequiresAimConfirmation = false;
            asdReturnToLeader.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            asdReturnToLeader.moveInputScale = 1;
            asdReturnToLeader.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdReturnToLeader.ignoreNodeGraph = false;
            asdReturnToLeader.shouldSprint = false;
            asdReturnToLeader.shouldFireEquipment = false;
            asdReturnToLeader.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdReturnToLeader.driverUpdateTimerOverride = -1f;
            asdReturnToLeader.resetCurrentEnemyOnNextDriverSelection = false;
            asdReturnToLeader.noRepeat = false;
            asdReturnToLeader.nextHighPriorityOverride = null;
            #endregion

            #region asdWaitNearLeader
            var asdWaitNearLeader = clonedObject.AddComponent<AISkillDriver>();
            asdWaitNearLeader.customName = "WaitNearLeaderDefault";
            asdWaitNearLeader.skillSlot = SkillSlot.None;

            asdWaitNearLeader.requiredSkill = null;
            asdWaitNearLeader.requireSkillReady = false;
            asdWaitNearLeader.requireEquipmentReady = false;
            asdWaitNearLeader.minUserHealthFraction = float.NegativeInfinity;
            asdWaitNearLeader.maxUserHealthFraction = float.PositiveInfinity;
            asdWaitNearLeader.minTargetHealthFraction = float.NegativeInfinity;
            asdWaitNearLeader.maxTargetHealthFraction = float.PositiveInfinity;
            asdWaitNearLeader.minDistance = 0f;
            asdWaitNearLeader.maxDistance = float.PositiveInfinity;
            asdWaitNearLeader.selectionRequiresTargetLoS = false;
            asdWaitNearLeader.selectionRequiresOnGround = false;
            asdWaitNearLeader.selectionRequiresAimTarget = false;
            asdWaitNearLeader.maxTimesSelected = -1;

            asdWaitNearLeader.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            asdWaitNearLeader.activationRequiresTargetLoS = true;
            asdWaitNearLeader.activationRequiresAimTargetLoS = false;
            asdWaitNearLeader.activationRequiresAimConfirmation = false;
            asdWaitNearLeader.movementType = AISkillDriver.MovementType.Stop;
            asdWaitNearLeader.moveInputScale = 1;
            asdWaitNearLeader.aimType = AISkillDriver.AimType.AtCurrentLeader;
            asdWaitNearLeader.ignoreNodeGraph = false;
            asdWaitNearLeader.shouldSprint = false;
            asdWaitNearLeader.shouldFireEquipment = false;
            asdWaitNearLeader.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdWaitNearLeader.driverUpdateTimerOverride = -1f;
            asdWaitNearLeader.resetCurrentEnemyOnNextDriverSelection = false;
            asdWaitNearLeader.noRepeat = false;
            asdWaitNearLeader.nextHighPriorityOverride = null;
            #endregion

            clonedObject.AddComponent<GolemAllyDeployable>();

            return clonedObject;
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyGameObject = null)
        {
            var card = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            (card as ScriptableObject).name = name;
            card.prefab = master;
            card.sendOverNetwork = true;
            card.hullSize = HullClassification.Golem;
            card.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            card.requiredFlags = RoR2.Navigation.NodeFlags.None;
            card.forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn;
            card.directorCreditCost = 0;
            card.occupyPosition = false;
            card.eliteRules = SpawnCard.EliteRules.Default;
            card.noElites = false;
            card.forbiddenAsBoss = false;
            if (skin && bodyGameObject && bodyGameObject.TryGetComponent<CharacterBody>(out var body))
            {
                card.loadout = new SerializableLoadout
                {
                    bodyLoadouts = new SerializableLoadout.BodyLoadout[]
                    {
                        new SerializableLoadout.BodyLoadout()
                        {
                            body = body,
                            skinChoice = skin,
                            skillChoices = Array.Empty<SerializableLoadout.BodyLoadout.SkillChoice>() // yes, we need it
                        }
                    }
                };
            };

            return card;
        }

        public static void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        //public static void RecalcStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        //{
        //    var count = sender.inventory.GetItemCountPermanent(ColossalKnurlFactory.itemDef);
        //    if (count > 0)
        //    {
        //        args.armorAdd += 20 + 20 * (count - 1);
        //    }
        //}

        private static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (body && body.inventory)
            {
                body.AddItemBehavior<ColossalKnurlBodyBehavior>(body.inventory.GetItemCountEffective(EnemiesReturns.Content.Items.ColossalCurl));
            }
        }

    }
}
