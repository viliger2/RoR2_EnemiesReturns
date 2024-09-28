using RoR2;

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
