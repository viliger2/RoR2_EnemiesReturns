using EnemiesReturns.Reflection;
using EntityStates.Engi.EngiWeapon;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Engi
{
    [RegisterEntityState]
    public class PlaceMechSpider : PlaceTurret
    {
        public static GameObject spiderTurretMasterPrefab;

        public static GameObject spiderBlueprintPrefab;

        public static GameObject wristDisplay = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiTurretWristDisplay.prefab").WaitForCompletion();

        public override void OnEnter()
        {
            blueprintPrefab = spiderBlueprintPrefab;
            wristDisplayPrefab = wristDisplay;
            turretMasterPrefab = spiderTurretMasterPrefab;
            base.OnEnter();
        }
    }
}
