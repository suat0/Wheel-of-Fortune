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
                ui_image_slice_bg.raycastTarget = false;
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
                    ui_image_slice_icon_value.raycastTarget = false;
                    ui_image_slice_icon_value.maskable = false;
                }
            }

            if (ui_text_slice_amount_value != null)
            {
                ui_text_slice_amount_value.text = sliceConfig.IsBomb ? string.Empty : $"x{displayAmount}";
                ui_text_slice_amount_value.gameObject.SetActive(!sliceConfig.IsBomb);
                ui_text_slice_amount_value.raycastTarget = false;
                ui_text_slice_amount_value.maskable = false;
            }

            if (ui_container_bomb_value != null)
                ui_container_bomb_value.SetActive(sliceConfig.IsBomb);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            AutoDisableImage(ui_image_slice_bg);
            AutoDisableImage(ui_image_slice_icon_value);
            AutoDisableText(ui_text_slice_amount_value);
        }

        private static void AutoDisableImage(Image image)
        {
            if (image == null)
            {
                return;
            }

            image.raycastTarget = false;
            image.maskable = false;
        }

        private static void AutoDisableText(TextMeshProUGUI text)
        {
            if (text == null)
            {
                return;
            }

            text.raycastTarget = false;
            text.maskable = false;
        }
#endif
    }
}
