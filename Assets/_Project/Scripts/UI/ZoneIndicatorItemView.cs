using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    public class ZoneIndicatorItemView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Image ui_image_zone_bg_value;
        [SerializeField] private TextMeshProUGUI ui_text_zone_number_value;

        [Header("Zone Sprites")]
        [SerializeField] private Sprite spriteZoneComing;
        [SerializeField] private Sprite spriteZoneCurrent;
        [SerializeField] private Sprite spriteZonePassed;
        [SerializeField] private Sprite spriteZoneSuper;

        [Header("Zone Colors")]
        [SerializeField] private Color colorNormal = new Color(0.6f, 0.6f, 0.6f);
        [SerializeField] private Color colorSafe = new Color(0.75f, 0.85f, 0.95f);
        [SerializeField] private Color colorSuper = new Color(1f, 0.85f, 0.4f);
        [SerializeField] private Color colorCurrent = Color.white;
        [SerializeField] private Color colorPassed = new Color(0.4f, 0.4f, 0.4f);

        public void Setup(int zoneNumber, ZoneType zoneType, bool isCurrent, bool isPassed)
        {
            if (ui_text_zone_number_value != null)
            {
                ui_text_zone_number_value.text = zoneNumber.ToString();
                UIGraphicUtility.MakeNonInteractive(ui_text_zone_number_value);
            }

            if (ui_image_zone_bg_value == null)
                return;

            UIGraphicUtility.MakeNonInteractive(ui_image_zone_bg_value);

            if (isCurrent)
            {
                SetVisual(spriteZoneCurrent, colorCurrent);
            }
            else if (isPassed)
            {
                SetVisual(spriteZonePassed, colorPassed);
            }
            else if (zoneType == ZoneType.Super)
            {
                SetVisual(spriteZoneSuper, colorSuper);
            }
            else if (zoneType == ZoneType.Safe)
            {
                SetVisual(spriteZoneComing, colorSafe);
            }
            else
            {
                SetVisual(spriteZoneComing, colorNormal);
            }
        }

        private void SetVisual(Sprite sprite, Color color)
        {
            if (sprite != null)
                ui_image_zone_bg_value.sprite = sprite;

            ui_image_zone_bg_value.color = color;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_image_zone_bg_value == null)
                ui_image_zone_bg_value = GetComponentInChildren<Image>();
            if (ui_text_zone_number_value == null)
                ui_text_zone_number_value = GetComponentInChildren<TextMeshProUGUI>();

            UIGraphicUtility.MakeNonInteractive(ui_image_zone_bg_value);
            UIGraphicUtility.MakeNonInteractive(ui_text_zone_number_value);
        }
#endif
    }
}
