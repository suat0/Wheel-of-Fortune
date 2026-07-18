using System.Collections.Generic;
using UnityEngine;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    /// <summary>
    /// Thin composition root for the gameplay UI: routes GameManager events to
    /// focused sub-views. All visual behavior (tweens, colors, texts) lives in
    /// the views themselves.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private GameManager gameManager;

        [Header("Panels")]
        [SerializeField] private GameOverPanelView gameOverPanel;
        [SerializeField] private CollectPanelView collectPanel;

        [Header("Views")]
        [SerializeField] private WheelVisualsView wheelVisuals;
        [SerializeField] private SpinButtonView spinButton;
        [SerializeField] private CollectButtonView collectButton;
        [SerializeField] private BackgroundView background;
        [SerializeField] private RewardHintView rewardHint;
        [SerializeField] private ZoneIndicatorView zoneIndicator;
        [SerializeField] private RewardDisplayView rewardDisplay;
        [SerializeField] private ZoneTargetCardView safeTargetCard;
        [SerializeField] private ZoneTargetCardView superTargetCard;

        private void OnEnable()
        {
            if (gameManager != null)
            {
                gameManager.ZoneChanged += OnZoneChanged;
                gameManager.SpinStarted += OnSpinStarted;
                gameManager.SpinCompleted += OnSpinCompleted;
                gameManager.BombHit += OnBombHit;
                gameManager.RewardsCollected += OnRewardsCollected;
                gameManager.GameRestarted += OnGameRestarted;
            }

            if (spinButton != null)
                spinButton.Clicked += OnSpinClicked;
            if (collectButton != null)
                collectButton.Clicked += OnCollectClicked;
        }

        private void OnDisable()
        {
            if (gameManager != null)
            {
                gameManager.ZoneChanged -= OnZoneChanged;
                gameManager.SpinStarted -= OnSpinStarted;
                gameManager.SpinCompleted -= OnSpinCompleted;
                gameManager.BombHit -= OnBombHit;
                gameManager.RewardsCollected -= OnRewardsCollected;
                gameManager.GameRestarted -= OnGameRestarted;
            }

            if (spinButton != null)
                spinButton.Clicked -= OnSpinClicked;
            if (collectButton != null)
                collectButton.Clicked -= OnCollectClicked;
        }

        private void OnSpinClicked()
        {
            if (gameManager != null)
                gameManager.RequestSpin();
        }

        private void OnCollectClicked()
        {
            if (gameManager != null)
                gameManager.CollectRewards();
        }

        private void OnZoneChanged(int zone, ZoneType zoneType)
        {
            WheelConfig wheelConfig = gameManager != null ? gameManager.GetCurrentWheelConfig() : null;

            if (wheelVisuals != null)
                wheelVisuals.Apply(wheelConfig);

            if (rewardHint != null && wheelConfig != null && gameManager != null)
                rewardHint.Show(wheelConfig.RewardMultiplier * gameManager.GetCurrentZoneMultiplier());

            if (background != null)
                background.ApplyZone(zoneType);

            if (zoneIndicator != null)
                zoneIndicator.UpdateZone(zone);

            UpdateZoneTargetCards(zone, zoneType);
            UpdateButtonStates();
        }

        private void OnSpinStarted()
        {
            if (spinButton != null)
            {
                spinButton.SetInteractable(false);
                spinButton.PlayPressFeedback();
            }

            if (collectButton != null)
                collectButton.SetVisible(false);
        }

        private void OnSpinCompleted(WheelSpinResult result)
        {
            if (wheelVisuals != null)
                wheelVisuals.PlayResultFeedback();

            if (result.IsBomb)
                return;

            if (rewardDisplay != null)
            {
                rewardDisplay.Refresh(gameManager.CollectedRewards, result.Reward);
                rewardDisplay.PlayGainFeedback();
            }

            UpdateButtonStates();
        }

        private void OnBombHit()
        {
            if (rewardDisplay != null)
                rewardDisplay.Clear();

            if (background != null)
                background.FlashBomb();

            if (gameOverPanel != null)
                gameOverPanel.Show(OnRestartRequested);
        }

        private void OnRewardsCollected(IReadOnlyList<CollectedReward> rewards)
        {
            if (collectPanel != null)
                collectPanel.Show(rewards, OnRestartRequested);
        }

        private void OnGameRestarted()
        {
            if (gameOverPanel != null)
                gameOverPanel.Hide();
            if (collectPanel != null)
                collectPanel.Hide();
            if (rewardDisplay != null)
                rewardDisplay.Clear();

            UpdateButtonStates();
        }

        private void OnRestartRequested()
        {
            if (gameManager != null)
                gameManager.RestartGame();
        }

        private void UpdateZoneTargetCards(int zone, ZoneType zoneType)
        {
            GameConfig config = gameManager != null ? gameManager.Config : null;
            if (config == null)
                return;

            if (safeTargetCard != null)
                safeTargetCard.UpdateCard(zone, config.SafeZoneInterval, zoneType);
            if (superTargetCard != null)
                superTargetCard.UpdateCard(zone, config.SuperZoneInterval, zoneType);
        }

        private void UpdateButtonStates()
        {
            if (gameManager == null)
                return;

            if (spinButton != null)
                spinButton.SetInteractable(gameManager.CanSpin);
            if (collectButton != null)
                collectButton.SetVisible(gameManager.CanCollect);
        }
    }
}
