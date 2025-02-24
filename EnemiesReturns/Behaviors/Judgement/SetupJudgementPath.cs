using EnemiesReturns.Behaviors.Judgement.MithrixWeaponDrop;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.Judgement
{
    public static class SetupJudgementPath
    {
        public static GameObject PileOfDirt;

        public static GameObject BrokenTeleporter;

        public static void SpawnObjects(SceneDirector sceneDirector)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!SceneInfo.instance
                || !SceneInfo.instance.sceneDef)
            {
                return;
            }

            if(SceneInfo.instance.sceneDef.baseSceneName == "arena"
                && PileOfDirt)
            {
                var newPile = UnityEngine.Object.Instantiate(PileOfDirt);
                newPile.transform.position = new Vector3(136.779999f, 18.2870007f, 300.329987f);
                newPile.transform.rotation = Quaternion.identity;
                NetworkServer.Spawn(newPile);
            }

            if (SceneInfo.instance.sceneDef.baseSceneName == "moon2"
                && BrokenTeleporter)
            {
                var newTeleporter = UnityEngine.Object.Instantiate(BrokenTeleporter);
                newTeleporter.transform.position = new Vector3(-236.809998f, 489.529999f, -89.0899963f);
                newTeleporter.transform.rotation = Quaternion.identity;
                NetworkServer.Spawn(newTeleporter);
            }
        }

        public static void AddInteractabilityToNewt()
        {
            var shopkeeperBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Shopkeeper/ShopkeeperBody.prefab").WaitForCompletion();

            var newtTrader = shopkeeperBody.AddComponent<Behaviors.Judgement.Newt.NewtTrader>();
            newtTrader.contextString = "ENEMIES_RETURNS_JUDGEMENT_NEWT_TRADE_CONTEXT";
            newtTrader.itemToTake = Content.Items.TradableRock;
            newtTrader.itemToGive = Content.Items.LunarFlower;
            newtTrader.available = true; // TODO: eh?

            var highlight = shopkeeperBody.AddComponent<Highlight>();
            highlight.targetRenderer = shopkeeperBody.transform.Find("ModelBase/mdlNewtShopkeeper/NewtMesh").gameObject.GetComponent<Renderer>();

            shopkeeperBody.AddComponent<EntityLocator>().entity = shopkeeperBody;
        }

        public static void AddWeaponDropToMithrix()
        {
            var mithrixHurtBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherHurtMaster.prefab").WaitForCompletion();

            var dropEquipment = mithrixHurtBody.AddComponent<DropEquipment>();
            dropEquipment.itemToCheck = Content.Items.LunarFlower;
            dropEquipment.equipmentToDrop = Content.Equipment.MithrixHammer;
        }

    }
}
