using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static RoR2.BossGroup;

namespace EnemiesReturns.Behaviors.Judgement
{
    public class SceneSpecificHooks : MonoBehaviour
    {
        private void Start()
        {
            On.RoR2.UI.HUDBossHealthBarController.LateUpdate += HUDBossHealthBarController_LateUpdate;
            On.RoR2.PickupDropletController.CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3 += PickupDropletController_CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3;
        }

        private void PickupDropletController_CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3(On.RoR2.PickupDropletController.orig_CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3 orig, GenericPickupController.CreatePickupInfo pickupInfo, Vector3 position, Vector3 velocity)
        {
            if (RoR2.Stage.instance.sceneDef.cachedName == "enemiesreturns_outoftime")
            {
                var pickupDef = PickupCatalog.GetPickupDef(pickupInfo.pickupIndex);
                if (pickupDef != null)
                {
                    if(pickupDef.equipmentIndex != Content.Equipment.EliteAeonian.equipmentIndex
                        && pickupDef.equipmentIndex != Content.Equipment.MithrixHammer.equipmentIndex)
                    {
                        return;
                    }
                }
            }
            orig(pickupInfo, position, velocity);
        }

        private void HUDBossHealthBarController_LateUpdate(On.RoR2.UI.HUDBossHealthBarController.orig_LateUpdate orig, RoR2.UI.HUDBossHealthBarController self)
        {
            orig(self);
            BossGroupHealthColorOverride.ReplaceColor(self);
            BossGroupTextOverride.ReplaceNames(self);
        }

        private void OnDestroy()
        {
            On.RoR2.UI.HUDBossHealthBarController.LateUpdate -= HUDBossHealthBarController_LateUpdate;
            On.RoR2.PickupDropletController.CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3 -= PickupDropletController_CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3;
        }

    }
}
