using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class ReplaceShaders : MonoBehaviour
{
    #if UNITY_EDITOR

    const string PREFIX = "Stubbed";
    const int PREFIX_LENGTH = 7;

    // Start is called before the first frame update
    void Awake()
    {
        SwapShaders();
    }

    private void SwapShaders() 
    {
        Renderer[] components = Resources.FindObjectsOfTypeAll<Renderer>();

        foreach(var renderer in components)
        {
            if(renderer.materials != null){
                for(int i = 0; i < renderer.materials.Length; i++)
                {
                    var material = renderer.materials[i];

                    if(!material){
                        continue;
                    }
                    SwapShader(material);

                    renderer.materials[i] = material; 
                }
            } 
            if(!renderer.material){
                continue;
            }
            SwapShader(renderer.material);
        }

        void SwapShader(Material material)
        {
            string stubbedShaderName = material.shader.name;
            if (!stubbedShaderName.StartsWith(PREFIX)) 
            {
                return;
            }
            IList<IResourceLocation> resourceLocations = Addressables.LoadResourceLocationsAsync(stubbedShaderName.Substring(PREFIX_LENGTH) + ".shader", typeof(Shader)).WaitForCompletion();
            if (resourceLocations.Count > 0)
            {
                material.shader = Addressables.LoadAssetAsync<Shader>(resourceLocations[0]).WaitForCompletion();
            }
        }
    }
    #endif
}
