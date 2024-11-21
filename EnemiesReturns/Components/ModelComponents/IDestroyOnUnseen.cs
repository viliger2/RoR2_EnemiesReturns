using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.ModelComponents
{
    public interface IDestroyOnUnseen
    {
        protected bool NeedToAddDestroyOnUnseen();

        protected DestroyOnUnseen AddDestroyOnUnseen(GameObject model)
        {
            DestroyOnUnseen destroy = null;
            if (NeedToAddDestroyOnUnseen())
            {
                destroy = model.GetOrAddComponent<DestroyOnUnseen>();
            }
            return destroy;
        }
    }
}
