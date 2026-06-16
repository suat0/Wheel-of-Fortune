using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoCase.Core;
using VertigoCase.UI;

namespace VertigoCase.EditorTools
{
    public static class UIManagerSetupEditor
    {
        [MenuItem("Vertigo Case/Wire UIManager References")]
        private static void WireReferences()
        {
            UIManager uiManager = Object.FindObjectOfType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("[UIManagerSetup] No UIManager found in scene.");
                return;
            }

            SerializedObject so = new SerializedObject(uiManager);

            SetRefIfNull(so, "gameManager", Object.FindObjectOfType<GameManager>());
            SetRefIfNull(so, "ui_panel_gameover", Object.FindObjectOfType<GameOverPanelView>(true));
            SetRefIfNull(so, "ui_panel_collect", Object.FindObjectOfType<CollectPanelView>(true));
            SetRefIfNull(so, "ui_image_spin_base_value", FindImageByName("ui_image_spin_base_value"));
            SetRefIfNull(so, "ui_image_spin_indicator_value", FindImageByName("ui_image_spin_indicator_value"));
            SetRefIfNull(so, "wheelBackplateImage", FindImageByName("ui_fx_wheel_backplate"));
            SetRefIfNull(so, "ui_button_spin", FindComponentByName<Button>("ui_button_spin"));
            SetRefIfNull(so, "ui_button_collect", FindComponentByName<Button>("ui_button_collect"));
            SetRefIfNull(so, "zoneIndicator", Object.FindObjectOfType<ZoneIndicatorView>(true));
            SetRefIfNull(so, "ui_text_reward_hint_value", FindComponentByName<TextMeshProUGUI>("ui_text_reward_hint_value"));
            SetRefIfNull(so, "ui_image_background", FindImageByName("ui_image_background"));
            SetRefIfNull(so, "safeTargetBg", FindImageByName("ui_card_safe_zone_target"));
            SetRefIfNull(so, "superTargetBg", FindImageByName("ui_card_super_zone_target"));
            SetRefIfNull(so, "safeTargetValueText", FindComponentByName<TextMeshProUGUI>("ui_text_safe_zone_value"));
            SetRefIfNull(so, "superTargetValueText", FindComponentByName<TextMeshProUGUI>("ui_text_super_zone_value"));

            RectTransform rewardsContainer = FindByName<RectTransform>("ui_container_rewards");
            if (rewardsContainer != null)
                SetRefIfNull(so, "rewardDisplay", rewardsContainer.GetComponent<RewardDisplayView>());

            so.ApplyModifiedProperties();
            Debug.Log("[UIManagerSetup] All references wired successfully.");
        }

        private static void SetRefIfNull(SerializedObject so, string propertyName, Object value)
        {
            SerializedProperty prop = so.FindProperty(propertyName);
            if (prop == null)
                return;

            if (prop.objectReferenceValue == null && value != null)
                prop.objectReferenceValue = value;
        }

        private static Image FindImageByName(string name)
        {
            return FindComponentByName<Image>(name);
        }

        private static T FindComponentByName<T>(string name) where T : Component
        {
            T[] all = Object.FindObjectsOfType<T>(true);
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].gameObject.name == name)
                    return all[i];
            }
            return null;
        }

        private static T FindByName<T>(string name) where T : Component
        {
            return FindComponentByName<T>(name);
        }
    }
}
