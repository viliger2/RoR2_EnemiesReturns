using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.UI;
using RoR2.UI;
using RoR2;

namespace EnemiesReturns.Items.AdrenalineCore
{
    public class AdrenalineCoreUI : MonoBehaviour
    {
        private static List<AdrenalineCoreUI> instanceList = new List<AdrenalineCoreUI>();

        private HUD hud;

        public AdrenalineCoreMasterComponent masterBehaviour;

        private HGTextMeshProUGUI textMesh;

        private Image levelBar;

        private void Update()
        {
            if (masterBehaviour) 
            {
                UpdateUI(masterBehaviour.GetCurrentPoints(), masterBehaviour.GetCurrentPointsPerLevel());
            }
        }

        private void UpdateUI(float adrenalineLevel, float adrenalinePerLevel)
        {
            if (textMesh)
            {
                textMesh.SetText(string.Format(RoR2.Language.GetString("ENEMIES_RETURNS_CONTACTLIGHT_ITEM_ADRENALINE_CORE_LEVEL"), (int)(adrenalineLevel / adrenalinePerLevel)));
            }
            if (levelBar)
            {
                if (adrenalineLevel >= adrenalinePerLevel * 5)
                {
                    levelBar.fillAmount = 1f;
                }
                else
                {
                    levelBar.fillAmount = Mathf.Clamp((float)adrenalineLevel % adrenalinePerLevel / adrenalinePerLevel, 0f, 1f);
                }

            }
        }

        public void Enable(AdrenalineCoreMasterComponent masterComponent)
        {
            this.masterBehaviour = masterComponent;
            if (gameObject)
            {
                gameObject.SetActive(true);
            }
        }

        public void Disable() 
        {
            this.masterBehaviour = null;
            if (gameObject) 
            { 
                gameObject.SetActive(false); 
            }
        }

        public static AdrenalineCoreUI FindInstance(CharacterMaster master)
        {
            foreach (AdrenalineCoreUI instance in instanceList)
            {
                if (instance.hud.targetMaster == master) return instance;
            }
            return null;
        }

        public static void Hooks()
        {
            On.RoR2.UI.HUD.Awake += HUD_Awake;
        }

        private static void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
        {
            orig(self);
            if (Configuration.ContactLight.AdrenalineCore.EnableUI.Value)
            {
                CreateUI(self);
            }
        }

        public static void CreateUI(RoR2.UI.HUD HUD)
        {
            if (!HUD || !HUD.healthBar || !HUD.itemInventoryDisplay)
            {
                return;
            }

            if (!HUD.itemInventoryDisplay.gameObject.TryGetComponent<Image>(out var copyImage))
            {
                return;
            }

            var AdrenalineHUD = new GameObject("AdrenalineHUD");

            var instance = AdrenalineHUD.AddComponent<AdrenalineCoreUI>();

            RectTransform rectTransform = AdrenalineHUD.AddComponent<RectTransform>();

            AdrenalineHUD.transform.SetParent(HUD.healthBar.transform);

            Image image = AdrenalineHUD.AddComponent<Image>();

            image.sprite = copyImage.sprite;
            image.color = copyImage.color;
            image.type = Image.Type.Sliced;

            rectTransform.localPosition = Vector3.zero;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.localScale = Vector3.one;
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.sizeDelta = new Vector2(421, 15);
            rectTransform.anchoredPosition = new Vector2(-1, -5);
            rectTransform.eulerAngles = new Vector3(0, -6, 0);

            GameObject AdrenalineLevelText = new GameObject("AdrenalineLevelText");
            RectTransform rectTransform1 = AdrenalineLevelText.AddComponent<RectTransform>();

            var textMesh = AdrenalineLevelText.AddComponent<HGTextMeshProUGUI>();

            AdrenalineLevelText.transform.SetParent(AdrenalineHUD.transform);

            rectTransform1.localPosition = Vector3.zero;
            rectTransform1.anchorMin = Vector2.zero;
            rectTransform1.anchorMax = Vector2.one;
            rectTransform1.localScale = Vector3.one;
            rectTransform1.sizeDelta = new Vector2(0, 0);
            rectTransform1.anchoredPosition = new Vector2(380, 0);
            rectTransform1.eulerAngles = new Vector3(0, -6, 0);

            textMesh.enableAutoSizing = true;
            textMesh.fontSizeMax = 256;
            textMesh.faceColor = Color.white;
            textMesh.alignment = TMPro.TextAlignmentOptions.MidlineLeft;
            textMesh.text = "Testing";

            GameObject AdrenalineLevelBar = new GameObject("AdrenalineLevelBar");
            RectTransform rectTransform2 = AdrenalineLevelBar.AddComponent<RectTransform>();

            AdrenalineLevelBar.transform.SetParent(AdrenalineHUD.transform);

            // I don't know how I did it but it is now attached to the top left corner of the parent
            rectTransform2.anchorMin = new Vector2(0, 1);
            rectTransform2.anchorMax = new Vector2(0, 1);
            rectTransform2.pivot = new Vector2(0, 1);
            rectTransform2.eulerAngles = new Vector3(0, -6, 0);
            rectTransform2.localScale = Vector3.one;
            rectTransform2.anchoredPosition = new Vector2(12, 2);
            rectTransform2.sizeDelta = new Vector2(360, 8);

            var levelBar = AdrenalineLevelBar.AddComponent<Image>();

            // you need to have a sprite if you want "Fill" to work
            levelBar.sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/UI/texUINonsegmentedHealthbar.png").WaitForCompletion();
            levelBar.color = Color.yellow;
            levelBar.type = Image.Type.Filled;
            levelBar.fillMethod = Image.FillMethod.Horizontal;
            levelBar.fillOrigin = (int)Image.OriginHorizontal.Left;
            levelBar.fillAmount = 0.5f;

            instance.levelBar = levelBar;
            instance.textMesh = textMesh;
            instance.hud = HUD;

            AdrenalineHUD.gameObject.SetActive(false);

            instanceList.Add(instance);
        }

    }
}
