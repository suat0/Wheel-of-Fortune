using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using VertigoCase.Core;

namespace VertigoCase.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CollectPanelView : MonoBehaviour
    {
        [SerializeField] private Button ui_button_collect_confirm;
        [SerializeField] private TextMeshProUGUI ui_text_collect_title_value;
        [SerializeField] private RewardDisplayView rewardSummary;

        private Action onConfirmCallback;
        private CanvasGroup canvasGroup;
        private Sequence showSequence;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            if (ui_button_collect_confirm != null)
                ui_button_collect_confirm.onClick.AddListener(OnConfirmClicked);
        }

        public void Show(IReadOnlyList<CollectedReward> rewards, Action onConfirm)
        {
            KillSequence();
            onConfirmCallback = onConfirm;
            gameObject.SetActive(true);

            if (ui_text_collect_title_value != null)
                ui_text_collect_title_value.text = "COLLECTED!";

            if (rewardSummary != null)
                rewardSummary.Refresh(rewards);

            canvasGroup.alpha = 0f;

            showSequence = DOTween.Sequence();
            showSequence.Append(canvasGroup.DOFade(1f, 0.3f));

            if (transform.childCount > 0)
            {
                Transform content = transform.GetChild(0);
                content.localScale = Vector3.one * 0.5f;
                showSequence.Join(content.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
            }

            if (ui_text_collect_title_value != null)
            {
                showSequence.Append(
                    ui_text_collect_title_value.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 6, 0.5f)
                );
            }
        }

        public void Hide()
        {
            KillSequence();
            gameObject.SetActive(false);
            onConfirmCallback = null;

            if (rewardSummary != null)
                rewardSummary.Clear();
        }

        private void OnConfirmClicked()
        {
            onConfirmCallback?.Invoke();
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_button_collect_confirm == null)
                ui_button_collect_confirm = GetComponentInChildren<Button>(true);
        }
#endif
    }
}
