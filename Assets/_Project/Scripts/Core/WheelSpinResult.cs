using VertigoCase.Data;

namespace VertigoCase.Core
{
    public readonly struct WheelSpinResult
    {
        public WheelSpinResult(
            int sliceIndex,
            bool isBomb,
            RewardData reward,
            int amount,
            int zoneMultiplier,
            int wheelMultiplier)
        {
            SliceIndex = sliceIndex;
            IsBomb = isBomb;
            Reward = reward;
            Amount = amount;
            ZoneMultiplier = zoneMultiplier;
            WheelMultiplier = wheelMultiplier;
        }

        public int SliceIndex { get; }
        public bool IsBomb { get; }
        public RewardData Reward { get; }
        public int Amount { get; }
        public int ZoneMultiplier { get; }
        public int WheelMultiplier { get; }
    }
}
