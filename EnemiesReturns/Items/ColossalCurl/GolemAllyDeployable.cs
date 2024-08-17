using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Items.ColossalKnurl
{
    public class GolemAllyDeployable : Deployable
    {
        private CharacterMaster master;
        
        private void OnEnable()
        {
            master = GetComponent<CharacterMaster>();
            this.onUndeploy.AddListener(TrueKillMinion);
        }

        private void TrueKillMinion()
        {
            master.TrueKill();
        }
    }
}
