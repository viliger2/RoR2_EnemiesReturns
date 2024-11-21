using System.Collections.Generic;
using UnityEngine;
using static EnemiesReturns.Components.ModelComponents.Hitboxes.IHitBoxGroup;

namespace EnemiesReturns.Components.ModelComponents.Hitboxes
{
    public interface IHitboxes
    {
        protected struct HitBoxesParams
        {
            public string groupName;
            public string[] pathsToTransforms;
        }

        protected bool NeedToAddHitBoxes();

        protected HitBoxesParams[] GetHitBoxesParams();

        protected HitBoxGroupParams[] AddHitboxes(GameObject modelPrefab, HitBoxesParams[] hitBoxesParams)
        {
            List<HitBoxGroupParams> hitBoxGroupParams = new List<HitBoxGroupParams>();
            if (NeedToAddHitBoxes())
            {
#if DEBUG || NOWEAVER
                if(hitBoxesParams.Length == 0)
                {
                    Log.Warning($"No hitbox params for {modelPrefab} despite needing to add them!");
                }
#endif
                foreach (var hitBoxParams in hitBoxesParams)
                {
                    var hitBoxGroupParam = new HitBoxGroupParams();
                    hitBoxGroupParam.name = hitBoxParams.groupName;
                    List<RoR2.HitBox> hitboxes = new List<RoR2.HitBox>();
                    foreach (var pathToTransform in hitBoxParams.pathsToTransforms)
                    {
                        hitboxes.Add(modelPrefab.transform.Find(pathToTransform).gameObject.AddComponent<RoR2.HitBox>());
                    }
                    hitBoxGroupParam.hitboxes = hitboxes.ToArray();
                    hitBoxGroupParams.Add(hitBoxGroupParam);
                }
            }
            return hitBoxGroupParams.ToArray();
        }
    }
}
