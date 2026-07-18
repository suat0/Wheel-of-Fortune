using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.Core
{
    public class RewardManager : MonoBehaviour, IRewardManager
    {
        private readonly List<CollectedReward> collectedRewards = new List<CollectedReward>();
        private ReadOnlyCollection<CollectedReward> readOnlyRewards;

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

        /// <summary>
        /// Live view of the current rewards. Wrapped in ReadOnlyCollection so a
        /// caller cannot cast it back to List and mutate internal state. Note it
        /// is still a live view: ClearRewards empties it. Use GetRewardsSnapshot
        /// for data that must survive a clear (e.g. event payloads).
        /// </summary>
        public IReadOnlyList<CollectedReward> GetCollectedRewards()
        {
            if (readOnlyRewards == null)
                readOnlyRewards = collectedRewards.AsReadOnly();

            return readOnlyRewards;
        }

        public IReadOnlyList<CollectedReward> GetRewardsSnapshot()
        {
            return collectedRewards.ToArray();
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
