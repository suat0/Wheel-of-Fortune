using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    /// <summary>
    /// One milestone card (safe or super target). Replaces the two duplicated
    /// safe/super blocks that previously lived in UIManager — the same component
    /// is used for both cards, configured via targetType and colors.
    /// </summary>
    public class ZoneTargetCardView : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private ZoneType targetType = ZoneType.Safe;
        [SerializeField] private Color activeColor = new Color(0.12f, 0.82f, 0.08f, 0.95f);
        [SerializeField] private Color inactiveColor = new Color(0.05f, 0.43f, 0.04f, 0.86f);
        [SerializeField, Min(0f)] private float punchScale = 0.15f;

        public void UpdateCard(int currentZone, int interval, ZoneType currentZoneType)
        {
            if (valueText != null)
                valueText.text = GetCurrentOrNextMilestone(Mathf.Max(1, currentZone), interval).ToString();

            bool isActive = currentZoneType == targetType;

            if (background != null)
                background.color = isActive ? activeColor : inactiveColor;

            if (isActive)
            {
                transform.DOKill();
                transform.localScale = Vector3.one;
                transform.DOPunchScale(Vector3.one * punchScale, 0.4f, 6, 0.4f)
                    .SetLink(gameObject);
            }
        }

        private static int GetCurrentOrNextMilestone(int currentZone, int interval)
        {
            interval = Mathf.Max(1, interval);
            int remainder = currentZone % interval;
            return remainder == 0 ? currentZone : currentZone + interval - remainder;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (background == null)
                background = GetComponent<Image>();
            if (valueText == null)
                valueText = GetComponentInChildren<TextMeshProUGUI>(true);
        }
#endif
    }
}
