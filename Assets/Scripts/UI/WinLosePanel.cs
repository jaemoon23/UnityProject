using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace NovelianMagicLibraryDefense.UI
{
    /// <summary>
    /// LCB: Manages victory and defeat condition panels in combat
    /// Handles showing appropriate UI based on battle outcome
    /// </summary>
    public class WinLosePanel : MonoBehaviour
    {
        [Header("Panel References")]
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject defeatPanel;

        [Header("Victory UI Elements")]
        [SerializeField] private TextMeshProUGUI victoryStageText;
        [SerializeField] private TextMeshProUGUI victoryTimeText;
        [SerializeField] private TextMeshProUGUI victoryRewardText;
        [SerializeField] private Button victoryNextButton;
        [SerializeField] private Button victoryStageSelectButton;

        [Header("Defeat UI Elements")]
        [SerializeField] private TextMeshProUGUI defeatStageText;
        [SerializeField] private TextMeshProUGUI defeatTimeText;
        [SerializeField] private TextMeshProUGUI defeatRewardText;
        [SerializeField] private Button defeatRetryButton;
        [SerializeField] private Button defeatStageSelectButton;

        private void Awake()
        {
            //LCB: Setup button listeners on awake
            SetupButtonListeners();

            //LCB: Hide both panels initially
            HideAllPanels();
        }

        private void OnDestroy()
        {
            //LCB: Clean up button listeners
            RemoveButtonListeners();
        }

        #region Button Setup

        //LCB: Setup all button click listeners for both panels
        private void SetupButtonListeners()
        {
            if (victoryNextButton != null)
                victoryNextButton.onClick.AddListener(OnVictoryNextButtonClicked);

            if (victoryStageSelectButton != null)
                victoryStageSelectButton.onClick.AddListener(OnVictoryStageSelectButtonClicked);

            if (defeatRetryButton != null)
                defeatRetryButton.onClick.AddListener(OnDefeatRetryButtonClicked);

            if (defeatStageSelectButton != null)
                defeatStageSelectButton.onClick.AddListener(OnDefeatStageSelectButtonClicked);
        }

        //LCB: Remove all button click listeners
        private void RemoveButtonListeners()
        {
            if (victoryNextButton != null)
                victoryNextButton.onClick.RemoveListener(OnVictoryNextButtonClicked);

            if (victoryStageSelectButton != null)
                victoryStageSelectButton.onClick.RemoveListener(OnVictoryStageSelectButtonClicked);

            if (defeatRetryButton != null)
                defeatRetryButton.onClick.RemoveListener(OnDefeatRetryButtonClicked);

            if (defeatStageSelectButton != null)
                defeatStageSelectButton.onClick.RemoveListener(OnDefeatStageSelectButtonClicked);
        }

        #endregion

        #region Victory Panel Methods

        //LCB: Show victory panel with stage completion data
        public void ShowVictoryPanel(string stageName, float clearTime, int reward)
        {
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(true);
                UpdateVictoryInfo(stageName, clearTime, reward);
                Debug.Log("[WinLosePanel] Victory panel displayed");
            }

            if (defeatPanel != null)
            {
                defeatPanel.SetActive(false);
            }

            //TODO: Implement victory logic (animations, sounds, etc.)
        }

        //LCB: Update victory panel information
        private void UpdateVictoryInfo(string stageName, float clearTime, int reward)
        {
            if (victoryStageText != null)
            {
                victoryStageText.text = stageName;
            }

            if (victoryTimeText != null)
            {
                int minutes = Mathf.FloorToInt(clearTime / 60f);
                int seconds = Mathf.FloorToInt(clearTime % 60f);
                victoryTimeText.text = $"전투시간 : {minutes:00}:{seconds:00}";
            }

            if (victoryRewardText != null)
            {
                victoryRewardText.text = $"처치 몬스터 : {reward}마리";
            }
        }

        //LCB: Handle victory next stage button click
        private void OnVictoryNextButtonClicked()
        {
            Debug.Log("[WinLosePanel] Victory Next button clicked - Logic to be implemented");
            //TODO: Implement next stage logic
        }

        //LCB: Handle victory stage select button click
        private void OnVictoryStageSelectButtonClicked()
        {
            Debug.Log("[WinLosePanel] Victory Stage Select button clicked - Logic to be implemented");
            //TODO: Implement stage select logic
        }

        #endregion

        #region Defeat Panel Methods

        //LCB: Show defeat panel with stage failure data
        public void ShowDefeatPanel(string stageName, float survivalTime, int monstersKilled)
        {
            if (defeatPanel != null)
            {
                defeatPanel.SetActive(true);
                UpdateDefeatInfo(stageName, survivalTime, monstersKilled);
                Debug.Log("[WinLosePanel] Defeat panel displayed");
            }

            if (victoryPanel != null)
            {
                victoryPanel.SetActive(false);
            }

            //TODO: Implement defeat logic (animations, sounds, etc.)
        }

        //LCB: Update defeat panel information
        private void UpdateDefeatInfo(string stageName, float survivalTime, int monstersKilled)
        {
            if (defeatStageText != null)
            {
                defeatStageText.text = stageName;
            }

            if (defeatTimeText != null)
            {
                int minutes = Mathf.FloorToInt(survivalTime / 60f);
                int seconds = Mathf.FloorToInt(survivalTime % 60f);
                defeatTimeText.text = $"전투시간 : {minutes:00}:{seconds:00}";
            }

            if (defeatRewardText != null)
            {
                defeatRewardText.text = $"처치 몬스터 : {monstersKilled}마리";
            }
        }

        //LCB: Handle defeat retry button click
        private void OnDefeatRetryButtonClicked()
        {
            Debug.Log("[WinLosePanel] Defeat Retry button clicked - Logic to be implemented");
            //TODO: Implement retry stage logic
        }

        //LCB: Handle defeat stage select button click
        private void OnDefeatStageSelectButtonClicked()
        {
            Debug.Log("[WinLosePanel] Defeat Stage Select button clicked - Logic to be implemented");
            //TODO: Implement stage select logic
        }

        #endregion

        #region Panel Control

        //LCB: Hide all panels
        public void HideAllPanels()
        {
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(false);
            }

            if (defeatPanel != null)
            {
                defeatPanel.SetActive(false);
            }

            Debug.Log("[WinLosePanel] All panels hidden");
        }

        //LCB: Check if any panel is currently visible
        public bool IsAnyPanelVisible()
        {
            bool victoryVisible = victoryPanel != null && victoryPanel.activeSelf;
            bool defeatVisible = defeatPanel != null && defeatPanel.activeSelf;
            return victoryVisible || defeatVisible;
        }

        #endregion
    }
}
