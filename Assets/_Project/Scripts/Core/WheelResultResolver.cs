using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.Core
{
    public static class WheelResultResolver
    {
        public static int GetRandomSliceIndex(WheelConfig wheelConfig)
        {
            if (wheelConfig == null || wheelConfig.SliceCount == 0)
            {
                return -1;
            }

            return Random.Range(0, wheelConfig.SliceCount);
        }

        public static bool TryResolve(
            WheelConfig wheelConfig,
            int zoneRewardMultiplier,
            int sliceIndex,
            out WheelSpinResult result)
        {
            result = default;

            if (wheelConfig == null)
            {
                Debug.LogError("[WheelResultResolver] WheelConfig is null.");
                return false;
            }

            if (sliceIndex < 0 || sliceIndex >= wheelConfig.SliceCount)
            {
                Debug.LogError($"[WheelResultResolver] Slice index {sliceIndex} is out of range.");
                return false;
            }

            WheelSliceConfig slice = wheelConfig.Slices[sliceIndex];
            int safeZoneMultiplier = Mathf.Max(1, zoneRewardMultiplier);
            int wheelMultiplier = Mathf.Max(1, wheelConfig.RewardMultiplier);
            int amount = slice.IsBomb ? 0 : slice.Amount * safeZoneMultiplier * wheelMultiplier;

            result = new WheelSpinResult(
                sliceIndex,
                slice.IsBomb,
                slice.Reward,
                amount,
                safeZoneMultiplier,
                wheelMultiplier
            );

            return true;
        }

        public static bool TryResolveRandom(
            WheelConfig wheelConfig,
            int zoneRewardMultiplier,
            out WheelSpinResult result)
        {
            int sliceIndex = GetRandomSliceIndex(wheelConfig);
            return TryResolve(wheelConfig, zoneRewardMultiplier, sliceIndex, out result);
        }
    }
}
