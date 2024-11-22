using EnemiesReturns.EditorHelpers;
using System;
using UnityEngine;

namespace EnemiesReturns.Components.ModelComponents
{
    public interface IChildLocator
    {
        protected bool NeedToAddChildLocator();

        protected ChildLocator AddChildLocator(GameObject model)
        {
            ChildLocator childLocator = null;
            if (NeedToAddChildLocator())
            {
                childLocator = model.GetOrAddComponent<ChildLocator>();

                var ourChildLocator = model.GetComponent<OurChildLocator>();
                if (!ourChildLocator)
                {
#if DEBUG || NOWEAVER
                    Log.Warning($"Model {model} is missing OurChildLocator component, ChildLocator will be empty!");
#endif
                    return childLocator;
                }
                childLocator.transformPairs = Array.ConvertAll(ourChildLocator.transformPairs, item =>
                {
                    return new ChildLocator.NameTransformPair
                    {
                        name = item.name,
                        transform = item.transform,
                    };
                });
                UnityEngine.Object.Destroy(ourChildLocator);
            }

            return childLocator;
        }
    }
}
