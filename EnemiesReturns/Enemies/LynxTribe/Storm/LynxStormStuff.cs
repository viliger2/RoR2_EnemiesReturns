using EntityStates.VoidRaidCrab.Leg;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe.Storm
{
    public class LynxStormStuff
    {
        public static BuffDef StormImmunity;

        public BuffDef CreateStormImmunityBuff()
        {
            BuffDef defBuff = ScriptableObject.CreateInstance<BuffDef>();
            (defBuff as ScriptableObject).name = "bdLynxStormImmunity";
            defBuff.isDebuff = false;
            defBuff.canStack = false;
            defBuff.isCooldown = true;
            defBuff.isDOT = false;
            defBuff.isHidden = true;
            defBuff.buffColor = Color.green;

            return defBuff;
        }

        public GameObject CreateStormThrowEffect()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoDiseaseImpactEffect.prefab").WaitForCompletion().InstantiateClone("LynxStormThrowEffect", false);

            var effectComponent = prefab.GetComponent<EffectComponent>();
            effectComponent.soundName = ""; // TODO
            effectComponent.positionAtReferencedTransform = true;
            effectComponent.parentToReferencedTransform = true; // TODO: ?

            return prefab;
        }
    }
}
