using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using Rewired.ComponentControls.Effects;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.EquipmentSlot;

namespace EnemiesReturns.Enemies.ContactLight
{
    public static class SetupContactLight
    {

        public static void Hooks()
        {
            if (Configuration.General.EnableContactLight.Value)
            {
                IL.ProximityHighlight.OnPreRenderOutlineHighlight += ProximityHighlight_OnPreRenderOutlineHighlight;
                IL.RoR2.InteractionDriver.OnPreRenderOutlineHighlight += InteractionDriver_OnPreRenderOutlineHighlight;

                CostTypeCatalog.modHelper.getAdditionalEntries += ModHelper_getAdditionalEntries;
            }
        }

        private static void ModHelper_getAdditionalEntries(List<CostTypeDef> list)
        {
            if (Content.CostTypes.AccessCard != null)
            {
                list.Add(Content.CostTypes.AccessCard);
            }
        }

        private static void InteractionDriver_OnPreRenderOutlineHighlight(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            var match = c.TryGotoNext(MoveType.After,
                x => x.MatchCallvirt<RoR2.OutlineHighlight>("AddHighlight"));

            if (match)
            {
                c.Emit(OpCodes.Ldarg, 0); // outlinehighlight
                c.Emit(OpCodes.Ldloc, 2); // game object with highlights
                c.Emit(OpCodes.Ldloc, 4); // existing highlight
                c.Emit(OpCodes.Ldloc, 5); // precalculcated color
                c.EmitDelegate<Action<OutlineHighlight, GameObject, Highlight, Color>>(UpdateOtherHighlights);
            }
            else
            {
                Log.Warning($"IL Hook Failed - ProximityHighlight.OnPreRenderOutlineHighlight: Contact Light doors will have only one outline.");
            }

            void UpdateOtherHighlights(OutlineHighlight outlineHighlight, GameObject highlightGameObject, RoR2.Highlight highlight, Color color)
            {
                var highlights = highlightGameObject.GetComponents<Highlight>();
                if (highlights.Length < 2)
                {
                    return;
                }

                foreach (var highlight2 in highlights)
                {
                    if (highlight2 == highlight)
                    {
                        continue;
                    }

                    outlineHighlight.AddHighlight(highlight2.targetRenderer, color * highlight2.strength);
                }
            }
        }

        private static void ProximityHighlight_OnPreRenderOutlineHighlight(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            var match = c.TryGotoNext(MoveType.After,
                x => x.MatchCallvirt<RoR2.OutlineHighlight>("AddHighlight"));

            if (match)
            {
                c.Emit(OpCodes.Ldarg, 0); // self
                c.Emit(OpCodes.Ldarg, 1); // outlinehighlight
                c.Emit(OpCodes.Ldloc, 2); // gameObject
                c.Emit(OpCodes.Ldloc, 4); // already found highlight
                c.EmitDelegate<Action<ProximityHighlight, RoR2.OutlineHighlight, GameObject, RoR2.Highlight>>(UpdateOtherHighlights);
            }
            else
            {
                Log.Warning($"IL Hook Failed - ProximityHighlight.OnPreRenderOutlineHighlight: Contact Light doors will have only one outline.");
            }

            void UpdateOtherHighlights(ProximityHighlight self, RoR2.OutlineHighlight outline, GameObject gameObject, RoR2.Highlight highlight)
            {
                var highlights = gameObject.GetComponents<Highlight>();
                if (highlights.Length < 2)
                {
                    return;
                }
                foreach (var highlight2 in highlights)
                {
                    if (highlight2 == highlight)
                    {
                        continue;
                    }

                    Color h = highlight2.GetColor() * highlight2.strength * self.highlightScale;
                    outline.AddHighlight(highlight2.targetRenderer, h);
                }
            }
        }

        public static GameObject CreateEliteSlayerIndicator(Sprite proviMask)
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_PassiveHealing.WoodSpriteIndicator_prefab).WaitForCompletion().InstantiateClone("EliteSlayerWeaponIndicator", false);

            var spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();

            prefab.transform.Find("Holder").localScale = new Vector3(0.2f, 0.2f, 0.2f);

            prefab.GetComponentInChildren<RotateAroundAxis>().slowRotationSpeed = 10f;

            spriteRenderer.sprite = proviMask;
            spriteRenderer.color = new Color(0.8490566f, 0.7833268f, 0f, 1f);

            prefab.GetComponentInChildren<TextMeshPro>().color = new Color(0.8490566f, 0.7833268f, 0f, 1f);

            return prefab;
        }
    }
}
