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
                UIGraphicUtility.MakeNonInteractive(ui_image_reward_icon_value);
            }

            if (ui_text_reward_name_value != null)
            {
                ui_text_reward_name_value.text = reward.Reward.DisplayName;
                UIGraphicUtility.MakeNonInteractive(ui_text_reward_name_value);
            }

            if (ui_text_reward_amount_value != null)
            {
                ui_text_reward_amount_value.text = $"x{reward.Amount}";
                UIGraphicUtility.MakeNonInteractive(ui_text_reward_amount_value);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ui_image_reward_icon_value == null)
                ui_image_reward_icon_value = GetComponentInChildren<Image>();

            UIGraphicUtility.MakeNonInteractive(ui_image_reward_icon_value);
            UIGraphicUtility.MakeNonInteractive(ui_text_reward_name_value);
            UIGraphicUtility.MakeNonInteractive(ui_text_reward_amount_value);
        }
#endif
    }
}
