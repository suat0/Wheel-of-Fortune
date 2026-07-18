using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace VertigoCase.UI
{
    public class SpinButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;

        public event Action Clicked;

        private Tween pulseTween;

        private Button Button
        {
            get
            {
                if (button == null)
                    button = GetComponent<Button>();
                return button;
            }
        }

        private void Awake()
        {
            if (Button != null)
                Button.onClick.AddListener(HandleClicked);
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(HandleClicked);
        }

        private void OnDisable()
        {
            KillPulse();
        }

        public void SetInteractable(bool interactable)
        {
            if (Button == null)
                return;

            button.interactable = interactable;
            KillPulse();

            if (interactable)
            {
                pulseTween = button.transform
                    .DOScale(1.06f, 0.6f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .SetLink(button.gameObject);
            }
        }

        public void PlayPressFeedback()
        {
            if (Button == null)
                return;

            button.transform.DOKill();
            button.transform.localScale = Vector3.one;
            button.transform.DOScale(0.9f, 0.15f).SetEase(Ease.InBack)
                .OnComplete(() => button.transform
                    .DOScale(1f, 0.1f)
                    .SetLink(button.gameObject))
                .SetLink(button.gameObject);
        }

        private void HandleClicked()
        {
            Clicked?.Invoke();
        }

        private void KillPulse()
        {
            if (pulseTween != null && pulseTween.IsActive())
            {
                pulseTween.Kill();
                pulseTween = null;
            }

            if (button != null)
                button.transform.localScale = Vector3.one;
        }
    }
}
