using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.Death
{
    public class DeathNormal : GenericCharacterDeath
    {
        //public static GameObject smokeBombPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2SmokeBomb.prefab").WaitForCompletion();

        public override void OnEnter()
        {
            base.OnEnter();
            //EffectManager.SimpleMuzzleFlash(smokeBombPrefab, characterBody.gameObject, "Body", false);
        }
    }
}
