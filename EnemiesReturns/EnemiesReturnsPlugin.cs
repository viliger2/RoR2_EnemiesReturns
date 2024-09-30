using BepInEx;
using BepInEx.Configuration;
using EnemiesReturns.Enemies.Colossus;
using EnemiesReturns.Enemies.Ifrit;
using EnemiesReturns.Enemies.Spitter;
using EnemiesReturns.Items.ColossalKnurl;
using EnemiesReturns.Items.SpawnPillarOnChampionKill;
using RoR2;
using RoR2.ContentManagement;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

[assembly: HG.Reflection.SearchableAttribute.OptInAttribute]
namespace EnemiesReturns
{
    [BepInPlugin(GUID, ModName, Version)]
    [BepInDependency(R2API.PrefabAPI.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Viliger.RandyBobandyBrokeMyGamandy", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(R2API.DeployableAPI.PluginGUID)]
    [BepInDependency(R2API.DirectorAPI.PluginGUID)]
    public class EnemiesReturnsPlugin : BaseUnityPlugin
    {
        public const string Author = "Viliger";
        public const string ModName = "EnemiesReturns";
        public const string Version = "0.2.3";
        public const string GUID = "com." + Author + "." + ModName;

        private void Awake()
        {
#if DEBUG == true
            On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
#endif
            var UseConfigFile = Config.Bind<bool>("Config", "Use Config File", false, "Use config file for storring config. Due to mod being currently unfinished and unbalanced, we deploy rapid changes to values. So this way we can still have configs, but without the issue of people having those values saved.");

            Log.Init(Logger);

            if (UseConfigFile.Value)
            {
                EnemiesReturnsConfiguration.PopulateConfig(Config);
            }
            else
            {
                var spitterConfigFile = new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, "Config"), false)
                {
                    SaveOnConfigSet = false,
                };

                EnemiesReturnsConfiguration.PopulateConfig(spitterConfigFile);
            }

            Hooks();
        }

        private void Hooks()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
            RoR2.Language.collectLanguageRootFolders += CollectLanguageRootFolders;
            RoR2.Language.onCurrentLanguageChanged += Language.Language_onCurrentLanguageChanged;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;
            ColossalKnurlFactory.Hooks();
            IfritFactory.Hooks();
            SpawnPillarOnChampionKillFactory.Hooks();
            // using single R2API recalcstats hook for the sake of performance
            //R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport obj)
        {
            var damageInfo = obj.damageInfo;
            var victim = obj.victim.gameObject;

            if (!damageInfo.attacker || !damageInfo.attacker.TryGetComponent<CharacterBody>(out var attackerBody))
            {
                return;
            }

            if (!attackerBody.master)
            {
                return;
            }

            ColossalKnurlFactory.OnHitEnemy(damageInfo, attackerBody, victim);
        }

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

            SpawnMonster(Enemies.Spitter.SpitterFactory.SpawnCards.cscSpitterDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Spitter.SpitterFactory.SpawnCards.cscSpitterLakes, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Spitter.SpitterFactory.SpawnCards.cscSpitterDepths, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Spitter.SpitterFactory.SpawnCards.cscSpitterSulfur, localPlayer.modelLocator.modelBaseTransform.position);
        }

        [ConCommand(commandName = "returns_spawn_colossi", flags = ConVarFlags.None, helpText = "Spawns all Colossus variants")]
        private static void CCSpawnColossi(ConCommandArgs args)
        {
            var localPlayers = LocalUserManager.readOnlyLocalUsersList;
            var localPlayer = localPlayers[0].cachedBody;

            SpawnMonster(Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusDefault, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusGrassy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusSnowy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusSandy, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusSkyMeadow, localPlayer.modelLocator.modelBaseTransform.position);
            SpawnMonster(Enemies.Colossus.ColossusFactory.SpawnCards.cscColossusCastle, localPlayer.modelLocator.modelBaseTransform.position);
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

        [ConCommand(commandName = "returns_body_generate_portraits", flags = ConVarFlags.None, helpText = "Generates portraits for all EnemiesReturns bodies.")]
        private static void CCBodyGeneratePortraits(ConCommandArgs args)
        {
            var addressable = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/IconGenerator.prefab").WaitForCompletion();
            if (addressable)
            {
                var modelPanel = addressable.GetComponentInChildren<ModelPanel>();
                if (modelPanel)
                {
                    modelPanel.modelPostProcessVolumePrefab = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Cube);
                }
            }

            RoR2Application.instance.StartCoroutine(GeneratePortraits(args.TryGetArgBool(0) ?? false));
        }

        private static IEnumerator GeneratePortraits(bool forceRegeneration)
        {
            Debug.Log("Starting portrait generation.");
            ModelPanel modelPanel = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/UI/IconGenerator")).GetComponentInChildren<ModelPanel>();
            modelPanel.renderSettings = new RenderSettingsState
            {
                ambientIntensity = 1f,
                ambientLight = Color.white,
                ambientMode = AmbientMode.Flat,
                ambientGroundColor = Color.white
            };
            yield return new WaitForEndOfFrame();
            modelPanel.BuildRenderTexture();
            yield return new WaitForEndOfFrame();
            yield return GeneratePortrait(modelPanel, SpitterFactory.SpitterBody);
            yield return GeneratePortrait(modelPanel, ColossusFactory.ColossusBody);
            yield return GeneratePortrait(modelPanel, IfritFactory.IfritBody);
            yield return GeneratePortrait(modelPanel, IfritPillarFactory.Enemy.IfritPillarBody);
            yield return GeneratePortrait(modelPanel, Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBody.prefab").WaitForCompletion());
            UnityEngine.Object.Destroy(modelPanel.transform.root.gameObject);
            Debug.Log("Portrait generation complete.");
        }

        private static IEnumerator GeneratePortrait(ModelPanel modelPanel, GameObject gameObject)
        {
            CharacterBody characterBody = gameObject.GetComponent<CharacterBody>();
            if ((bool)characterBody)
            {
                float num = 1f;
                try
                {
                    Debug.LogFormat("Generating portrait for {0}", gameObject.name);
                    modelPanel.modelPrefab = gameObject.GetComponent<ModelLocator>()?.modelTransform.gameObject;
                    //modelPanel.modelPostProcessVolumePrefab = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Cube);
                    modelPanel.SetAnglesForCharacterThumbnail(setZoom: true);
                    PrintController printController;
                    if ((object)(printController = modelPanel.modelPrefab?.GetComponentInChildren<PrintController>()) != null)
                    {
                        num = Mathf.Max(num, printController.printTime + 1f);
                    }
                    TemporaryOverlay temporaryOverlay;
                    if ((object)(temporaryOverlay = modelPanel.modelPrefab?.GetComponentInChildren<TemporaryOverlay>()) != null)
                    {
                        num = Mathf.Max(num, temporaryOverlay.duration + 1f);
                    }
                    var particles = modelPanel.modelPrefab.GetComponentsInChildren<ParticleSystem>();
                    foreach (var particle in particles)
                    {
                        UnityEngine.Object.Destroy(particle.gameObject);
                    }
                    var lights = modelPanel.modelPrefab.GetComponentsInChildren<Light>();
                    foreach (var light in lights)
                    {
                        UnityEngine.Object.Destroy(light.gameObject);
                    }

                }
                catch (Exception message)
                {
                    Debug.Log(message);
                }
                RoR2Application.onLateUpdate += UpdateCamera;
                yield return new WaitForSeconds(num);
                modelPanel.SetAnglesForCharacterThumbnail(setZoom: true);
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                try
                {
                    Texture2D texture2D = new Texture2D(modelPanel.renderTexture.width, modelPanel.renderTexture.height, TextureFormat.ARGB32, mipChain: false, linear: false);
                    RenderTexture active = RenderTexture.active;
                    RenderTexture.active = modelPanel.renderTexture;
                    texture2D.ReadPixels(new Rect(0f, 0f, modelPanel.renderTexture.width, modelPanel.renderTexture.height), 0, 0, recalculateMipMaps: false);
                    RenderTexture.active = active;
                    byte[] array = texture2D.EncodeToPNG();
                    FileStream fileStream = new FileStream("Assets/RoR2/GeneratedPortraits/" + gameObject.name + ".png", FileMode.Create, FileAccess.Write);
                    fileStream.Write(array, 0, array.Length);
                    fileStream.Close();
                }
                catch (Exception message2)
                {
                    Debug.Log(message2);
                }
                RoR2Application.onLateUpdate -= UpdateCamera;
                yield return new WaitForEndOfFrame();
            }
            void UpdateCamera()
            {
                modelPanel.SetAnglesForCharacterThumbnail(setZoom: true);
            }
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new ContentProvider());
        }

        private void CollectLanguageRootFolders(List<string> folders)
        {
            folders.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(base.Info.Location), Language.LanguageFolder));
        }
    }
}
