using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    public class GameOverPanelView : AnimatedPanelView
    {
        private static readonly Color DefaultPanelColor = new Color(0.22f, 0.02f, 0.04f, 0.68f);
        private static readonly Color FlashColor = new Color(1f, 0.04f, 0.03f, 0.95f);
        private const string DefaultTitle = "OH NO, A BOMB EXPLODED RIGHT IN YOUR HANDS!";

        [SerializeField] private Button restartButton;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image iconImage;
        [SerializeField] private UITextConfig textConfig;

        private Action onRestartCallback;
        private Image panelBackground;

        protected override void Awake()
        {
            base.Awake();
            panelBackground = GetComponent<Image>();

            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);
        }

        public void Show(Action onRestart)
        {
            onRestartCallback = onRestart;
            BeginShow();

            if (titleText != null)
                titleText.text = textConfig != null ? textConfig.GameOverTitle : DefaultTitle;

            if (panelBackground != null)
            {
                Color originalColor = IsFadedBlack(panelBackground.color) ? DefaultPanelColor : panelBackground.color;
                panelBackground.color = FlashColor;
                ShowSequence.Append(CanvasGroup.DOFade(1f, 0.1f));
                ShowSequence.Join(panelBackground.DOColor(originalColor, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.1f));
            }
            else
            {
                ShowSequence.Append(CanvasGroup.DOFade(1f, 0.15f));
            }

            RectTransform content = Content;
            if (content != null)
            {
                content.localScale = Vector3.one * 0.5f;
                ShowSequence.Join(content.DOScale(1f, 0.25f).SetEase(Ease.OutBack));
                ShowSequence.Join(content.DOShakePosition(0.3f, 10f, 15, 90f, false, true, ShakeRandomnessMode.Harmonic));
            }

            if (iconImage != null)
            {
                iconImage.transform.localScale = Vector3.zero;
                ShowSequence.Join(iconImage.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack).SetDelay(0.05f));
                ShowSequence.AppendInterval(0.1f);
                ShowSequence.Append(iconImage.transform.DOPunchRotation(new Vector3(0f, 0f, 15f), 0.3f, 6));
            }
        }

        public override void Hide()
        {
            base.Hide();
            onRestartCallback = null;
        }

        private void OnRestartClicked()
        {
            onRestartCallback?.Invoke();
        }

        private static bool IsFadedBlack(Color color)
        {
            return color.r < 0.05f && color.g < 0.05f && color.b < 0.05f;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (restartButton == null)
                restartButton = GetComponentInChildren<Button>(true);
        }
#endif
    }
}
