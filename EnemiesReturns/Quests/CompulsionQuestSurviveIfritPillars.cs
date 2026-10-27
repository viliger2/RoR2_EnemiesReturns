using RoR2;
using RoR2.ContentManagement;
using RoR2.Quests;
using RoR2.Quests.CompulsionQuests;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using HG;
using EnemiesReturns.Components;

namespace EnemiesReturns.Quests
{
    [RegisterQuest(IDENTIFIER, canStartMethodName = "CanStart", questWeight = 1f)] // you can't set weight to a value here
    public class CompulsionQuestSurviveIfritPillars : BaseCompulsionQuest
    {
        private class ObjectiveTracker : BaseQuestObjectiveTracker
        {
            public override string GenerateString()
            {
                return RoR2.Language.GetString("ENEMIES_RETURNS_QUEST_PILLARS_SURVIVE");
            }
        }

        public const string IDENTIFIER = "ER_CompulsionQuestSurviveIfritPillars";

        public const int RETRY_COUNT = 10;

        private static float minSpawnDistance => Configuration.Ifrit.QuestPillarSpawnDistanceMin.Value;

        private static float maxSpawnDistance => Configuration.Ifrit.QuestPillarSpawnDistanceMax.Value;

        private static CharacterSpawnCard cscIfritPillar => EnemiesReturns.Enemies.Ifrit.Pillar.PillarEnemyBody.SpawnCard;

        private static int pillarCount => Configuration.Ifrit.QuestPillarCount.Value;

        private AssetReferenceGameObject _combatSquad = new AssetReferenceGameObject(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC4_ShrineCompulsion.CompulsionCombatSquad_prefab);

        private AsyncOperationHandle<GameObject> _combatSquadHandle;

        private bool assetHandleLoaded;

        private CombatSquad combatSquad;

        private static bool CanStart(QuestRunner runner)
        {
            return !BaseCompulsionQuest.IsInstanceActive(IDENTIFIER) && Configuration.General.EnableIfrit.Value && Configuration.Ifrit.QuestPillarEnable.Value;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                _combatSquadHandle = AssetAsyncReferenceManager<GameObject>.LoadAsset(_combatSquad, AsyncReferenceHandleUnloadType.OnSceneUnload);
            }
            Util.PlaySound(CompulsionQuestKillSuperTarget.summonGreatSoundString, RoR2Application.instance.gameObject);
        }

        public override void OnExit()
        {
            if (assetHandleLoaded)
            {
                AssetAsyncReferenceManager<GameObject>.UnloadAsset(_combatSquad);
                _combatSquadHandle = default(AsyncOperationHandle<GameObject>);
            }
            base.OnExit();
        }

        public override void FixedUpdate(float fixedDeltaTime)
        {
            base.FixedUpdate(fixedDeltaTime);

            if (NetworkServer.active)
            {
                if (combatSquad)
                {
                    if (combatSquad.memberCount == 0)
                    {
                        EndQuest(true);
                        return;
                    }
                }
                if (!assetHandleLoaded && _combatSquadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    assetHandleLoaded = true;
                    SpawnPillars(_combatSquadHandle);
                }
            }

        }

        private void SpawnPillars(AsyncOperationHandle<GameObject> combatSquadHandle)
        {
            if (!combatSquadHandle.Result)
            {
                return;
            }

            CharacterBody targetBody;

            GameObject combatSquadGameObject = Instantiate(combatSquadHandle.Result);
            var tracker = runner.gameObject.GetComponent<CompulsionQuestTracker>();
            if (tracker)
            {
                targetBody = tracker.questOwnerCharacterBody;
            } else
            {
                List<CharacterBody> characterBodiesFromParticipatingTeam = GetCharacterBodiesFromParticipatingTeam(playerOnly: true);
                targetBody = runner.questRNG.NextElementUniform(characterBodiesFromParticipatingTeam);
            }

            if (!targetBody)
            {
                return;
            }

            if(!combatSquadGameObject.TryGetComponent<CombatSquad>(out combatSquad))
            {
                return;
            }

            var step = 360f / pillarCount;
            for (int i = 0; i < pillarCount; i++)
            {
                float angle = step * i;
                Vector3 direction = Quaternion.Euler(0f, angle, 0f) * targetBody.transform.forward;

                if(SpawnOnePillar(targetBody, direction, out var master))
                {
                    combatSquad.AddMember(master);
                }
            }

            if(combatSquad.memberCount <= 0)
            {
                AbortCurrentQuestServer(QuestCatalog.FindQuestIndex(IDENTIFIER));
                return;
            }
            NetworkServer.Spawn(combatSquadGameObject);
            combatSquad.onDefeatedServer += Component_onDefeatedServer;
            MarkDirtyServer();
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            if (combatSquad)
            {
                writer.Write(combatSquad.memberCount);
                if (combatSquad.memberCount > 0)
                {
                    for (int i = 0; i < combatSquad.memberCount; i++)
                    {
                        var master = combatSquad.readOnlyMembersList[i];
                        if (master)
                        {
                            var body = master.GetBody();
                            if (body)
                            {
                                writer.Write(body.netId);
                            }
                        }
                        else
                        {
                            writer.Write(NetworkInstanceId.Invalid);
                        }
                    }
                }
            } else
            {
                writer.Write(0);
            }
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var id = reader.ReadNetworkId();
                var bodyObject = Util.FindNetworkObject(id);
                if (bodyObject)
                {
                    bodyObject.GetComponent<CharacterBody>().subtitleNameToken = "ENEMIES_RETURNS_QUEST_PILLARS_SUBTITLE";
                }
            }
        }

        private void Component_onDefeatedServer()
        {
            EndQuest(true);
        }

        private bool SpawnOnePillar(CharacterBody targetBody, Vector3 direction, out CharacterMaster monsterMaster)
        {
            monsterMaster = null;
            var position = targetBody.corePosition + direction * minSpawnDistance;
            var request = new DirectorSpawnRequest(cscIfritPillar, new DirectorPlacementRule()
            {
                minDistance = 0f,
                maxDistance = Mathf.Min(0, maxSpawnDistance - minSpawnDistance),
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                position = position
            }, runner.questRNG);

            request.teamIndexOverride = (targetBody.AsValidOrNull() != null && targetBody.teamComponent.teamIndex == TeamIndex.Player) ? TeamIndex.Monster : TeamIndex.Neutral;
            request.ignoreTeamMemberLimit = true;
            GameObject gameObject = null;
            int num = 0;
            while(!gameObject && num < RETRY_COUNT)
            {
                num++;
                gameObject = DirectorCore.instance.TrySpawnObject(request);
                if (gameObject == null)
                {
                    request.placementRule.maxDistance += 10f;
                    continue;
                }
                monsterMaster = gameObject.GetComponent<CharacterMaster>();
                SetSubtitleToken(monsterMaster);
                return true;
            }
            return false;
        }

        private static void SetSubtitleToken(CharacterMaster monsterMaster)
        {
            if (monsterMaster && monsterMaster.GetBody() != null)
            {
                monsterMaster.GetBody().subtitleNameToken = "ENEMIES_RETURNS_QUEST_PILLARS_SUBTITLE";
            }
        }

        public override Type GetObjectiveType()
        {
            return typeof(ObjectiveTracker);
        }

    }
}
