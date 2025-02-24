using RoR2;
using UnityEngine;

namespace EnemiesReturns.Behaviors.Garbage
{
    public class AimDirectionDumper : MonoBehaviour
    {
        public InputBankTest inputBank;

        public void Awake()
        {
            if (!inputBank)
            {
                inputBank = GetComponent<InputBankTest>();
            }
        }


        public void FixedUpdate()
        {
            if (inputBank)
            {
                Log.Info("aimOrigin: " + inputBank.aimOrigin + ", aimDirection: " + inputBank.aimDirection);
            }
        }
    }
}
