using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.Core
{
    public static class WheelResultResolver
    {
        /// <summary>
        /// Single source of truth for the reward amount formula. Both the actual
        /// spin result and the amounts displayed on the wheel slices must go
        /// through here so they can never drift apart.
        /// </summary>
        public static int GetSliceAmount(WheelSliceConfig slice, int zoneRewardMultiplier, int wheelRewardMultiplier)
        {
            if (slice == null || slice.IsBomb)
                return 0;

            return slice.Amount * Mathf.Max(1, zoneRewardMultiplier) * Mathf.Max(1, wheelRewardMultiplier);
        }

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
            int amount = GetSliceAmount(slice, safeZoneMultiplier, wheelMultiplier);

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
