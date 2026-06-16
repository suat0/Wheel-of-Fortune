using System;
using VertigoCase.Data;

namespace VertigoCase.Core
{
    [Serializable]
    public class CollectedReward
    {
        public CollectedReward(RewardData reward, int amount)
        {
            Reward = reward;
            Amount = amount;
        }

        public RewardData Reward { get; }
        public int Amount { get; private set; }

        public void AddAmount(int amount)
        {
            Amount += amount;
        }
    }
}
