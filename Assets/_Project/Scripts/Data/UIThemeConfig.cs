using UnityEngine;

namespace VertigoCase.Data
{
    /// <summary>
    /// Per-zone background palette, previously hard-coded fields on UIManager.
    /// </summary>
    [CreateAssetMenu(fileName = "UIThemeConfig", menuName = "Vertigo Case/UI Theme Config")]
    public class UIThemeConfig : ScriptableObject
    {
        [SerializeField] private Color normalBackground = new Color(0.025f, 0.012f, 0.04f);
        [SerializeField] private Color safeBackground = new Color(0.015f, 0.045f, 0.035f);
        [SerializeField] private Color superBackground = new Color(0.1f, 0.065f, 0.012f);
        [SerializeField] private Color bombFlash = new Color(0.4f, 0f, 0f);

        public Color BombFlash => bombFlash;

        public Color GetBackgroundColor(ZoneType zoneType)
        {
            switch (zoneType)
            {
                case ZoneType.Safe:
                    return safeBackground;
                case ZoneType.Super:
                    return superBackground;
                default:
                    return normalBackground;
            }
        }
    }
}
