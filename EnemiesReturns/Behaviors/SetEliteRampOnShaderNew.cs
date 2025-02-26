using UnityEngine; 
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors
{
    public class SetEliteRampOnShaderNew : MonoBehaviour
    {
        public Renderer[] renderers;

        public Texture2D vanillaEliteRamp;

        public Texture2D ourEliteRamp;

        public int eliteRampIndex = 0;

        private MaterialPropertyBlock propertyStorage;

        private static int EliteRampPropertyID = Shader.PropertyToID("_EliteRamp");

        private static int EliteIndex = Shader.PropertyToID("_EliteIndex");

        private void Awake()
        {
            propertyStorage = new MaterialPropertyBlock();
            if (renderers == null)
            {
                renderers = GetComponentsInChildren<Renderer>();
            }
            SetEliteRamp(eliteRampIndex);
        }

        private void Update(){
            SetEliteRamp(eliteRampIndex);
        }

        private void SetEliteRamp(int eliteRamp)
        {
            foreach (var renderer in renderers)
            {
                renderer.GetPropertyBlock(propertyStorage);
                if (ourEliteRamp)
                {
                    propertyStorage.SetTexture(EliteRampPropertyID, ourEliteRamp);
                    propertyStorage.SetFloat(EliteIndex, eliteRamp + 1);
                }
                else
                {
                    propertyStorage.SetTexture(EliteRampPropertyID, vanillaEliteRamp);
                    propertyStorage.SetFloat(EliteIndex, eliteRamp + 1);
                }
                renderer.SetPropertyBlock(propertyStorage);
            }
        }
    }
}
