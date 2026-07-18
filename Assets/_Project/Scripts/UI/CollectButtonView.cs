using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace VertigoCase.UI
{
    public class CollectButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;

        public event Action Clicked;

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

        public void SetVisible(bool visible)
        {
            if (Button == null)
                return;

            if (visible)
            {
                button.gameObject.SetActive(true);
                button.interactable = true;
                button.transform.DOKill();
                button.transform.localScale = Vector3.zero;
                button.transform.DOScale(1f, 0.3f)
                    .SetEase(Ease.OutBack)
                    .SetLink(button.gameObject);
            }
            else
            {
                button.transform.DOKill();
                button.transform.localScale = Vector3.one;
                button.gameObject.SetActive(false);
                button.interactable = false;
            }
        }

        private void HandleClicked()
        {
            Clicked?.Invoke();
        }
    }
}
