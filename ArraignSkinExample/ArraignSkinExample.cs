using BepInEx;
using EnemiesReturns.Components;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

[assembly: HG.Reflection.SearchableAttribute.OptInAttribute]
namespace ArraignSkinExample
{
    // This is an example plugin for adding skins as a reward for killing Arraign and surviving Judgement
    // example is rather messy, but it should serve you as a basic example of how to add unlock condition for a skin related to Arraign
    // The plugin adds skin as a normal skin with no unlock conditions if EnemiesReturns is not found
    // The skin added is ClassicBanditSkin by FORCED_REASSEMBLY,
    // asset bundle does not come with this project, you can grab it from here (its actually embded into dll because that's how SkinBuilder works, oops)
    // https://thunderstore.io/package/Forced_Reassembly/ClassicBanditSkin/
    // also it uses ContentAddition because I got lazy
    [BepInPlugin(GUID, ModName, Version)]
    [BepInDependency(R2API.ContentManagement.R2APIContentManager.PluginGUID)]
    [BepInDependency(EnemiesReturns.EnemiesReturnsPlugin.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class ArraignSkinExample : BaseUnityPlugin
    {
        public const string Author = "Viliger";
        public const string ModName = "ArraignExamplePlugin";
        public const string Version = "1.0.0";
        public const string GUID = "com." + Author + "." + ModName;

        public const string BodyName = "Bandit2Body";
        public const string AssetBundleName = "ClassicBanditSkin.bundle";

        private static AssetBundle bundle;

        private static UnlockableDef unlockable;

        private static readonly List<Material> materialsWithRoRShader = new List<Material>();

        public void Awake()
        {
            Log.Init(Logger);

            bundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), "ClassicBanditSkin.bundle"));
            LoadMaterialsWithReplacedShader("RoR2/Base/Shaders/HGStandard.shader", "Assets/ClassicBanditSkin/Resources/BanditCoatMaterial.mat", "Assets/ClassicBanditSkin/Resources/BanditPistolMaterial.mat");

            if (EnemiesReturnsModCompat.enabled)
            {
                // We add Bandit2Body to blacklist so EnemiesReturns won't generate automatic Anointed skin
                EnemiesReturnsModCompat.AddBodyToBlacklist(BodyName);
                // We create unlockable that is base for our achievement
                unlockable = EnemiesReturnsModCompat.CreateUnlockable(BodyName, bundle.LoadAsset<Sprite>("Assets\\SkinMods\\ClassicBanditSkin\\Icons\\ClassicBanditSkinDefIcon.png"));
                // We register achievement manually to keep EnemiesReturns as soft dependency,
                // if you are fine with EnemiesReturns being hard dependency, then go to Bandit2JudgementClearedAchievement and read the comment
                RoR2.AchievementManager.onAchievementsRegistered += CreateAchievement;
                // We add unlockable to catalog via R2API
                R2API.ContentAddition.AddUnlockableDef(unlockable);
            }
        }

        private void CreateAchievement()
        {
            EnemiesReturnsModCompat.CreateAchievement(unlockable, BodyName);
        }

        [SystemInitializer(new Type[] { typeof(BodyCatalog) })]
        public static void Init()
        {
            if (!EnemiesReturns.EnemiesReturnsPlugin.ModIsLoaded)
            {
                return;
            }
            AddArraignSkinExampleBandit2Skin();
        }

        // this entire thing is pretty much copy pasted from SkinBuilder
        private static void AddArraignSkinExampleBandit2Skin()
        {
            string skinDefName = "ArraignSkinExampleBandit2SkinDef";
            try
            {
                GameObject gameObject = BodyCatalog.FindBodyPrefab(BodyName);
                if (!gameObject)
                {
                    Log.Warning("Failed to add \"" + skinDefName + "\" skin because \"" + BodyName + "\" doesn't exist");
                    return;
                }
                ModelLocator component = gameObject.GetComponent<ModelLocator>();
                if (!component)
                {
                    Log.Warning(("Failed to add \"" + skinDefName + "\" skin to \"" + BodyName + "\" because it doesn't have \"ModelLocator\" component"));
                    return;
                }
                GameObject gameObject2 = component.modelTransform.gameObject;
                ModelSkinController skinController = (gameObject2 ? gameObject2.GetComponent<ModelSkinController>() : null);
                if (!skinController)
                {
                    Log.Warning(("Failed to add \"" + skinDefName + "\" skin to \"" + BodyName + "\" because it doesn't have \"ModelSkinController\" component"));
                    return;
                }
                Renderer[] renderers = gameObject2.GetComponentsInChildren<Renderer>(includeInactive: true);
                SkinDef skin;
                if (EnemiesReturnsModCompat.enabled)
                {
                    skin = EnemiesReturnsModCompat.CreateHiddenSkinDef();
                }
                else
                {
                    skin = ScriptableObject.CreateInstance<SkinDef>();
                }
                TryCatchThrow("Icon", delegate
                {
                    skin.icon = bundle.LoadAsset<Sprite>("Assets\\SkinMods\\ClassicBanditSkin\\Icons\\ClassicBanditSkinDefIcon.png");
                });
                skin.name = skinDefName;
                skin.nameToken = "ARRAIGN_SKIN_EXAMPLE_BANDIT2_SKIN_NAME";
                skin.rootObject = gameObject2;
                TryCatchThrow("Base Skins", delegate
                {
                    skin.baseSkins = new SkinDef[1] { skinController.skins[0] };
                });
                TryCatchThrow("Unlockable Name", delegate
                {
                    if (EnemiesReturnsModCompat.enabled && unlockable)
                    {
                        skin.unlockableDef = unlockable;
                    }
                });
                var skinDefParams = ScriptableObject.CreateInstance<SkinDefParams>();
                skinDefParams.name = skinDefName + "SkinDefParams";
                TryCatchThrow("Game Object Activations", delegate
                {
                    skinDefParams.gameObjectActivations = Array.Empty<SkinDefParams.GameObjectActivation>();
                });
                TryCatchThrow("Renderer Infos", delegate
                {
                    skinDefParams.rendererInfos = new CharacterModel.RendererInfo[4]
                    {
                    new CharacterModel.RendererInfo
                    {
                        defaultMaterial = bundle.LoadAsset<Material>("Assets/ClassicBanditSkin/Resources/BanditCoatMaterial.mat"),
                        defaultShadowCastingMode = ShadowCastingMode.On,
                        ignoreOverlays = false,
                        renderer = renderers[2]
                    },
                    new CharacterModel.RendererInfo
                    {
                        defaultMaterial = bundle.LoadAsset<Material>("Assets/ClassicBanditSkin/Resources/BanditCoatMaterial.mat"),
                        defaultShadowCastingMode = ShadowCastingMode.On,
                        ignoreOverlays = false,
                        renderer = renderers[3]
                    },
                    new CharacterModel.RendererInfo
                    {
                        defaultMaterial = bundle.LoadAsset<Material>("Assets/ClassicBanditSkin/Resources/BanditCoatMaterial.mat"),
                        defaultShadowCastingMode = ShadowCastingMode.On,
                        ignoreOverlays = false,
                        renderer = renderers[4]
                    },
                    new CharacterModel.RendererInfo
                    {
                        defaultMaterial = bundle.LoadAsset<Material>("Assets/ClassicBanditSkin/Resources/BanditPistolMaterial.mat"),
                        defaultShadowCastingMode = ShadowCastingMode.On,
                        ignoreOverlays = false,
                        renderer = renderers[7]
                    }
                    };
                });
                TryCatchThrow("Mesh Replacements", delegate
                {
                    skinDefParams.meshReplacements = new SkinDefParams.MeshReplacement[7]
                    {
                    new SkinDefParams.MeshReplacement
                    {
                        mesh = null,
                        renderer = renderers[0]
                    },
                    new SkinDefParams.MeshReplacement
                    {
                        mesh = null,
                        renderer = renderers[1]
                    },
                    new SkinDefParams.MeshReplacement
                    {
                        mesh = bundle.LoadAsset<Mesh>("Assets\\SkinMods\\ClassicBanditSkin\\Meshes\\BanditMesh.mesh"),
                        renderer = renderers[2]
                    },
                    new SkinDefParams.MeshReplacement
                    {
                        mesh = bundle.LoadAsset<Mesh>("Assets\\SkinMods\\ClassicBanditSkin\\Meshes\\BanditCoatMesh.mesh"),
                        renderer = renderers[3]
                    },
                    new SkinDefParams.MeshReplacement
                    {
                        mesh = null,
                        renderer = renderers[6]
                    },
                    new SkinDefParams.MeshReplacement
                    {
                        mesh = bundle.LoadAsset<Mesh>("Assets\\SkinMods\\ClassicBanditSkin\\Meshes\\BanditShotgunMesh.001.mesh"),
                        renderer = renderers[4]
                    },
                    new SkinDefParams.MeshReplacement
                    {
                        mesh = bundle.LoadAsset<Mesh>("Assets\\SkinMods\\ClassicBanditSkin\\Meshes\\ClassicPistolMesh.mesh"),
                        renderer = renderers[7]
                    }
                    };
                });
                TryCatchThrow("Minion Skin Replacements", delegate
                {
                    skinDefParams.minionSkinReplacements = Array.Empty<SkinDefParams.MinionSkinReplacement>();
                });
                TryCatchThrow("Projectile Ghost Replacements", delegate
                {
                    skinDefParams.projectileGhostReplacements = Array.Empty<SkinDefParams.ProjectileGhostReplacement>();
                });
                skin.skinDefParams = skinDefParams;
                Array.Resize(ref skinController.skins, skinController.skins.Length + 1);
                skinController.skins[skinController.skins.Length - 1] = skin;
                SkinCatalog.skinsByBody[(int)BodyCatalog.FindBodyIndex(gameObject)] = skinController.skins;
                //BodyCatalog.skins[(int)BodyCatalog.FindBodyIndex(gameObject)] = skinController.skins;
            }
            catch (Exception ex2)
            {
                Log.Warning((object)("Failed to add \"" + skinDefName + "\" skin to \"" + BodyName + "\""));
                Log.Error((object)ex2);
            }

            void TryCatchThrow(string message, Action action)
            {
                try
                {
                    action?.Invoke();
                }
                catch (Exception innerException)
                {
                    throw new Exception(message, innerException);
                }
            }
        }

        private static void LoadMaterialsWithReplacedShader(string shaderPath, params string[] materialPaths)
        {
            Shader shader = Addressables.LoadAssetAsync<Shader>(shaderPath).WaitForCompletion();
            foreach (string text in materialPaths)
            {
                Material material = bundle.LoadAsset<Material>(text);
                material.shader = shader;
                materialsWithRoRShader.Add(material);
            }
        }
    }
}
