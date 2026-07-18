using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    public class CollectPanelView : AnimatedPanelView
    {
        private const string DefaultTitle = "COLLECTED!";

        [SerializeField] private Button confirmButton;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private RewardDisplayView rewardSummary;
        [SerializeField] private UITextConfig textConfig;

        private Action onConfirmCallback;

        protected override void Awake()
        {
            base.Awake();

            if (confirmButton != null)
                confirmButton.onClick.AddListener(OnConfirmClicked);
        }

        public void Show(IReadOnlyList<CollectedReward> rewards, Action onConfirm)
        {
            onConfirmCallback = onConfirm;
            BeginShow();

            if (titleText != null)
                titleText.text = textConfig != null ? textConfig.CollectTitle : DefaultTitle;

            if (rewardSummary != null)
                rewardSummary.Refresh(rewards);

            ShowSequence.Append(CanvasGroup.DOFade(1f, 0.3f));

            RectTransform content = Content;
            if (content != null)
            {
                content.localScale = Vector3.one * 0.5f;
                ShowSequence.Join(content.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
            }

            if (titleText != null)
            {
                ShowSequence.Append(
                    titleText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 6, 0.5f)
                );
            }
        }

        public override void Hide()
        {
            base.Hide();
            onConfirmCallback = null;

            if (rewardSummary != null)
                rewardSummary.Clear();
        }

        private void OnConfirmClicked()
        {
            onConfirmCallback?.Invoke();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (confirmButton == null)
                confirmButton = GetComponentInChildren<Button>(true);
        }
#endif
    }
}
