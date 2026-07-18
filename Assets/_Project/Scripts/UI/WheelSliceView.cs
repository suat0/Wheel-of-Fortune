using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    public class WheelSliceView : MonoBehaviour
    {
        [SerializeField] private Image ui_image_slice_bg;
        [SerializeField] private Image ui_image_slice_icon_value;
        [SerializeField] private TextMeshProUGUI ui_text_slice_amount_value;
        [SerializeField] private GameObject ui_container_bomb_value;

        public void Setup(WheelSliceConfig sliceConfig, int displayAmount, float angle)
        {
            if (sliceConfig == null)
                return;

            transform.localRotation = Quaternion.Euler(0f, 0f, -angle);

            if (ui_image_slice_bg != null)
            {
                Color sliceColor = sliceConfig.Color;
                sliceColor.a = 0.12f;
                ui_image_slice_bg.color = sliceColor;
                ui_image_slice_bg.enabled = true;
                UIGraphicUtility.MakeNonInteractive(ui_image_slice_bg);
            }

            if (ui_image_slice_icon_value != null)
            {
                if (sliceConfig.IsBomb)
                {
                    ui_image_slice_icon_value.enabled = false;
                }
                else
                {
                    ui_image_slice_icon_value.sprite = sliceConfig.Icon;
                    ui_image_slice_icon_value.enabled = sliceConfig.Icon != null;
                    ui_image_slice_icon_value.preserveAspect = true;
                    UIGraphicUtility.MakeNonInteractive(ui_image_slice_icon_value);
                }
            }

            if (ui_text_slice_amount_value != null)
            {
                ui_text_slice_amount_value.text = sliceConfig.IsBomb ? string.Empty : $"x{displayAmount}";
                ui_text_slice_amount_value.gameObject.SetActive(!sliceConfig.IsBomb);
                UIGraphicUtility.MakeNonInteractive(ui_text_slice_amount_value);
            }

            if (ui_container_bomb_value != null)
                ui_container_bomb_value.SetActive(sliceConfig.IsBomb);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UIGraphicUtility.MakeNonInteractive(ui_image_slice_bg);
            UIGraphicUtility.MakeNonInteractive(ui_image_slice_icon_value);
            UIGraphicUtility.MakeNonInteractive(ui_text_slice_amount_value);
        }
#endif
    }
}
