using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Helpers
{
    public class AimDirectionDumper : MonoBehaviour
    {
        public InputBankTest inputBank;

        public void Awake()
        {
            if (!inputBank) { 
                inputBank = GetComponent<InputBankTest>();
            }
        }


        public void FixedUpdate()
        {
            if(inputBank)
            {
                Log.Info("aimOrigin: " + inputBank.aimOrigin + ", aimDirection: " + inputBank.aimDirection);
            }
        }
    }
}
