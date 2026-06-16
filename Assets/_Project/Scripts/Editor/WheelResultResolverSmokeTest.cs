using UnityEditor;
using UnityEngine;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.EditorTools
{
    public static class WheelResultResolverSmokeTest
    {
        [MenuItem("Vertigo Case/Tests/Run Wheel Result Resolver Smoke Test")]
        private static void Run()
        {
            RewardData reward = ScriptableObject.CreateInstance<RewardData>();
            WheelConfig wheelConfig = ScriptableObject.CreateInstance<WheelConfig>();

            ConfigureWheel(wheelConfig, reward);

            bool passed = WheelResultResolver.TryResolve(wheelConfig, 3, 0, out WheelSpinResult rewardResult);
            passed &= !rewardResult.IsBomb;
            passed &= rewardResult.Reward == reward;
            passed &= rewardResult.Amount == 60;

            passed &= WheelResultResolver.TryResolve(wheelConfig, 3, 1, out WheelSpinResult bombResult);
            passed &= bombResult.IsBomb;
            passed &= bombResult.Reward == null;
            passed &= bombResult.Amount == 0;

            Object.DestroyImmediate(wheelConfig);
            Object.DestroyImmediate(reward);

            if (passed)
            {
                Debug.Log("[WheelResultResolverSmokeTest] Passed. Reward and bomb results resolve correctly.");
            }
            else
            {
                Debug.LogError("[WheelResultResolverSmokeTest] Failed. Check wheel result calculation.");
            }
        }

        private static void ConfigureWheel(WheelConfig wheelConfig, RewardData reward)
        {
            SerializedObject wheel = new SerializedObject(wheelConfig);

            wheel.FindProperty("rewardMultiplier").intValue = 2;

            SerializedProperty slices = wheel.FindProperty("slices");
            slices.arraySize = 2;

            SerializedProperty rewardSlice = slices.GetArrayElementAtIndex(0);
            rewardSlice.FindPropertyRelative("reward").objectReferenceValue = reward;
            rewardSlice.FindPropertyRelative("amount").intValue = 10;
            rewardSlice.FindPropertyRelative("isBomb").boolValue = false;

            SerializedProperty bombSlice = slices.GetArrayElementAtIndex(1);
            bombSlice.FindPropertyRelative("reward").objectReferenceValue = null;
            bombSlice.FindPropertyRelative("amount").intValue = 0;
            bombSlice.FindPropertyRelative("isBomb").boolValue = true;

            wheel.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
