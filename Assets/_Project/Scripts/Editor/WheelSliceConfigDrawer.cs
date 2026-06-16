using UnityEditor;
using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.EditorTools
{
    [CustomPropertyDrawer(typeof(WheelSliceConfig))]
    public class WheelSliceConfigDrawer : PropertyDrawer
    {
        private const float IconSize = 32f;
        private const float ColorSize = 16f;
        private const float BombToggleWidth = 50f;
        private const float Padding = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IconSize + Padding * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty rewardProp = property.FindPropertyRelative("reward");
            SerializedProperty amountProp = property.FindPropertyRelative("amount");
            SerializedProperty isBombProp = property.FindPropertyRelative("isBomb");
            SerializedProperty iconOverrideProp = property.FindPropertyRelative("iconOverride");
            SerializedProperty colorOverrideProp = property.FindPropertyRelative("colorOverride");

            bool isBomb = isBombProp.boolValue;

            if (isBomb)
            {
                EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, position.height),
                    new Color(0.6f, 0.12f, 0.12f, 0.25f));
            }

            Rect inner = new Rect(
                position.x + Padding,
                position.y + Padding,
                position.width - Padding * 2,
                position.height - Padding * 2
            );

            float x = inner.x;

            Rect iconRect = new Rect(x, inner.y, IconSize, IconSize);
            DrawIconPreview(iconRect, rewardProp, iconOverrideProp, isBomb);
            x += IconSize + Padding;

            Rect colorRect = new Rect(x, inner.y + (IconSize - ColorSize) * 0.5f, ColorSize, ColorSize);
            Color sliceColor = GetSliceColor(rewardProp, colorOverrideProp);
            EditorGUI.DrawRect(colorRect, sliceColor);
            EditorGUI.DrawRect(new Rect(colorRect.x, colorRect.y, colorRect.width, 1), Color.black);
            EditorGUI.DrawRect(new Rect(colorRect.x, colorRect.yMax - 1, colorRect.width, 1), Color.black);
            EditorGUI.DrawRect(new Rect(colorRect.x, colorRect.y, 1, colorRect.height), Color.black);
            EditorGUI.DrawRect(new Rect(colorRect.xMax - 1, colorRect.y, 1, colorRect.height), Color.black);
            x += ColorSize + Padding;

            float bombWidth = BombToggleWidth;
            float remaining = inner.xMax - x - bombWidth - Padding;
            float rewardWidth = remaining * 0.6f;
            float amountWidth = remaining * 0.4f;

            Rect rewardRect = new Rect(x, inner.y + (IconSize - EditorGUIUtility.singleLineHeight) * 0.5f,
                rewardWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(rewardRect, rewardProp, GUIContent.none);
            x += rewardWidth + Padding;

            if (!isBomb)
            {
                Rect amountRect = new Rect(x, inner.y + (IconSize - EditorGUIUtility.singleLineHeight) * 0.5f,
                    amountWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(amountRect, amountProp, GUIContent.none);
            }
            else
            {
                Rect bombLabel = new Rect(x, inner.y + (IconSize - EditorGUIUtility.singleLineHeight) * 0.5f,
                    amountWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(bombLabel, "BOMB", EditorStyles.boldLabel);
            }
            x += amountWidth + Padding;

            Rect bombRect = new Rect(x, inner.y + (IconSize - EditorGUIUtility.singleLineHeight) * 0.5f,
                bombWidth, EditorGUIUtility.singleLineHeight);
            isBombProp.boolValue = EditorGUI.ToggleLeft(bombRect, "Bomb", isBombProp.boolValue);

            EditorGUI.EndProperty();
        }

        private void DrawIconPreview(Rect rect, SerializedProperty rewardProp, SerializedProperty iconOverrideProp, bool isBomb)
        {
            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

            if (isBomb)
            {
                GUIStyle centeredBold = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = new Color(1f, 0.3f, 0.3f) }
                };
                EditorGUI.LabelField(rect, "X", centeredBold);
                return;
            }

            Sprite icon = iconOverrideProp.objectReferenceValue as Sprite;

            if (icon == null && rewardProp.objectReferenceValue != null)
            {
                RewardData reward = rewardProp.objectReferenceValue as RewardData;
                if (reward != null)
                    icon = reward.Icon;
            }

            if (icon != null && icon.texture != null)
            {
                Rect texCoords = new Rect(
                    icon.textureRect.x / icon.texture.width,
                    icon.textureRect.y / icon.texture.height,
                    icon.textureRect.width / icon.texture.width,
                    icon.textureRect.height / icon.texture.height
                );
                GUI.DrawTextureWithTexCoords(rect, icon.texture, texCoords);
            }
        }

        private Color GetSliceColor(SerializedProperty rewardProp, SerializedProperty colorOverrideProp)
        {
            Color overrideColor = colorOverrideProp.colorValue;
            if (overrideColor.a > 0f)
                return overrideColor;

            if (rewardProp.objectReferenceValue != null)
            {
                RewardData reward = rewardProp.objectReferenceValue as RewardData;
                if (reward != null)
                    return reward.SliceColor;
            }

            return Color.white;
        }
    }
}
