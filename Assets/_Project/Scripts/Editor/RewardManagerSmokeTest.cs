using UnityEditor;
using UnityEngine;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.EditorTools
{
    public static class RewardManagerSmokeTest
    {
        [MenuItem("Vertigo Case/Tests/Run Reward Manager Smoke Test")]
        private static void Run()
        {
            GameObject testObject = new GameObject("RewardManagerSmokeTest");
            RewardManager rewardManager = testObject.AddComponent<RewardManager>();
            RewardData reward = ScriptableObject.CreateInstance<RewardData>();

            rewardManager.AddReward(reward, 3);
            rewardManager.AddReward(reward, 2);

            bool passed = rewardManager.UniqueRewardCount == 1 &&
                          rewardManager.TotalAmount == 5 &&
                          rewardManager.HasRewards;

            rewardManager.ClearRewards();
            passed &= rewardManager.UniqueRewardCount == 0 &&
                      rewardManager.TotalAmount == 0 &&
                      !rewardManager.HasRewards;

            Object.DestroyImmediate(reward);
            Object.DestroyImmediate(testObject);

            if (passed)
            {
                Debug.Log("[RewardManagerSmokeTest] Passed. Rewards stack and clear correctly.");
            }
            else
            {
                Debug.LogError("[RewardManagerSmokeTest] Failed. Check reward stacking/clearing.");
            }
        }
    }
}
