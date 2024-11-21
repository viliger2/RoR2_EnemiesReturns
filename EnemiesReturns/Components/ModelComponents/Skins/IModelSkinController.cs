using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.ModelComponents.Skins
{
    public interface IModelSkinController
    {
        protected bool NeedToAddModelSkinController();

        protected ModelSkinController AddModelSkinController(GameObject model, SkinDef[] skinDefs)
        {
            ModelSkinController skinController = null;
            if (NeedToAddModelSkinController())
            {
                skinController = model.GetOrAddComponent<ModelSkinController>();
                skinController.skins = skinDefs;
            }

            return skinController;
        }
    }
}
