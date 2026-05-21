using RoR2.UI;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.ObjectModel;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Behaviors.SkinDefPicker
{
    public class SkinDefPickerPanel : MonoBehaviour
    {
        public class SkinDefPickerEvent : UnityEvent<MPButton, SkinDef>
        {

        }

        public GridLayoutGroup gridlayoutGroup;

        public RectTransform buttonContainer;

        public GameObject buttonPrefab;

        public Image[] coloredImages;

        public Image[] darkColoredImages;

        public int maxColumnCount;

        public bool useLockSpriteForUnavailableOptions = true;

        private UIElementAllocator<MPButton> buttonAllocator;

        public bool shouldChangeButtonFrameColor;

        public bool shouldLeaveDisabledButtonsInteractable;

        [SerializeField]
        [Tooltip("If the root of the panel is a button, keep this false!")]
        private bool automaticButtonNavigation;

        [Header("Events")]
        public SkinDefPickerEvent pickupSelected;

        public SkinDefPickerEvent pickupBaseContentReady;

        private Sprite lockedSprite;

        private SkinDefPickerController.Option[] pickupOptions;

        public Action OnPanelSetupComplete;

        public SkinDefPickerController pickerController { get; set; }

        private void Awake()
        {
            buttonAllocator = new UIElementAllocator<MPButton>(buttonContainer, buttonPrefab);
            buttonAllocator.onCreateElement = OnCreateButton;
            gridlayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridlayoutGroup.constraintCount = maxColumnCount;
            ScoreboardController.onScoreboardOpen += DestroyIt;
        }

        private void OnDestroy()
        {
            ScoreboardController.onScoreboardOpen -= DestroyIt;
        }

        private void DestroyIt()
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }

        private void OnCreateButton(int index, MPButton button)
        {
            button.onClick.AddListener(delegate
            {
                pickerController.SubmitChoice(index);
            });
            button.onSelect.AddListener(delegate
            {
                pickupSelected?.Invoke(button, pickupOptions[index].skin);
            });
        }

        public void RemovePickupButtonAvailability(int pickupOption)
        {
            if (lockedSprite == null)
            {
                lockedSprite = Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_MiscIcons.texUnlockIcon_png).WaitForCompletion();
            }
            pickupOptions[pickupOption].available = false;
            MPButton mPButton = buttonAllocator.elements[pickupOption];
            Image component = mPButton.GetComponent<ChildLocator>().FindChild("Icon").GetComponent<Image>();
            component.color = Color.gray;
            component.sprite = lockedSprite;
            if (!shouldLeaveDisabledButtonsInteractable)
            {
                mPButton.interactable = false;
            }
        }

        public void SetPickupOptions(SkinDefPickerController.Option[] options)
        {
            pickupOptions = options;
            buttonAllocator.AllocateElements(options.Length);
            ReadOnlyCollection<MPButton> elements = buttonAllocator.elements;
            if (lockedSprite)
            {
                lockedSprite = Addressables.LoadAssetAsync<Sprite>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_MiscIcons.texUnlockIcon_png).WaitForCompletion();
            }
            Sprite sprite = lockedSprite;
            if (options.Length != 0)
            {
                SkinDef skinDef = options[0].skin;
                Color baseColor = Color.white;
                Color darkColor = Color.gray;
                Image[] array = coloredImages;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].color *= baseColor;
                }
                array = darkColoredImages;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].color *= darkColor;
                }
            }
            for (int j = 0; j < options.Length; j++)
            {
                MPButton mPButton = elements[j];
                int num = j - j % maxColumnCount;
                int num2 = j % maxColumnCount;
                int num3 = num2 - maxColumnCount;
                int num4 = num2 - 1;
                int num5 = num2 + 1;
                int num6 = num2 + maxColumnCount;
                Navigation navigation = mPButton.navigation;
                if (automaticButtonNavigation)
                {
                    navigation.mode = Navigation.Mode.Automatic;
                }
                else
                {
                    navigation.mode = Navigation.Mode.Explicit;
                    if (num4 >= 0)
                    {
                        MPButton mPButton2 = (MPButton)(navigation.selectOnLeft = elements[num + num4]);
                    }
                    if (num5 < maxColumnCount && num + num5 < options.Length)
                    {
                        MPButton mPButton3 = (MPButton)(navigation.selectOnRight = elements[num + num5]);
                    }
                    if (num + num3 >= 0)
                    {
                        MPButton mPButton4 = (MPButton)(navigation.selectOnUp = elements[num + num3]);
                    }
                    if (num + num6 < options.Length)
                    {
                        MPButton mPButton5 = (MPButton)(navigation.selectOnDown = elements[num + num6]);
                    }
                }
                mPButton.navigation = navigation;
                ref SkinDefPickerController.Option reference = ref options[j];
                SkinDef skinDef2 = reference.skin;
                Image component = mPButton.GetComponent<ChildLocator>().FindChild("Icon").GetComponent<Image>();
                if (reference.available)
                {
                    component.color = Color.white;
                    component.sprite = skinDef2.icon;
                    mPButton.interactable = true;
                }
                else
                {
                    component.color = Color.gray;
                    if (useLockSpriteForUnavailableOptions)
                    {
                        component.sprite = sprite;
                    }
                    else
                    {
                        component.sprite = skinDef2.icon;
                    }
                    if (!shouldLeaveDisabledButtonsInteractable)
                    {
                        mPButton.interactable = false;
                    }
                }
                if (shouldChangeButtonFrameColor)
                {
                    mPButton.image.color = (reference.available ? Color.white : new Color(0.1f, 0.1f, 0.1f, 1f));
                }
                ColorBlock colors = mPButton.colors;
                pickupBaseContentReady?.Invoke(mPButton, skinDef2);
            }
            OnPanelSetupComplete?.Invoke();
        }
    }
}
