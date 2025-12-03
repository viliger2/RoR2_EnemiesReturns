using RoR2;
using RoR2.CharacterAI;
using System.Collections.Generic;
using UnityEngine;
using static RoR2.CharacterBody;

namespace EnemiesReturns.Items.LynxFetish
{
    public class LynxFetishItemBehavior : ItemBehavior
    {
        private DirectorPlacementRule placementRule;

        private float spawnTimer;

        private void Awake()
        {
            placementRule = new DirectorPlacementRule()
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                minDistance = 3f,
                maxDistance = 40f,
                spawnOnTarget = base.transform
            };
            spawnTimer = 15f;
        }

        private void Start()
        {
            TrySummonTribesmen();
        }

        private void FixedUpdate()
        {
            spawnTimer += Time.fixedDeltaTime;
            TrySummonTribesmen();
        }

        private void TrySummonTribesmen()
        {
            if (body && body.master)
            {
                if (!body.master.IsDeployableLimited(Content.Deployables.LynxFetishDeployable) && spawnTimer >= 15f)
                {
                    if (!Configuration.LynxTribe.LynxTotem.LynxFetishSpawnInBazaar.Value)
                    {
                        if (RoR2.Stage.instance.sceneDef.cachedName == "bazaar")
                        {
                            spawnTimer = 0f;
                            return;
                        }
                    }

                    var nextSpawnCard = GetNextSpawnCard();
                    if (nextSpawnCard == null)
                    {
                        spawnTimer = 0f;
                        return;
                    }
                    DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(nextSpawnCard, placementRule, RoR2Application.rng);
                    directorSpawnRequest.summonerBodyObject = base.gameObject;
                    directorSpawnRequest.onSpawnedServer = OnMasterSpawned;
                    directorSpawnRequest.ignoreTeamMemberLimit = true;
                    DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                    if (body.master.GetDeployableCount(Content.Deployables.LynxFetishDeployable) < body.master.GetDeployableSameSlotLimit(Content.Deployables.LynxFetishDeployable))
                    {
                        spawnTimer = 14f;
                    }
                    else
                    {
                        spawnTimer = 0f;
                    }
                }
            }
        }

        private void OnMasterSpawned(SpawnCard.SpawnResult spawnResult)
        {
            if (!spawnResult.success)
            {
                return;
            }

            var summonedMaster = spawnResult.spawnedInstance.GetComponent<CharacterMaster>();
            if (!summonedMaster)
            {
                return;
            }

            var bodyObject = summonedMaster.GetBodyObject();
            if (bodyObject)
            {
                SetTeamFilter(bodyObject);
            }
            else
            {
                summonedMaster.onBodyStart += SummonedMaster_onBodyStart;
            }

            summonedMaster.inventory.GiveItemPermanent(RoR2Content.Items.BoostDamage, EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBonusDamage.Value + EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBonusDamagePerStack.Value * Mathf.Max(0, stack - 4));
            summonedMaster.inventory.GiveItemPermanent(RoR2Content.Items.BoostHp, EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBonusHP.Value + EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBonusHPPerStack.Value * Mathf.Max(0, stack - 4));
            if (ModCompats.RiskyModCompat.enabled)
            {
                summonedMaster.inventory.GiveItemPermanent(ModCompats.RiskyModCompat.RiskyModAllyMarker, 1);
                summonedMaster.inventory.GiveItemPermanent(ModCompats.RiskyModCompat.RiskyModAllyScaling, 1);
                summonedMaster.inventory.GiveItemPermanent(ModCompats.RiskyModCompat.RiskyModAllyRegen, 40);
                summonedMaster.inventory.GiveItemPermanent(ModCompats.RiskyModCompat.RiskyModAllyAllowOverheatDeath, 1);
                summonedMaster.inventory.GiveItemPermanent(ModCompats.RiskyModCompat.RiskyModAllyAllowVoidDeath, 1);
            }

            var aiownership = spawnResult.spawnedInstance.GetComponent<AIOwnership>();
            if (aiownership)
            {
                aiownership.ownerMaster = this.body.master;
            }

            var deployable = spawnResult.spawnedInstance.GetComponent<Deployable>();
            if (deployable)
            {
                deployable.onUndeploy.AddListener(summonedMaster.TrueKill);
                body.master.AddDeployable(deployable, Content.Deployables.LynxFetishDeployable);
            }
        }

        private void SummonedMaster_onBodyStart(CharacterBody body)
        {
            SetTeamFilter(body.gameObject);
            body.master.onBodyStart -= SummonedMaster_onBodyStart;
        }

        private void SetTeamFilter(GameObject bodyObject)
        {
            var teamFilter = bodyObject.GetComponent<TeamFilter>();
            if (teamFilter)
            {
                teamFilter.teamIndex = this.body.teamComponent.teamIndex;
            }
        }

        private CharacterSpawnCard GetNextSpawnCard()
        {
            // step 1. establishing a list of already deployed tribesmen
            List<BodyIndex> deployedList = new List<BodyIndex>();
            if (body.master.deployablesList != null)
            {
                for (int i = body.master.deployablesList.Count - 1; i >= 0; i--)
                {
                    if (body.master.deployablesList[i].slot == Content.Deployables.LynxFetishDeployable)
                    {
                        var deployable = this.body.master.deployablesList[i].deployable;
                        if (!deployable)
                        {
                            continue;
                        }
                        var master = deployable.gameObject.GetComponent<CharacterMaster>();
                        if (!master)
                        {
                            continue;
                        }

                        var bodyDeployable = master.GetBody();
                        if (!bodyDeployable)
                        {
                            continue;
                        }

                        deployedList.Add(bodyDeployable.bodyIndex);
                    }
                }
            }

            // step 2. iterating over IndexToCards array, so we can figure out what is not spawned yet
            List<CharacterSpawnCard> list = new List<CharacterSpawnCard>();
            for (int i = 0; i < LynxFetishFactory.spawnCards.Length; i++)
            {
                if (!deployedList.Contains(LynxFetishFactory.spawnCards[i].bodyIndex))
                {
                    list.Add(LynxFetishFactory.spawnCards[i].spawnCard);
                }
            }

            if (list.Count == 0)
            {
                return null;
            }

            // step 3. returning random spawn card
            return list[RoR2Application.rng.RangeInt(0, list.Count)];
        }

    }
}
