using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface IInputBankTest
    {
        public bool NeedToAddInputBankTest();

        internal InputBankTest AddInputBankTest(GameObject bodyPrefab)
        {
            if (NeedToAddInputBankTest())
            {
                return bodyPrefab.GetOrAddComponent<InputBankTest>();
            }
            else
            {
                return null;
            }
        }

    }
}
