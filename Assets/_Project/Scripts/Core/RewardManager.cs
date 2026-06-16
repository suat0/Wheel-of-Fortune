using System.Collections.Generic;
using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.Core
{
    public class RewardManager : MonoBehaviour, IRewardManager
    {
        private readonly List<CollectedReward> collectedRewards = new List<CollectedReward>();

        public int UniqueRewardCount => collectedRewards.Count;
        public int TotalAmount { get; private set; }
        public bool HasRewards => collectedRewards.Count > 0;

        public void AddReward(RewardData reward, int amount)
        {
            if (reward == null)
            {
                Debug.LogWarning("[RewardManager] Tried to add a null reward.", this);
                return;
            }

            if (amount <= 0)
            {
                Debug.LogWarning($"[RewardManager] Tried to add {amount} of {reward.name}.", this);
                return;
            }

            CollectedReward existingReward = FindReward(reward);
            if (existingReward != null)
            {
                existingReward.AddAmount(amount);
            }
            else
            {
                collectedRewards.Add(new CollectedReward(reward, amount));
            }

            TotalAmount += amount;
            Debug.Log($"[RewardManager] Added {amount}x {reward.DisplayName}. TotalAmount: {TotalAmount}", this);
        }

        public void ClearRewards()
        {
            collectedRewards.Clear();
            TotalAmount = 0;
            Debug.Log("[RewardManager] Rewards cleared.", this);
        }

        public IReadOnlyList<CollectedReward> GetCollectedRewards()
        {
            return collectedRewards;
        }

        private CollectedReward FindReward(RewardData reward)
        {
            for (int i = 0; i < collectedRewards.Count; i++)
            {
                if (collectedRewards[i].Reward == reward)
                {
                    return collectedRewards[i];
                }
            }

            return null;
        }
    }
}
