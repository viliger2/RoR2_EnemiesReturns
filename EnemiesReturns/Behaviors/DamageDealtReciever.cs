using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class DamageDealtReciever : MonoBehaviour, IOnDamageDealtServerReceiver
    {
        public bool useDamageType;

        public DamageTypeCombo damageType;

        public bool useTimer;

        public float resetTime;

        [HideInInspector]
        public bool DamageDealt { get; private set; }

        private float timer;

        private void FixedUpdate()
        {
            if (useTimer)
            {
                if(timer >= resetTime)
                {
                    DamageDealt = false;
                    timer -= resetTime;
                }
                timer += Time.fixedDeltaTime;
            }
        }

        public void OnDamageDealtServer(DamageReport damageReport)
        {
            if(damageReport.damageDealt <= 0)
            {
                return;
            }

            if (useDamageType && damageReport.damageInfo.damageType.Equals(damageType)) 
            {
                DamageDealt = true;
            } else
            {
                DamageDealt = true;
            }
        }

        public void ResetDamageDealt()
        {
            DamageDealt = false;
        }
    }
}
