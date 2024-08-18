using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Junk.Items.ColossalKnurl
{
    public class GolemAllyDeployable : Deployable
    {
        private CharacterMaster master;

        private void OnEnable()
        {
            master = GetComponent<CharacterMaster>();
            onUndeploy.AddListener(TrueKillMinion);
        }

        private void TrueKillMinion()
        {
            master.TrueKill();
        }
    }
}
