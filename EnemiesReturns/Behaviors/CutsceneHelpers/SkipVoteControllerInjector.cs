using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Behaviors.CutsceneHelpers
{
    public class SkipVoteControllerInjector : MonoBehaviour
    {
        public VoteController voteController;

        private void Awake()
        {
            if (!voteController)
            {
                return;
            }
            var skipVoteOverlay = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_UI.SkipVoteOverlay_prefab).WaitForCompletion();
            skipVoteOverlay.transform.SetParent(base.transform, false);
            var nakedButton = skipVoteOverlay.transform.Find("CanvasGroup/NakedButton");
            if (nakedButton)
            {
                var hgButton = nakedButton.GetComponent<HGButton>();
                if (hgButton)
                {
                    hgButton.onClick.AddListener(voteController.SubmitVoteZeroFotAllLocalUsers);
                }
            }
            var voteInfoPanel = skipVoteOverlay.transform.Find("CanvasGroup/NakedButton/VoteInfoPanel");
            if (voteInfoPanel)
            {
                var controller = voteInfoPanel.GetComponent<VoteInfoPanelController>();
                if (controller)
                {
                    controller.voteController = voteController;
                }
            }

        }

    }
}
