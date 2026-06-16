using UnityEditor;
using UnityEngine;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.EditorTools
{
    public static class WheelControllerSmokeTest
    {
        [MenuItem("Vertigo Case/Tests/Run Wheel Controller Smoke Test")]
        private static void Run()
        {
            WheelConfig wheelConfig = ScriptableObject.CreateInstance<WheelConfig>();
            ConfigureWheel(wheelConfig);

            float targetAngle = WheelController.CalculateTargetAngle(wheelConfig, 1);
            bool passed = Mathf.Approximately(targetAngle, 5 * 360f + 135f);

            Object.DestroyImmediate(wheelConfig);

            if (passed)
            {
                Debug.Log("[WheelControllerSmokeTest] Passed. Target angle calculation is correct.");
            }
            else
            {
                Debug.LogError($"[WheelControllerSmokeTest] Failed. Target angle was {targetAngle}.");
            }
        }

        private static void ConfigureWheel(WheelConfig wheelConfig)
        {
            SerializedObject wheel = new SerializedObject(wheelConfig);
            wheel.FindProperty("extraRotations").intValue = 5;

            SerializedProperty slices = wheel.FindProperty("slices");
            slices.arraySize = 4;

            for (int i = 0; i < slices.arraySize; i++)
            {
                SerializedProperty slice = slices.GetArrayElementAtIndex(i);
                slice.FindPropertyRelative("amount").intValue = 1;
                slice.FindPropertyRelative("isBomb").boolValue = i == 3;
            }

            wheel.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
