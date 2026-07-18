using UnityEngine;
using VertigoCase.Core;
using VertigoCase.Core.Events;
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
            EventBus.Subscribe<ZoneChangedEvent>(OnZoneChanged);
            EventBus.Subscribe<SpinStartedEvent>(OnSpinStarted);
            EventBus.Subscribe<SpinCompletedEvent>(OnSpinCompleted);
            EventBus.Subscribe<BombHitEvent>(OnBombHit);
            EventBus.Subscribe<RewardsCollectedEvent>(OnRewardsCollected);
            EventBus.Subscribe<GameRestartedEvent>(OnGameRestarted);

            if (spinButton != null)
                spinButton.Clicked += OnSpinClicked;
            if (collectButton != null)
                collectButton.Clicked += OnCollectClicked;
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ZoneChangedEvent>(OnZoneChanged);
            EventBus.Unsubscribe<SpinStartedEvent>(OnSpinStarted);
            EventBus.Unsubscribe<SpinCompletedEvent>(OnSpinCompleted);
            EventBus.Unsubscribe<BombHitEvent>(OnBombHit);
            EventBus.Unsubscribe<RewardsCollectedEvent>(OnRewardsCollected);
            EventBus.Unsubscribe<GameRestartedEvent>(OnGameRestarted);

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

        private void OnZoneChanged(ZoneChangedEvent evt)
        {
            WheelConfig wheelConfig = gameManager != null ? gameManager.GetCurrentWheelConfig() : null;

            if (wheelVisuals != null)
                wheelVisuals.Apply(wheelConfig);

            if (rewardHint != null && wheelConfig != null && gameManager != null)
                rewardHint.Show(wheelConfig.RewardMultiplier * gameManager.GetCurrentZoneMultiplier());

            if (background != null)
                background.ApplyZone(evt.ZoneType);

            if (zoneIndicator != null)
                zoneIndicator.UpdateZone(evt.Zone);

            UpdateZoneTargetCards(evt.Zone, evt.ZoneType);
            UpdateButtonStates();
        }

        private void OnSpinStarted(SpinStartedEvent evt)
        {
            if (spinButton != null)
            {
                spinButton.SetInteractable(false);
                spinButton.PlayPressFeedback();
            }

            if (collectButton != null)
                collectButton.SetVisible(false);
        }

        private void OnSpinCompleted(SpinCompletedEvent evt)
        {
            if (wheelVisuals != null)
                wheelVisuals.PlayResultFeedback();

            if (evt.Result.IsBomb)
                return;

            if (rewardDisplay != null)
            {
                rewardDisplay.Refresh(gameManager.CollectedRewards, evt.Result.Reward);
                rewardDisplay.PlayGainFeedback();
            }

            UpdateButtonStates();
        }

        private void OnBombHit(BombHitEvent evt)
        {
            if (rewardDisplay != null)
                rewardDisplay.Clear();

            if (background != null)
                background.FlashBomb();

            if (gameOverPanel != null)
                gameOverPanel.Show(OnRestartRequested);
        }

        private void OnRewardsCollected(RewardsCollectedEvent evt)
        {
            if (collectPanel != null)
                collectPanel.Show(evt.Rewards, OnRestartRequested);
        }

        private void OnGameRestarted(GameRestartedEvent evt)
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
