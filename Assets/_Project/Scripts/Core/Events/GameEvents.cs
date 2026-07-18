using System.Collections.Generic;
using VertigoCase.Data;

namespace VertigoCase.Core.Events
{
    public readonly struct ZoneChangedEvent
    {
        public ZoneChangedEvent(int zone, ZoneType zoneType)
        {
            Zone = zone;
            ZoneType = zoneType;
        }

        public int Zone { get; }
        public ZoneType ZoneType { get; }
    }

    public readonly struct SpinStartedEvent
    {
    }

    public readonly struct SpinCompletedEvent
    {
        public SpinCompletedEvent(WheelSpinResult result)
        {
            Result = result;
        }

        public WheelSpinResult Result { get; }
    }

    public readonly struct BombHitEvent
    {
    }

    public readonly struct RewardsCollectedEvent
    {
        public RewardsCollectedEvent(IReadOnlyList<CollectedReward> rewards)
        {
            Rewards = rewards;
        }

        /// <summary>Snapshot taken at collect time; safe to hold after ClearRewards.</summary>
        public IReadOnlyList<CollectedReward> Rewards { get; }
    }

    public readonly struct GameRestartedEvent
    {
    }
}
