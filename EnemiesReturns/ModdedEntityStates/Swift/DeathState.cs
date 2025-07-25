using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Swift
{
    public class DeathState : EntityStates.Vulture.FallingDeath
    {
        public override void OnEnter()
        {
            base.OnEnter();
            var stones1 = FindModelChild("Stones1");
            if (stones1)
            {
                stones1.gameObject.SetActive(false);
            }

            var stones2 = FindModelChild("Stones2");
            if(stones2)
            {
                stones2.gameObject.SetActive(false);
            }
        }
    }
}
