using EnemiesReturns.Enemies.Ifrit;
using EnemiesReturns.Enemies.Ifrit.Pillar;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using static RoR2.CharacterBody;

namespace EnemiesReturns.Items.SpawnPillarOnChampionKill
{
    public class PillarItemBehavior : ItemBehavior, IOnKilledOtherServerReceiver
    {
        public void OnKilledOtherServer(DamageReport damageReport)
        {
            if (!damageReport.damageInfo.procChainMask.HasModdedProc(IfritStuff.PillarExplosion))
            {
                if (base.body.master.IsDeployableLimited(IfritStuff.PylonDeployable))
                {
                    return;
                }
                bool spawn = false;
                if (damageReport.victimBody)
                {
                    spawn = damageReport.victimBody.isChampion || damageReport.victimBody.isPlayerControlled;
                    if (!spawn && damageReport.victimBody.isElite)
                    {
                        spawn = Util.CheckRoll(EnemiesReturns.Configuration.Ifrit.SpawnPillarOnChampionKillEliteChance.Value, damageReport.attackerMaster);
                    }
                }
                if (spawn)
                {
                    DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(PillarAllyBody.SpawnCard, new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                        position = damageReport.victimBody.transform.position
                    }, RoR2Application.rng);
                    directorSpawnRequest.summonerBodyObject = base.gameObject;
                    directorSpawnRequest.ignoreTeamMemberLimit = true;
                    directorSpawnRequest.onSpawnedServer = (SpawnCard.SpawnResult spawnResult) =>
                    {
                        if (spawnResult.success && spawnResult.spawnedInstance)
                        {
                            var aiownership = spawnResult.spawnedInstance.GetComponent<AIOwnership>();
                            if (aiownership)
                            {
                                aiownership.ownerMaster = this.body.master;
                            }

                            if (spawnResult.spawnedInstance.TryGetComponent<CharacterMaster>(out var deployableMaster))
                            {
                                var deployable = deployableMaster.GetComponent<Deployable>();
                                if (deployable)
                                {
                                    //deployable.onUndeploy.AddListener(deployableMaster.TrueKill);
                                    base.body.master.AddDeployable(deployable, IfritStuff.PylonDeployable);
                                }
                            }
                        }
                    };
                    DirectorCore.instance?.TrySpawnObject(directorSpawnRequest);
                }
            }
        }
    }
}
