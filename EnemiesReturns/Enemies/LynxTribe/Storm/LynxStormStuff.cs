using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
            effectComponent.soundName = "ER_Lynx_Storm_Release";
            effectComponent.positionAtReferencedTransform = true;
            effectComponent.parentToReferencedTransform = true;

            return prefab;
        }
    }
}
