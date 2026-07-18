using TMPro;
using UnityEngine;
using DG.Tweening;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    public class RewardHintView : MonoBehaviour
    {
        private const string DefaultFormat = "Up To x{0} Rewards";

        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private UITextConfig textConfig;

        /// <summary>
        /// Shows the combined (zone x wheel) multiplier so the hint matches what
        /// the player would actually receive.
        /// </summary>
        public void Show(int totalMultiplier)
        {
            if (hintText == null)
                return;

            string format = textConfig != null ? textConfig.RewardHintFormat : DefaultFormat;
            hintText.text = string.Format(format, Mathf.Max(1, totalMultiplier));

            // Kill on the TMP component itself: DOFade targets the text, not its
            // transform, so the previous transform.DOKill() let fades stack up.
            hintText.DOKill();
            hintText.alpha = 0f;
            hintText.DOFade(1f, 0.4f)
                .SetDelay(0.2f)
                .SetLink(hintText.gameObject);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (hintText == null)
                hintText = GetComponent<TextMeshProUGUI>();
        }
#endif
    }
}
