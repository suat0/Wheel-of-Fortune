using System;
using VertigoCase.Data;

namespace VertigoCase.Core
{
    public interface IWheelController
    {
        bool IsSpinning { get; }
        bool IsTransitioning { get; }
        bool HasValidConfig { get; }
        bool CanSpin { get; }
        WheelSpinResult LastResult { get; }

        void Configure(WheelConfig wheelConfig, int zoneRewardMultiplier);
        void ConfigureWithTransition(WheelConfig wheelConfig, int zoneRewardMultiplier);
        bool Spin(Action<WheelSpinResult> onCompleted);
    }
}
