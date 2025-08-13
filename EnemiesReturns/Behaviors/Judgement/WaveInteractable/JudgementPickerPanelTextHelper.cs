using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors.Judgement.WaveInteractable
{
    public class JudgementPickerPanelTextHelper : MonoBehaviour
    {
        public PickupPickerPanel pickupPickerPanel;

        private JudgementSelectionController selectionController;

        private void Start()
        {
            if(pickupPickerPanel && pickupPickerPanel.pickerController)
            {
                selectionController = pickupPickerPanel.pickerController.gameObject.GetComponent<JudgementSelectionController>();
            }
        }


        public void AddQuantityToPickerButton(MPButton button, PickupDef pickupDef)
        {
            if (!selectionController && pickupPickerPanel && pickupPickerPanel.pickerController)
            {
                selectionController = pickupPickerPanel.pickerController.gameObject.GetComponent<JudgementSelectionController>();
            }

            if (!selectionController)
            {
                return;
            }

            var itemCount = selectionController.GetItemCountFromList(pickupDef.pickupIndex);
            if (itemCount < 0)
            {
                itemCount = 10;
            }

            var textGameTransform = button.transform.Find("CostText");
            GameObject textGameObject;
            if (textGameTransform)
            {
                textGameObject = textGameTransform.gameObject;
            }
            else
            {
                textGameObject = new GameObject("CostText");
            }
            textGameObject.transform.SetParent(button.transform);
            textGameObject.layer = 5;

            var counterRect = textGameObject.GetOrAddComponent<RectTransform>();
            counterRect.localPosition = Vector3.zero;
            counterRect.anchorMin = Vector2.zero;
            counterRect.anchorMax = Vector2.one;
            counterRect.localScale = Vector3.one;
            counterRect.sizeDelta = new Vector2(-10, -4);
            counterRect.anchoredPosition = Vector2.zero;

            var counterText = textGameObject.GetOrAddComponent<HGTextMeshProUGUI>();
            counterText.enableWordWrapping = false;
            counterText.alignment = TMPro.TextAlignmentOptions.BottomRight;
            counterText.fontSize = 20f;
            counterText.color = new Color(0f, 255/255f, 233/255f);
            counterText.outlineColor = Color.gray;
            counterText.outlineWidth = 0.2f;
            counterText.text = "x" + itemCount;
        }
    }
}
