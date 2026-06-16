using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoCase.Core;

namespace VertigoCase.UI
{
    public class RewardDisplayItemView : MonoBehaviour
    {
        [SerializeField] private Image ui_image_reward_icon_value;
        [SerializeField] private TextMeshProUGUI ui_text_reward_name_value;
        [SerializeField] private TextMeshProUGUI ui_text_reward_amount_value;

        public void Setup(CollectedReward reward)
        {
            if (reward == null || reward.Reward == null)
                return;

            if (ui_image_reward_icon_value != null)
            {
                ui_image_reward_icon_value.sprite = reward.Reward.Icon;
                ui_image_reward_icon_value.enabled = reward.Reward.Icon != null;
                ui_image_reward_icon_value.preserveAspect = true;
                ui_image_reward_icon_value.raycastTarget = false;
                ui_image_reward_icon_value.maskable = false;
            }

            if (ui_text_reward_name_value != null)
            {
                ui_text_reward_name_value.text = reward.Reward.DisplayName;
                ui_text_reward_name_value.raycastTarget = false;
                ui_text_reward_name_value.maskable = false;
            }

            if (ui_text_reward_amount_value != null)
            {
                ui_text_reward_amount_value.text = $"x{reward.Amount}";
                ui_text_reward_amount_value.raycastTarget = false;
                ui_text_reward_amount_value.maskable = false;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_image_reward_icon_value == null)
                ui_image_reward_icon_value = GetComponentInChildren<Image>();

            AutoDisableImage(ui_image_reward_icon_value);
            AutoDisableText(ui_text_reward_name_value);
            AutoDisableText(ui_text_reward_amount_value);
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
