using System;
using VertigoCase.Data;

namespace VertigoCase.Core
{
    public interface IWheelController
    {
        bool IsSpinning { get; }
        bool IsTransitioning { get; }
        WheelSpinResult LastResult { get; }

        void Configure(WheelConfig wheelConfig, int zoneRewardMultiplier);
        void ConfigureWithTransition(WheelConfig wheelConfig, int zoneRewardMultiplier);
        void Spin(Action<WheelSpinResult> onCompleted);
    }
}
