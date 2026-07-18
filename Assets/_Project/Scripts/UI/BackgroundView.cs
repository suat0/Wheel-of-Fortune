using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    /// <summary>
    /// Zone-tinted background. Colors come from a UIThemeConfig asset; the
    /// built-in fallbacks only exist so an unwired scene still renders sanely.
    /// </summary>
    public class BackgroundView : MonoBehaviour
    {
        private static readonly Color FallbackNormal = new Color(0.025f, 0.012f, 0.04f);
        private static readonly Color FallbackSafe = new Color(0.015f, 0.045f, 0.035f);
        private static readonly Color FallbackSuper = new Color(0.1f, 0.065f, 0.012f);
        private static readonly Color FallbackFlash = new Color(0.4f, 0f, 0f);

        [SerializeField] private Image background;
        [SerializeField] private UIThemeConfig theme;

        private ZoneType currentZoneType = ZoneType.Normal;

        public void ApplyZone(ZoneType zoneType)
        {
            currentZoneType = zoneType;

            if (background == null)
                return;

            background.DOKill();
            background.DOColor(GetZoneColor(zoneType), 0.5f)
                .SetEase(Ease.OutQuad)
                .SetLink(background.gameObject);
        }

        public void FlashBomb()
        {
            if (background == null)
                return;

            // Restore to the zone's target color, not to whatever color the
            // image happened to have (it could be mid-transition when the bomb
            // hits, which previously froze an in-between tint).
            Color restoreColor = GetZoneColor(currentZoneType);
            Color flashColor = theme != null ? theme.BombFlash : FallbackFlash;

            background.DOKill();
            background.DOColor(flashColor, 0.08f)
                .OnComplete(() => background
                    .DOColor(restoreColor, 0.4f)
                    .SetEase(Ease.OutQuad)
                    .SetLink(background.gameObject))
                .SetLink(background.gameObject);
        }

        private Color GetZoneColor(ZoneType zoneType)
        {
            if (theme != null)
                return theme.GetBackgroundColor(zoneType);

            switch (zoneType)
            {
                case ZoneType.Safe:
                    return FallbackSafe;
                case ZoneType.Super:
                    return FallbackSuper;
                default:
                    return FallbackNormal;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (background == null)
                background = GetComponent<Image>();
        }
#endif
    }
}
