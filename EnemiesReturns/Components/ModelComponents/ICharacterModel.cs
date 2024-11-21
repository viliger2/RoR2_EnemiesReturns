using RoR2;
using UnityEngine;
using UnityEngine.Rendering;

namespace EnemiesReturns.Components.ModelComponents
{
    public interface ICharacterModel
    {
        protected struct MyRendererInfo
        {
            public string pathToRenderer;
            public ShadowCastingMode defaultShadowCastingMode;
            public bool ignoreOverlays;
            public bool hideOnDeath;
        }

        protected class CharacterModelParams
        {
            public CharacterModel.RendererInfo[] renderInfos;
            public bool autoPopulateLightInfos = true;
        }

        public ItemDisplayRuleSet GetItemDisplayRuleSet();

        protected CharacterModelParams GetCharacterModelParams(GameObject modelPrefab);

        protected bool NeedToAddCharacterModel();

        protected CharacterModel AddCharacterModel(GameObject model, CharacterBody body, ItemDisplayRuleSet idrs, CharacterModelParams characterModelParams)
        {
            CharacterModel characterModel = null;
            if (NeedToAddCharacterModel())
            {
                characterModel = model.GetOrAddComponent<CharacterModel>();

                characterModel.body = body;
                characterModel.itemDisplayRuleSet = idrs;
                characterModel.autoPopulateLightInfos = characterModelParams.autoPopulateLightInfos;
                characterModel.baseRendererInfos = characterModelParams.renderInfos;
            }

            return characterModel;
        }
    }
}
