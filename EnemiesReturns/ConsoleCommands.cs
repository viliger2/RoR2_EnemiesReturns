using EnemiesReturns.Enemies.Colossus;
using EnemiesReturns.Enemies.MechanicalSpider.Enemy;
using EnemiesReturns.Enemies.Spitter;
using HG;
using RoR2;
using RoR2.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using Console = RoR2.Console;

[assembly: HG.Reflection.SearchableAttribute.OptInAttribute]
namespace EnemiesReturns
{
    public static class ConsoleCommands
    {
        [ConCommand(commandName = "returns_spawn_titans", flags = ConVarFlags.None, helpText = "Spawns all Titan variants")]
        private static void CCSpawnTitans(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Titan/cscTitanBlackBeach.asset").WaitForCompletion(), localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Titan/cscTitanDampCave.asset").WaitForCompletion(), localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Titan/cscTitanGolemPlains.asset").WaitForCompletion(), localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Titan/cscTitanGooLake.asset").WaitForCompletion(), localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Titan/cscTitanGold.asset").WaitForCompletion(), localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_spawn_spitters", flags = ConVarFlags.None, helpText = "Spawns all Spitter variants")]
        private static void CCSpawnSpitters(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(SpitterBody.SpawnCards.cscSpitterDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(SpitterBody.SpawnCards.cscSpitterLakes, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(SpitterBody.SpawnCards.cscSpitterDepths, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(SpitterBody.SpawnCards.cscSpitterSulfur, localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_spawn_colossi", flags = ConVarFlags.None, helpText = "Spawns all Colossus variants")]
        private static void CCSpawnColossi(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(ColossusBody.SpawnCards.cscColossusDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(ColossusBody.SpawnCards.cscColossusGrassy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(ColossusBody.SpawnCards.cscColossusSnowy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(ColossusBody.SpawnCards.cscColossusSandy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(ColossusBody.SpawnCards.cscColossusSkyMeadow, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(ColossusBody.SpawnCards.cscColossusCastle, localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_spawn_spiders", flags = ConVarFlags.None, helpText = "Spawns all Mechanical Spider variants")]
        private static void CCPocketSpiders(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderSnowy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(MechanicalSpiderEnemyBody.SpawnCards.cscMechanicalSpiderGrassy, localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_spawn_lynx", flags = ConVarFlags.None, helpText = "Spawns all Lynx Tribe enemies (including allies)")]
        private static void CCSpawnLynx(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(Enemies.LynxTribe.Archer.ArcherBody.SpawnCards.cscLynxArcherDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.LynxTribe.Archer.ArcherBodyAlly.SpawnCards.cscLynxArcherAlly, localPlayer.modelLocator.modelBaseTransform.position);

            SpawnMonster(Enemies.LynxTribe.Scout.ScoutBody.SpawnCards.cscLynxScoutDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.LynxTribe.Scout.ScoutBodyAlly.SpawnCards.cscLynxScoutAlly, localPlayer.modelLocator.modelBaseTransform.position);

            SpawnMonster(Enemies.LynxTribe.Hunter.HunterBody.SpawnCards.cscLynxHunterDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.LynxTribe.Hunter.HunterBodyAlly.SpawnCards.cscLynxHunterAlly, localPlayer.modelLocator.modelBaseTransform.position);

            SpawnMonster(Enemies.LynxTribe.Shaman.ShamanBody.SpawnCards.cscLynxShamanDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.LynxTribe.Shaman.ShamanBodyAlly.SpawnCards.cscLynxShamanAlly, localPlayer.modelLocator.modelBaseTransform.position);

            SpawnMonster(Enemies.LynxTribe.Totem.TotemBody.SpawnCards.cscLynxTotemDefault, localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_spawn_archerbugs", flags = ConVarFlags.None, helpText = "Spawns all Archer Bug variants")]
        private static void CCSpawnArcherBugs(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(Enemies.ArcherBug.ArcherBugBody.SpawnCards.cscArcherBugDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.ArcherBug.ArcherBugBody.SpawnCards.cscArcherBugJungle, localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_spawn_swifts", flags = ConVarFlags.None, helpText = "Spawns all Swift variants")]
        private static void CCSpawnSwifts(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(Enemies.Swift.SwiftBody.SpawnCards.cscSwiftDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Swift.SwiftBody.SpawnCards.cscSwiftRallypoint, localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_spawn_sand_crabs", flags = ConVarFlags.None, helpText = "Spawns all Sand Crab variants")]
        private static void CCSpawnCrabs(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(Enemies.SandCrab.SandCrabBody.SpawnCards.cscSandCrabDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.SandCrab.SandCrabBody.SpawnCards.cscSandCrabSandy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.SandCrab.SandCrabBody.SpawnCards.cscSandCrabGrassy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.SandCrab.SandCrabBody.SpawnCards.cscSandCrabSulfur, localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_spawn_temple_guardian", flags = ConVarFlags.None, helpText = "Spawns all Temple Guardian variants")]
        private static void CCSpawnTempleGuardian(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(ContentProvider.cscTempleGuardian, localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_outoftime_test", flags = ConVarFlags.None)]
        private static void CCOutOfTimeTest(ConCommandArgs args)
        {
            NetworkUser user = args.sender;
            InvokeCMD(user, "fixed_time", UnityEngine.Random.Range(4200, 5100).ToString());
            InvokeCMD(user, "run_set_stages_cleared", "11");
            InvokeCMD(user, "team_set_level", "1", UnityEngine.Random.Range(27, 31).ToString());

            InvokeCMD(user, "random_items", UnityEngine.Random.Range(78, 119).ToString(), "Tier1:100");
            InvokeCMD(user, "random_items", UnityEngine.Random.Range(43, 58).ToString(), "Tier2:100");
            InvokeCMD(user, "random_items", UnityEngine.Random.Range(8, 18).ToString(), "Tier3:100");
            InvokeCMD(user, "random_items", UnityEngine.Random.Range(3, 4).ToString(), "Boss:100");

            InvokeCMD(user, "give_equip", "MithrixHammer");
            InvokeCMD(user, "set_scene", "enemiesreturns_outoftime");
        }

        [ConCommand(commandName = "returns_contactlight_test", flags = ConVarFlags.None)]
        private static void CCContactLightTest(ConCommandArgs args)
        {
            NetworkUser user = args.sender;
            InvokeCMD(user, "fixed_time", UnityEngine.Random.Range(1500, 1800).ToString());
            InvokeCMD(user, "run_set_stages_cleared", "5");
            InvokeCMD(user, "team_set_level", "1", UnityEngine.Random.Range(15, 20).ToString());

            InvokeCMD(user, "random_items", UnityEngine.Random.Range(20, 30).ToString(), "Tier1:100");
            InvokeCMD(user, "random_items", UnityEngine.Random.Range(10, 15).ToString(), "Tier2:100");
            InvokeCMD(user, "random_items", UnityEngine.Random.Range(1, 4).ToString(), "Tier3:100");
            InvokeCMD(user, "random_items", UnityEngine.Random.Range(1, 3).ToString(), "Boss:100");

            //InvokeCMD(user, "give_equip", "random");
            InvokeCMD(user, "set_scene", "enemiesreturns_contactlight");
        }

        [ConCommand(commandName = "returns_take_damage", flags = ConVarFlags.None)]
        private static void CCTakeDamage(ConCommandArgs args)
        {
            NetworkUser user = args.sender;
            if (!Run.instance)
            {
                Debug.Log("Can't do this without Run!");
                return;
            }

            if (!NetworkServer.active)
            {
                Debug.Log("Only works on hosts!");
                return;
            }

            if (args.Count == 0)
            {
                Debug.Log("Missing arguments! arg0 - damage to take");
                return;
            }

            float damage = 0;
            if (!float.TryParse(args[0], out damage))
            {
                Debug.Log("Couldn't parse arg0! Should be float.");
                return;
            }

            if (args.senderBody && damage > 0)
            {
                DamageInfo damageInfo = new DamageInfo
                {
                    attacker = null,
                    damage = damage,
                    crit = false,
                    procCoefficient = 0f,
                    damageColorIndex = DamageColorIndex.Item,
                    damageType = DamageType.BypassArmor | DamageType.BypassBlock | DamageType.BypassOneShotProtection,
                    position = args.senderBody.transform.position
                };
                args.senderBody.healthComponent.TakeDamage(damageInfo);
            }
        }

        [ConCommand(commandName = "returns_spawn_boombox", flags = ConVarFlags.ExecuteOnServer, helpText = "Hit it Boy!")]
        private static void CCHitItBoy(ConCommandArgs args)
        {
            var position = args.senderBody.footPosition;

            var boombox = UnityEngine.Object.Instantiate(Content.Interactables.BoomBox, position, Quaternion.LookRotation(args.senderBody.characterDirection.forward));
            NetworkServer.Spawn(boombox);
        }

        [ConCommand(commandName = "returns_draw_nodegraph", flags = ConVarFlags.None, helpText = "Draws nodegraph with only enabled gates. Updates in real time as you enable/disable gates. Same parameters debug_scene_draw_nodegraph. as Hits performance hard compared to vanilla method since this one doesn't memorize the output depending on input, redrawing the entire thing every frame..")]
        private static void CCDrawNodeGraphWithGates(ConCommandArgs args)
        {
            bool shouldDraw = args.GetArgBool(0);
            MapNodeGroup.GraphType graphType = args.GetArgEnum<MapNodeGroup.GraphType>(1);
            HullMask hullMask = (HullMask)(1 << (int)args.GetArgEnum<HullClassification>(2));
            if (hullMask == HullMask.None)
            {
                throw new ConCommandException("Cannot use HullMask.None.");
            }
            for (int i = 3; i < args.Count; i++)
            {
                HullClassification? hullClassification = args.TryGetArgEnum<HullClassification>(i);
                if (hullClassification.HasValue)
                {
                    hullMask = (HullMask)((int)hullMask | (1 << (int)hullClassification.Value));
                }
            }

            (MapNodeGroup.GraphType, HullMask) key = (graphType, hullMask);
            DebugOverlay.MeshDrawer drawer;
            if (shouldDraw)
            {
                if (RoR2.SceneInfo.NodeGraphOverlay.drawers == null)
                {
                    RoR2.SceneInfo.NodeGraphOverlay.drawers = new Dictionary<(MapNodeGroup.GraphType, HullMask), (DebugOverlay.MeshDrawer, Action)>();
                    RoR2Application.onUpdate += RoR2.SceneInfo.NodeGraphOverlay.StaticUpdate;
                }
                if (!RoR2.SceneInfo.NodeGraphOverlay.drawers.ContainsKey(key))
                {
                    drawer = DebugOverlay.GetMeshDrawer();
                    drawer.hasMeshOwnership = true;
                    drawer.material = DebugOverlay.defaultWireMaterial;

                    RoR2.SceneInfo.NodeGraphOverlay.drawers.Add(key, (drawer, Updater));
                }
            }
            else if (RoR2.SceneInfo.NodeGraphOverlay.drawers != null)
            {
                if (RoR2.SceneInfo.NodeGraphOverlay.drawers.TryGetValue(key, out var value))
                {
                    value.Item1.Dispose();
                    RoR2.SceneInfo.NodeGraphOverlay.drawers.Remove(key);
                }
                if (RoR2.SceneInfo.NodeGraphOverlay.drawers.Count == 0)
                {
                    RoR2.SceneInfo.NodeGraphOverlay.drawers = null;
                    RoR2Application.onUpdate -= RoR2.SceneInfo.NodeGraphOverlay.StaticUpdate;
                }
            }

            void Updater()
            {
                drawer.mesh = RoR2.SceneInfo.instance.GetNodeGraph(graphType).GenerateLinkDebugMeshOnlyActiveGates(hullMask);
            }
        }

        public static void InvokeCMD(NetworkUser user, string commandName, params string[] arguments)
        {
            var args = arguments.ToList();
            var consoleUser = new Console.CmdSender(user);
            if (Console.instance)
                Console.instance.RunCmd(consoleUser, commandName, args);
            else
                Log.Message("InvokeCMD called whilst no console instance exists");
        }

        private static void SpawnMonster(CharacterSpawnCard card, Vector3 position)
        {
            var spawnRequest = new DirectorSpawnRequest(
                card,
                new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                    position = position,
                    minDistance = 0f,
                    maxDistance = 30f
                },
                RoR2Application.rng
                );
            spawnRequest.teamIndexOverride = TeamIndex.Monster;
            spawnRequest.ignoreTeamMemberLimit = true;

            DirectorCore.instance.TrySpawnObject(spawnRequest);
        }
    }
}
