using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.ModelComponents.Hitboxes
{
    public interface IHitBoxGroup
    {
        public struct HitBoxGroupParams
        {
            public string name;
            public HitBox[] hitboxes;
        }

        protected bool NeedToAddHitBoxGroups();

        internal void AddHitBoxGroups(GameObject model, HitBoxGroupParams[] hitBoxGroupParams)
        {
            if (NeedToAddHitBoxGroups())
            {
                foreach (var param in hitBoxGroupParams)
                {
                    var hitBoxGroup = model.AddComponent<HitBoxGroup>();
                    hitBoxGroup.groupName = param.name;
                    hitBoxGroup.hitBoxes = param.hitboxes;
                }
            }
        }
    }
}
