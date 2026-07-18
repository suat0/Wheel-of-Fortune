using UnityEngine;
using DG.Tweening;

namespace VertigoCase.UI
{
    /// <summary>
    /// Shared plumbing for the full-screen result panels (game over / collect):
    /// CanvasGroup fade-in, an explicit content root instead of the fragile
    /// GetChild(0) convention, and show-sequence lifetime management.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class AnimatedPanelView : MonoBehaviour
    {
        [SerializeField] private RectTransform content;

        protected CanvasGroup CanvasGroup { get; private set; }
        protected Sequence ShowSequence { get; private set; }

        protected RectTransform Content
        {
            get
            {
                if (content != null)
                    return content;
                return transform.childCount > 0 ? transform.GetChild(0) as RectTransform : null;
            }
        }

        protected virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Activates the panel and starts a fresh show sequence for the derived
        /// class to append to.
        /// </summary>
        protected void BeginShow()
        {
            KillSequence();
            gameObject.SetActive(true);
            CanvasGroup.alpha = 0f;
            ShowSequence = DOTween.Sequence().SetLink(gameObject);
        }

        public virtual void Hide()
        {
            KillSequence();
            gameObject.SetActive(false);
        }

        protected void KillSequence()
        {
            if (ShowSequence != null && ShowSequence.IsActive())
                ShowSequence.Kill(true);

            ShowSequence = null;
        }

        protected virtual void OnDestroy()
        {
            KillSequence();
        }
    }
}
