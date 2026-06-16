using System.Collections.Generic;
using VertigoCase.Data;

namespace VertigoCase.Core
{
    public interface IRewardManager
    {
        int UniqueRewardCount { get; }
        int TotalAmount { get; }
        bool HasRewards { get; }

        void AddReward(RewardData reward, int amount);
        void ClearRewards();
        IReadOnlyList<CollectedReward> GetCollectedRewards();
    }
}
