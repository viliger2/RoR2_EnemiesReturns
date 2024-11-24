using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors
{
    public class SetEliteRampOnShader : NetworkBehaviour
    {
        [SyncVar(hook = "OnChangeEliteRamp")]
        public int eliteRampIndex;

        [SyncVar(hook = "OnChangeEliteIndex")]
        public int eliteIndex;

        public Renderer[] renderers;

        private MaterialPropertyBlock propertyStorage;

        private static int EliteRampPropertyID => Shader.PropertyToID("_EliteRamp");

        private static Texture2D vanillaEliteRamp = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/GlobalTextures/texRampElites.psd").WaitForCompletion();

        private void Awake()
        {
            propertyStorage = new MaterialPropertyBlock();
            if (renderers == null)
            {
                renderers = GetComponentsInChildren<Renderer>();
            }
        }

        // syncvars are not called on awake
        // so we manually call the method since value is already correct by the time OnStartClient happens
        public override void OnStartClient()
        {
            base.OnStartClient();
            SetEliteRamp(eliteRampIndex, (EliteIndex)eliteIndex);
        }

        public void SetEliteRampIndex(int eliteRampIndex, EliteIndex eliteIndex)
        {
            this.eliteRampIndex = eliteRampIndex;
            this.eliteIndex = (int)eliteIndex;
            SetEliteRamp(eliteRampIndex, eliteIndex);
        }

        public void OnChangeEliteIndex(int eliteIndex)
        {
            this.eliteIndex = eliteIndex;
            SetEliteRamp(eliteRampIndex, (EliteIndex)this.eliteIndex);
        }

        public void OnChangeEliteRamp(int eliteRamp)
        {
            this.eliteRampIndex = eliteRamp;
            SetEliteRamp(eliteRampIndex, (EliteIndex)eliteIndex);
        }

        private void SetEliteRamp(int eliteRamp, EliteIndex eliteIndex)
        {
            Texture2D texture = null;
            bool nonVanillaElite = R2API.EliteRamp.TryGetRamp(eliteIndex, out texture);

            foreach (var renderer in renderers)
            {
                renderer.GetPropertyBlock(propertyStorage);
                if (nonVanillaElite && texture)
                {
                    propertyStorage.SetTexture(EliteRampPropertyID, texture);
                    propertyStorage.SetFloat(CommonShaderProperties._EliteIndex, eliteRamp + 1);
                }
                else
                {
                    propertyStorage.SetTexture(EliteRampPropertyID, vanillaEliteRamp);
                    propertyStorage.SetFloat(CommonShaderProperties._EliteIndex, eliteRamp + 1);
                }
                renderer.SetPropertyBlock(propertyStorage);
            }
        }
    }
}
