using EnemiesReturns.Components.BodyComponents;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Enemies.LynxTribe.Archer
{
    public class ArcherBodyAlly : ArcherBody
    {
        public new struct SpawnCards
        {
            public static CharacterSpawnCard cscLynxArcherAlly;
        }

        public new struct SkinDefs
        {
            public static SkinDef Ally;
        }

        public new static GameObject BodyPrefab;

        public static BuffDef LynxArcherDamage;

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite)
        {
            var result = base.AddBodyComponents(bodyPrefab, sprite);

            result.AddComponent<TeamFilter>().defaultTeam = TeamIndex.Player;

            var buffWard = result.AddComponent<BuffWard>();
            buffWard.shape = BuffWard.BuffWardShape.VerticalTube;
            buffWard.radius = EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBuffWardRadius.Value;
            buffWard.interval = EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBuffWardBuffRefreshTimer.Value;
            buffWard.rangeIndicator = null;
            buffWard.buffDef = LynxArcherDamage;
            buffWard.buffDuration = EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBuffWardBuffDuration.Value;
            buffWard.floorWard = false;
            buffWard.expires = false;
            buffWard.animateRadius = false;

            return result;
        }

        public BuffDef CreateDamageBuffDef(Sprite sprite)
        {
            BuffDef buff = ScriptableObject.CreateInstance<BuffDef>();
            (buff as ScriptableObject).name = "bdLynxDamage";
            if (sprite)
            {
                buff.iconSprite = sprite;
            }
            buff.isDebuff = false;
            buff.canStack = false;
            buff.isCooldown = false;
            buff.isDOT = false;
            buff.isHidden = false;
            buff.buffColor = Color.red;

            return buff;
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            var bodyParams = base.CharacterBodyParams(aimOrigin, icon);

            bodyParams.baseRegen = 0.6f;
            bodyParams.levelRegen = 0.12f;

            return bodyParams;
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("Lynx").gameObject.GetComponent<SkinnedMeshRenderer>();
            var maskRenderer = modelPrefab.transform.Find("LynxArcherMask").gameObject.GetComponent<SkinnedMeshRenderer>();
            var weaponRenderer = modelPrefab.transform.Find("LynxArcherBow").gameObject.GetComponent<SkinnedMeshRenderer>();
            var arrowQuad1 = modelPrefab.transform.Find("LynxArcher/Root/Base/Stomach/Chest/Shoulder_L/UpperArm_L/LowerArm_L/Hand_L/BowBaseU/String/Arrow/ArrowQuads/Quad1").gameObject.GetComponent<MeshRenderer>();
            var arrowQuad2 = modelPrefab.transform.Find("LynxArcher/Root/Base/Stomach/Chest/Shoulder_L/UpperArm_L/LowerArm_L/Hand_L/BowBaseU/String/Arrow/ArrowQuads/Quad2").gameObject.GetComponent<MeshRenderer>();
            var matHuntressArrow = ContentProvider.MaterialCache.GetValueOrDefault("matLynxArcherArrow");

            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = modelRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = maskRenderer,
                    defaultMaterial = maskRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = weaponRenderer,
                    defaultMaterial = weaponRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = arrowQuad1,
                    defaultMaterial = matHuntressArrow,
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = true
                },
                new CharacterModel.RendererInfo
                {
                    renderer = arrowQuad2,
                    defaultMaterial = matHuntressArrow,
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = true
                }
            };

            SkinDefs.Ally = Utils.CreateSkinDef("skinLynxArcherAlly", modelPrefab, defaultRender);

            return new SkinDef[] { SkinDefs.Ally };
        }

    }
}
