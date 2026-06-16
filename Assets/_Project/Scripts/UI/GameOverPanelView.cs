using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace VertigoCase.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GameOverPanelView : MonoBehaviour
    {
        private static readonly Color DefaultPanelColor = new Color(0.22f, 0.02f, 0.04f, 0.68f);
        private static readonly Color FlashColor = new Color(1f, 0.04f, 0.03f, 0.95f);

        [SerializeField] private Button ui_button_gameover_restart;
        [SerializeField] private TextMeshProUGUI ui_text_gameover_title_value;
        [SerializeField] private Image ui_image_gameover_icon;

        private Action onRestartCallback;
        private CanvasGroup canvasGroup;
        private Sequence showSequence;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            if (ui_button_gameover_restart != null)
                ui_button_gameover_restart.onClick.AddListener(OnRestartClicked);
        }

        public void Show(Action onRestart)
        {
            KillSequence();
            onRestartCallback = onRestart;
            gameObject.SetActive(true);

            if (ui_text_gameover_title_value != null)
                ui_text_gameover_title_value.text = "OH NO, A BOMB EXPLODED RIGHT IN YOUR HANDS!";

            canvasGroup.alpha = 0f;

            Image panelBg = GetComponent<Image>();

            showSequence = DOTween.Sequence();

            if (panelBg != null)
            {
                Color originalColor = IsFadedBlack(panelBg.color) ? DefaultPanelColor : panelBg.color;
                panelBg.color = FlashColor;
                showSequence.Append(canvasGroup.DOFade(1f, 0.1f));
                showSequence.Join(panelBg.DOColor(originalColor, 0.3f).SetEase(Ease.OutQuad).SetDelay(0.1f));
            }
            else
            {
                showSequence.Append(canvasGroup.DOFade(1f, 0.15f));
            }

            if (transform.childCount > 0)
            {
                Transform content = transform.GetChild(0);
                content.localScale = Vector3.one * 0.5f;
                showSequence.Join(content.DOScale(1f, 0.25f).SetEase(Ease.OutBack));
            }

            if (ui_image_gameover_icon != null)
            {
                ui_image_gameover_icon.transform.localScale = Vector3.zero;
                showSequence.Join(ui_image_gameover_icon.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack).SetDelay(0.05f));
                showSequence.AppendInterval(0.1f);
                showSequence.Append(ui_image_gameover_icon.transform.DOPunchRotation(new Vector3(0, 0, 15f), 0.3f, 6));
            }

            if (transform.childCount > 0)
            {
                showSequence.Join(transform.GetChild(0).DOShakePosition(0.3f, 10f, 15, 90f, false, true, ShakeRandomnessMode.Harmonic));
            }
        }

        public void Hide()
        {
            KillSequence();
            gameObject.SetActive(false);
            onRestartCallback = null;
        }

        private void OnRestartClicked()
        {
            onRestartCallback?.Invoke();
        }

        private void KillSequence()
        {
            if (showSequence != null && showSequence.IsActive())
            {
                showSequence.Kill(true);
                showSequence = null;
            }
        }

        private void OnDestroy()
        {
            KillSequence();
        }

        private static bool IsFadedBlack(Color color)
        {
            return color.r < 0.05f && color.g < 0.05f && color.b < 0.05f;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_button_gameover_restart == null)
                ui_button_gameover_restart = GetComponentInChildren<Button>(true);
        }
#endif
    }
}
