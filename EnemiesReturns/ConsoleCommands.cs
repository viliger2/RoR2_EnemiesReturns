using EnemiesReturns.Enemies.Colossus;
using EnemiesReturns.Enemies.MechanicalSpider.Enemy;
using EnemiesReturns.Enemies.Spitter;
using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
                    placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                    position = position
                },
                RoR2Application.rng
                );
            spawnRequest.teamIndexOverride = TeamIndex.Monster;
            spawnRequest.ignoreTeamMemberLimit = true;

            DirectorCore.instance.TrySpawnObject(spawnRequest);
        }
    }
}
