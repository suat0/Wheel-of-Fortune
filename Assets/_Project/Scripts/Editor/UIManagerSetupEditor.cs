using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VertigoCase.Core;
using VertigoCase.Data;
using VertigoCase.UI;

namespace VertigoCase.EditorTools
{
    /// <summary>
    /// One-click scene wiring for the decomposed UI: adds the sub-view
    /// components to their named GameObjects, wires their internal references,
    /// creates the UI config assets if missing, and hooks everything up to
    /// UIManager. Safe to run repeatedly — existing references are kept.
    /// </summary>
    public static class UIManagerSetupEditor
    {
        private const string TextConfigPath = "Assets/_Project/ScriptableObjects/UI/UITextConfig.asset";
        private const string ThemeConfigPath = "Assets/_Project/ScriptableObjects/UI/UIThemeConfig.asset";

        [MenuItem("Vertigo Case/Wire UIManager References")]
        private static void WireReferences()
        {
            UIManager uiManager = Object.FindObjectOfType<UIManager>(true);
            if (uiManager == null)
            {
                Debug.LogError("[UIManagerSetup] No UIManager found in scene.");
                return;
            }

            UITextConfig textConfig = EnsureAsset<UITextConfig>(TextConfigPath);
            UIThemeConfig themeConfig = EnsureAsset<UIThemeConfig>(ThemeConfigPath);

            BackgroundView background = EnsureComponent<BackgroundView>(FindGameObject("ui_image_background"));
            if (background != null)
            {
                SerializedObject so = new SerializedObject(background);
                SetRefIfNull(so, "background", background.GetComponent<Image>());
                SetRefIfNull(so, "theme", themeConfig);
                so.ApplyModifiedProperties();
            }

            SpinButtonView spinButton = EnsureComponent<SpinButtonView>(FindGameObject("ui_button_spin"));
            if (spinButton != null)
            {
                SerializedObject so = new SerializedObject(spinButton);
                SetRefIfNull(so, "button", spinButton.GetComponent<Button>());
                so.ApplyModifiedProperties();
            }

            CollectButtonView collectButton = EnsureComponent<CollectButtonView>(FindGameObject("ui_button_collect"));
            if (collectButton != null)
            {
                SerializedObject so = new SerializedObject(collectButton);
                SetRefIfNull(so, "button", collectButton.GetComponent<Button>());
                so.ApplyModifiedProperties();
            }

            RewardHintView rewardHint = EnsureComponent<RewardHintView>(FindGameObject("ui_text_reward_hint_value"));
            if (rewardHint != null)
            {
                SerializedObject so = new SerializedObject(rewardHint);
                SetRefIfNull(so, "hintText", rewardHint.GetComponent<TextMeshProUGUI>());
                SetRefIfNull(so, "textConfig", textConfig);
                so.ApplyModifiedProperties();
            }

            WheelVisualsView wheelVisuals = EnsureComponent<WheelVisualsView>(uiManager.gameObject);
            if (wheelVisuals != null)
            {
                SerializedObject so = new SerializedObject(wheelVisuals);
                SetRefIfNull(so, "wheelBaseImage", FindComponentByName<Image>("ui_image_spin_base_value"));
                SetRefIfNull(so, "pointerImage", FindComponentByName<Image>("ui_image_spin_indicator_value"));
                SetRefIfNull(so, "backplateImage", FindComponentByName<Image>("ui_fx_wheel_backplate"));
                so.ApplyModifiedProperties();
            }

            ZoneTargetCardView safeCard = EnsureComponent<ZoneTargetCardView>(FindGameObject("ui_card_safe_zone_target"));
            WireZoneCard(safeCard, "ui_text_safe_zone_value", ZoneType.Safe,
                new Color(0.12f, 0.82f, 0.08f, 0.95f), new Color(0.05f, 0.43f, 0.04f, 0.86f), 0.15f);

            ZoneTargetCardView superCard = EnsureComponent<ZoneTargetCardView>(FindGameObject("ui_card_super_zone_target"));
            WireZoneCard(superCard, "ui_text_super_zone_value", ZoneType.Super,
                new Color(0.95f, 0.62f, 0.04f, 0.96f), new Color(0.72f, 0.43f, 0.02f, 0.86f), 0.2f);

            GameOverPanelView gameOverPanel = Object.FindObjectOfType<GameOverPanelView>(true);
            if (gameOverPanel != null)
            {
                SerializedObject so = new SerializedObject(gameOverPanel);
                SetRefIfNull(so, "restartButton", gameOverPanel.GetComponentInChildren<Button>(true));
                SetRefIfNull(so, "titleText", FindComponentByName<TextMeshProUGUI>("ui_text_gameover_title_value"));
                SetRefIfNull(so, "iconImage", FindComponentByName<Image>("ui_image_gameover_icon"));
                SetRefIfNull(so, "textConfig", textConfig);
                SetRefIfNull(so, "content", FirstChildRect(gameOverPanel.transform));
                so.ApplyModifiedProperties();
            }

            CollectPanelView collectPanel = Object.FindObjectOfType<CollectPanelView>(true);
            if (collectPanel != null)
            {
                SerializedObject so = new SerializedObject(collectPanel);
                SetRefIfNull(so, "confirmButton", collectPanel.GetComponentInChildren<Button>(true));
                SetRefIfNull(so, "titleText", FindComponentByName<TextMeshProUGUI>("ui_text_collect_title_value"));
                SetRefIfNull(so, "rewardSummary", collectPanel.GetComponentInChildren<RewardDisplayView>(true));
                SetRefIfNull(so, "textConfig", textConfig);
                SetRefIfNull(so, "content", FirstChildRect(collectPanel.transform));
                so.ApplyModifiedProperties();
            }

            RewardDisplayView rewardDisplay = null;
            RectTransform rewardsContainer = FindComponentByName<RectTransform>("ui_container_rewards");
            if (rewardsContainer != null)
                rewardDisplay = rewardsContainer.GetComponent<RewardDisplayView>();

            SerializedObject managerSo = new SerializedObject(uiManager);
            SetRefIfNull(managerSo, "gameManager", Object.FindObjectOfType<GameManager>(true));
            SetRefIfNull(managerSo, "gameOverPanel", gameOverPanel);
            SetRefIfNull(managerSo, "collectPanel", collectPanel);
            SetRefIfNull(managerSo, "wheelVisuals", wheelVisuals);
            SetRefIfNull(managerSo, "spinButton", spinButton);
            SetRefIfNull(managerSo, "collectButton", collectButton);
            SetRefIfNull(managerSo, "background", background);
            SetRefIfNull(managerSo, "rewardHint", rewardHint);
            SetRefIfNull(managerSo, "zoneIndicator", Object.FindObjectOfType<ZoneIndicatorView>(true));
            SetRefIfNull(managerSo, "rewardDisplay", rewardDisplay);
            SetRefIfNull(managerSo, "safeTargetCard", safeCard);
            SetRefIfNull(managerSo, "superTargetCard", superCard);
            managerSo.ApplyModifiedProperties();

            Debug.Log("[UIManagerSetup] UI components added and references wired.");
        }

        private static void WireZoneCard(
            ZoneTargetCardView card,
            string valueTextName,
            ZoneType targetType,
            Color activeColor,
            Color inactiveColor,
            float punchScale)
        {
            if (card == null)
                return;

            SerializedObject so = new SerializedObject(card);
            SetRefIfNull(so, "background", card.GetComponent<Image>());
            SetRefIfNull(so, "valueText", FindComponentByName<TextMeshProUGUI>(valueTextName));
            so.FindProperty("targetType").enumValueIndex = (int)targetType;
            so.FindProperty("activeColor").colorValue = activeColor;
            so.FindProperty("inactiveColor").colorValue = inactiveColor;
            so.FindProperty("punchScale").floatValue = punchScale;
            so.ApplyModifiedProperties();
        }

        private static T EnsureAsset<T>(string path) where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
                return asset;

            string folder = System.IO.Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(folder) && !AssetDatabase.IsValidFolder(folder))
            {
                string parent = System.IO.Path.GetDirectoryName(folder)?.Replace('\\', '/');
                string newFolderName = System.IO.Path.GetFileName(folder);
                AssetDatabase.CreateFolder(parent, newFolderName);
            }

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            Debug.Log($"[UIManagerSetup] Created {path}");
            return asset;
        }

        private static T EnsureComponent<T>(GameObject target) where T : Component
        {
            if (target == null)
                return null;

            T component = target.GetComponent<T>();
            if (component == null)
                component = Undo.AddComponent<T>(target);

            return component;
        }

        private static GameObject FindGameObject(string name)
        {
            Transform[] all = Object.FindObjectsOfType<Transform>(true);
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].gameObject.name == name)
                    return all[i].gameObject;
            }

            return null;
        }

        private static RectTransform FirstChildRect(Transform parent)
        {
            return parent.childCount > 0 ? parent.GetChild(0) as RectTransform : null;
        }

        private static void SetRefIfNull(SerializedObject so, string propertyName, Object value)
        {
            SerializedProperty prop = so.FindProperty(propertyName);
            if (prop == null)
            {
                Debug.LogWarning($"[UIManagerSetup] Property {propertyName} not found on {so.targetObject.name}.");
                return;
            }

            if (prop.objectReferenceValue == null && value != null)
                prop.objectReferenceValue = value;
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
    }
}
