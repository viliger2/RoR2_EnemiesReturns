using RoR2;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.Components.ModelComponents.Hurtboxes
{
    public interface IHurtBoxGroup
    {
        protected bool NeedToAddHurtBoxGhoup();

        protected HurtBoxGroup AddHurtBoxGroup(GameObject model, HurtBox[] hurtboxes)
        {
            HurtBoxGroup hurtBoxGroup = null;
            if (NeedToAddHurtBoxGhoup())
            {
                hurtBoxGroup = model.GetOrAddComponent<HurtBoxGroup>();
                if (hurtboxes == null || hurtboxes.Length == 0)
                {
#if DEBUG || NOWEAVER
                    Log.Warning($"Hurtbox array for model {model} is empty while trying to add HurtBoxGroup!");
#endif
                    return hurtBoxGroup;
                }

                hurtBoxGroup.hurtBoxes = hurtboxes;
                for (short i = 0; i < hurtBoxGroup.hurtBoxes.Length; i++)
                {
                    hurtBoxGroup.hurtBoxes[i].hurtBoxGroup = hurtBoxGroup;
                    hurtBoxGroup.hurtBoxes[i].indexInGroup = i;
                    if (hurtBoxGroup.hurtBoxes[i].isBullseye)
                    {
                        hurtBoxGroup.bullseyeCount++;
                    }
                    if (hurtBoxGroup.hurtBoxes[i].transform.name == "MainHurtBox")
                    {
                        hurtBoxGroup.mainHurtBox = hurtBoxGroup.hurtBoxes[i];
                    }
                }
            }

            return hurtBoxGroup;
        }
    }
}
