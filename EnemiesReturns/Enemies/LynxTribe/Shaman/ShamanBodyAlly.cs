using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.Enemies.LynxTribe.Shaman
{
    public class ShamanBodyAlly : ShamanBody
    {
        public new struct SpawnCards
        {
            public static CharacterSpawnCard cscLynxShamanAlly;
        }

        public new static GameObject BodyPrefab;

        public new struct SkinDefs
        {
            public static SkinDef Ally;
        }

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log)
        {
            var result = base.AddBodyComponents(bodyPrefab, sprite, log);

            result.AddComponent<TeamFilter>().defaultTeam = TeamIndex.None;

            var buffWard = result.AddComponent<BuffWard>();
            buffWard.shape = BuffWard.BuffWardShape.VerticalTube;
            buffWard.radius = EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBuffWardRadius.Value;
            buffWard.interval = EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBuffWardBuffRefreshTimer.Value;
            buffWard.rangeIndicator = null;
            buffWard.buffDef = Content.Buffs.LynxShamanSpecialDamage;
            buffWard.buffDuration = EnemiesReturns.Configuration.LynxTribe.LynxTotem.LynxFetishBuffWardBuffDuration.Value;
            buffWard.floorWard = false;
            buffWard.expires = false;
            buffWard.animateRadius = false;

            return result;
        }

        public BuffDef CreateSpecialBuffDef(Sprite sprite)
        {
            BuffDef buff = ScriptableObject.CreateInstance<BuffDef>();
            (buff as ScriptableObject).name = "bdLynxSpecial";
            if (sprite)
            {
                buff.iconSprite = sprite;
            }
            buff.isDebuff = false;
            buff.canStack = false;
            buff.isCooldown = false;
            buff.isDOT = false;
            buff.isHidden = false;
            buff.buffColor = Color.magenta;

            return buff;
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Shaman.DeathState)));
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
            var maskRenderer = modelPrefab.transform.Find("LynxShamanMask").gameObject.GetComponent<SkinnedMeshRenderer>();
            var weaponRenderer = modelPrefab.transform.Find("LynxShamanWeapon").gameObject.GetComponent<SkinnedMeshRenderer>();

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
                }
            };

            SkinDefs.Ally = Utils.CreateSkinDef("skinLynxShamanAlly", modelPrefab, defaultRender);

            return new SkinDef[] { SkinDefs.Ally };
        }
    }
}
