using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using VertigoCase.Core;
using VertigoCase.Data;
namespace VertigoCase.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private GameManager gameManager;

        [Header("Panels")]
        [SerializeField] private GameOverPanelView ui_panel_gameover;
        [SerializeField] private CollectPanelView ui_panel_collect;

        [Header("Wheel Visuals")]
        [SerializeField] private Image ui_image_spin_base_value;
        [SerializeField] private Image ui_image_spin_indicator_value;
        [SerializeField] private Image wheelBackplateImage;

        [Header("Buttons")]
        [SerializeField] private Button ui_button_spin;
        [SerializeField] private Button ui_button_collect;

        [Header("Zone")]
        [SerializeField] private ZoneIndicatorView zoneIndicator;

        [Header("Spin Info")]
        [SerializeField] private TextMeshProUGUI ui_text_reward_hint_value;

        [Header("Background")]
        [SerializeField] private Image ui_image_background;
        [SerializeField] private Color normalBgColor = new Color(0.025f, 0.012f, 0.04f);
        [SerializeField] private Color safeBgColor = new Color(0.015f, 0.045f, 0.035f);
        [SerializeField] private Color superBgColor = new Color(0.1f, 0.065f, 0.012f);

        [Header("Rewards")]
        [SerializeField] private RewardDisplayView rewardDisplay;

        [Header("Zone Targets")]
        [SerializeField] private Image safeTargetBg;
        [SerializeField] private Image superTargetBg;
        [SerializeField] private TextMeshProUGUI safeTargetValueText;
        [SerializeField] private TextMeshProUGUI superTargetValueText;

        private Tweener spinButtonPulse;

        private void Awake()
        {
            if (ui_button_spin != null)
                ui_button_spin.onClick.AddListener(OnSpinClicked);
            if (ui_button_collect != null)
                ui_button_collect.onClick.AddListener(OnCollectClicked);
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
            KillSpinPulse();
        }

        private void Subscribe()
        {
            if (gameManager == null)
                return;

            gameManager.ZoneChanged += OnZoneChanged;
            gameManager.SpinStarted += OnSpinStarted;
            gameManager.SpinCompleted += OnSpinCompleted;
            gameManager.BombHit += OnBombHit;
            gameManager.RewardsCollected += OnRewardsCollected;
            gameManager.GameRestarted += OnGameRestarted;
        }

        private void Unsubscribe()
        {
            if (gameManager == null)
                return;

            gameManager.ZoneChanged -= OnZoneChanged;
            gameManager.SpinStarted -= OnSpinStarted;
            gameManager.SpinCompleted -= OnSpinCompleted;
            gameManager.BombHit -= OnBombHit;
            gameManager.RewardsCollected -= OnRewardsCollected;
            gameManager.GameRestarted -= OnGameRestarted;
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
            UpdateWheelVisuals();
            UpdateSpinInfo(zoneType);
            UpdateBackground(zoneType);
            UpdateButtonStates();

            if (zoneIndicator != null)
                zoneIndicator.UpdateZone(zone);

            UpdateZoneTargetCards(zone, zoneType);
        }

        private void OnSpinStarted()
        {
            SetSpinButtonInteractable(false);
            SetCollectButtonVisible(false);

            if (ui_button_spin != null)
            {
                ui_button_spin.transform.DOKill();
                ui_button_spin.transform.localScale = Vector3.one;
                ui_button_spin.transform.DOScale(0.9f, 0.15f).SetEase(Ease.InBack)
                    .OnComplete(() => ui_button_spin.transform.DOScale(1f, 0.1f));
            }
        }

        private void OnSpinCompleted(WheelSpinResult result)
        {
            if (ui_image_spin_indicator_value != null)
            {
                ui_image_spin_indicator_value.transform.DOKill();
                ui_image_spin_indicator_value.transform.localScale = Vector3.one;
                ui_image_spin_indicator_value.transform.DOPunchScale(Vector3.one * 0.35f, 0.5f, 8, 0.4f);
            }

            if (ui_image_spin_base_value != null)
            {
                ui_image_spin_base_value.transform.DOPunchScale(Vector3.one * 0.04f, 0.35f, 4, 0.3f);
            }

            if (!result.IsBomb)
            {
                if (rewardDisplay != null)
                {
                    rewardDisplay.Refresh(gameManager.CollectedRewards);
                    rewardDisplay.transform.DOKill();
                    rewardDisplay.transform.localScale = Vector3.one;
                    rewardDisplay.transform.DOPunchScale(Vector3.one * 0.1f, 0.25f, 5, 0.4f);
                }

                UpdateButtonStates();
            }
        }

        private void OnBombHit()
        {
            if (rewardDisplay != null)
                rewardDisplay.Clear();

            if (ui_image_background != null)
            {
                Color bgBefore = ui_image_background.color;
                ui_image_background.DOKill();
                ui_image_background.DOColor(new Color(0.4f, 0f, 0f), 0.08f)
                    .OnComplete(() => ui_image_background.DOColor(bgBefore, 0.4f).SetEase(Ease.OutQuad));
            }

            if (ui_panel_gameover != null)
                ui_panel_gameover.Show(() => gameManager.RestartGame());
        }

        private void OnRewardsCollected(IReadOnlyList<CollectedReward> rewards)
        {
            if (ui_panel_collect != null)
                ui_panel_collect.Show(rewards, () => gameManager.RestartGame());
        }

        private void OnGameRestarted()
        {
            if (ui_panel_gameover != null)
                ui_panel_gameover.Hide();
            if (ui_panel_collect != null)
                ui_panel_collect.Hide();
            if (rewardDisplay != null)
                rewardDisplay.Clear();

            UpdateButtonStates();
        }

        private void UpdateWheelVisuals()
        {
            WheelConfig config = gameManager.GetCurrentWheelConfig();
            if (config == null)
                return;

            if (ui_image_spin_base_value != null && config.WheelSprite != null)
                ui_image_spin_base_value.sprite = config.WheelSprite;

            if (ui_image_spin_indicator_value != null && config.PointerSprite != null)
                ui_image_spin_indicator_value.sprite = config.PointerSprite;

            if (wheelBackplateImage != null && ui_image_spin_base_value != null)
                wheelBackplateImage.sprite = ui_image_spin_base_value.sprite;
        }

        private void UpdateSpinInfo(ZoneType zoneType)
        {
            WheelConfig config = gameManager.GetCurrentWheelConfig();

            if (ui_text_reward_hint_value != null && config != null)
            {
                ui_text_reward_hint_value.text = $"Up To x{Mathf.Max(1, config.RewardMultiplier)} Rewards";

                ui_text_reward_hint_value.transform.DOKill();
                ui_text_reward_hint_value.transform.localScale = Vector3.one;
                ui_text_reward_hint_value.DOFade(0f, 0f);
                ui_text_reward_hint_value.DOFade(1f, 0.4f).SetDelay(0.2f);
            }
        }

        private void UpdateBackground(ZoneType zoneType)
        {
            if (ui_image_background == null)
                return;

            Color targetColor;
            switch (zoneType)
            {
                case ZoneType.Safe:
                    targetColor = safeBgColor;
                    break;
                case ZoneType.Super:
                    targetColor = superBgColor;
                    break;
                default:
                    targetColor = normalBgColor;
                    break;
            }

            ui_image_background.DOKill();
            ui_image_background.DOColor(targetColor, 0.5f).SetEase(Ease.OutQuad);
        }

        private void UpdateButtonStates()
        {
            if (gameManager == null)
                return;

            SetSpinButtonInteractable(gameManager.CanSpin);
            SetCollectButtonVisible(gameManager.CanCollect);
        }

        private void SetSpinButtonInteractable(bool interactable)
        {
            if (ui_button_spin == null)
                return;

            ui_button_spin.interactable = interactable;
            KillSpinPulse();

            if (interactable)
            {
                spinButtonPulse = ui_button_spin.transform
                    .DOScale(1.06f, 0.6f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
        }

        private void SetCollectButtonVisible(bool visible)
        {
            if (ui_button_collect == null)
                return;

            if (visible)
            {
                ui_button_collect.gameObject.SetActive(true);
                ui_button_collect.interactable = true;
                ui_button_collect.transform.localScale = Vector3.zero;
                ui_button_collect.transform.DOKill();
                ui_button_collect.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
            }
            else
            {
                ui_button_collect.transform.DOKill();
                ui_button_collect.transform.localScale = Vector3.one;
                ui_button_collect.gameObject.SetActive(false);
                ui_button_collect.interactable = false;
            }
        }

        private void KillSpinPulse()
        {
            if (spinButtonPulse != null && spinButtonPulse.IsActive())
            {
                spinButtonPulse.Kill();
                spinButtonPulse = null;
            }

            if (ui_button_spin != null)
                ui_button_spin.transform.localScale = Vector3.one;
        }

        private void UpdateZoneTargetCards(int zone, ZoneType zoneType)
        {
            int currentZone = Mathf.Max(1, zone);
            int safeInterval = gameManager != null && gameManager.Config != null ? gameManager.Config.SafeZoneInterval : 10;
            int superInterval = gameManager != null && gameManager.Config != null ? gameManager.Config.SuperZoneInterval : 30;

            if (safeTargetValueText != null)
                safeTargetValueText.text = GetCurrentOrNextMilestone(currentZone, safeInterval).ToString();
            if (superTargetValueText != null)
                superTargetValueText.text = GetCurrentOrNextMilestone(currentZone, superInterval).ToString();

            if (safeTargetBg != null)
            {
                safeTargetBg.color = zoneType == ZoneType.Safe
                    ? new Color(0.12f, 0.82f, 0.08f, 0.95f)
                    : new Color(0.05f, 0.43f, 0.04f, 0.86f);

                if (zoneType == ZoneType.Safe)
                {
                    safeTargetBg.transform.DOKill();
                    safeTargetBg.transform.localScale = Vector3.one;
                    safeTargetBg.transform.DOPunchScale(Vector3.one * 0.15f, 0.4f, 6, 0.4f);
                }
            }

            if (superTargetBg != null)
            {
                superTargetBg.color = zoneType == ZoneType.Super
                    ? new Color(0.95f, 0.62f, 0.04f, 0.96f)
                    : new Color(0.72f, 0.43f, 0.02f, 0.86f);

                if (zoneType == ZoneType.Super)
                {
                    superTargetBg.transform.DOKill();
                    superTargetBg.transform.localScale = Vector3.one;
                    superTargetBg.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 6, 0.4f);
                }
            }
        }

        private int GetCurrentOrNextMilestone(int currentZone, int interval)
        {
            interval = Mathf.Max(1, interval);
            int remainder = currentZone % interval;
            return remainder == 0 ? currentZone : currentZone + interval - remainder;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_button_spin == null)
                ui_button_spin = FindButtonByName("ui_button_spin");
            if (ui_button_collect == null)
                ui_button_collect = FindButtonByName("ui_button_collect");
        }

        private static Button FindButtonByName(string name)
        {
            foreach (var btn in FindObjectsOfType<Button>(true))
            {
                if (btn.gameObject.name == name)
                    return btn;
            }
            return null;
        }
#endif
    }
}
