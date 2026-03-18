using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ReplaceShaders : MonoBehaviour
{
    #if UNITY_EDITOR

        public static readonly Dictionary<string, string> ShaderLookup = new Dictionary<string, string>()
        {
            {"stubbedror2/base/shaders/hgstandard", "RoR2/Base/Shaders/HGStandard.shader"},
            {"stubbedror2/base/shaders/hgsnowtopped", "RoR2/Base/Shaders/HGSnowTopped.shader"},
            {"stubbedror2/base/shaders/hgtriplanarterrainblend", "RoR2/Base/Shaders/HGTriplanarTerrainBlend.shader"},
            {"stubbedror2/base/shaders/hgintersectioncloudremap", "RoR2/Base/Shaders/HGIntersectionCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgcloudremap", "RoR2/Base/Shaders/HGCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgopaquecloudremap", "RoR2/Base/Shaders/HGOpaqueCloudRemap.shader" },
            {"stubbedror2/base/shaders/hgdistortion", "RoR2/Base/Shaders/HGDistortion.shader" },
            {"stubbedcalm water/calmwater - dx11 - doublesided", "Calm Water/CalmWater - DX11 - DoubleSided.shader" },
            {"stubbedcalm water/calmwater - dx11", "Calm Water/CalmWater - DX11.shader" },
            {"stubbednature/speedtree", "RoR2/Base/Shaders/SpeedTreeCustom.shader"},
            {"stubbeddecalicious/decaliciousdeferreddecal", "Decalicious/DecaliciousDeferredDecal.shader" },
            {"stubbedror2/base/shaders/hgdamagenumber", "RoR2/Base/Shaders/HGDamageNumber.shader" },
            {"stubbedror2/base/shaders/hguianimatealpha", "RoR2/Base/Shaders/HGUIAnimateAlpha.shader" }
        };

    // Start is called before the first frame update
    void Awake()
    {
        Renderer[] components = Resources.FindObjectsOfTypeAll<Renderer>();

        foreach(var renderer in components)
        {
            if(!renderer.material){
                continue;
            }
            if (ShaderLookup.TryGetValue(renderer.material.shader.name.ToLower(), out var matName))
            {
                var replacementShader = Addressables.LoadAssetAsync<Shader>(matName).WaitForCompletion();
                if (replacementShader)
                {
                    renderer.material.shader = replacementShader;
                }
            }
        }
    }
    #endif
}
