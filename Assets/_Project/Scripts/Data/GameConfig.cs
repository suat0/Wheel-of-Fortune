using System.Collections.Generic;
using UnityEngine;

namespace VertigoCase.Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Vertigo Case/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Zone Rules")]
        [SerializeField, Min(1)] private int safeZoneInterval = 5;
        [SerializeField, Min(1)] private int superZoneInterval = 30;

        [Header("Wheel Configs")]
        [SerializeField] private List<WheelConfig> normalWheelConfigs = new List<WheelConfig>();
        [SerializeField] private WheelConfig safeWheelConfig;
        [SerializeField] private WheelConfig superWheelConfig;

        [Header("Reward Scaling")]
        [SerializeField, Min(1)] private int rewardScaleInterval = 5;
        [SerializeField, Min(1f)] private float rewardScaleMultiplier = 1.5f;

        public int SafeZoneInterval => safeZoneInterval;
        public int SuperZoneInterval => superZoneInterval;
        public IReadOnlyList<WheelConfig> NormalWheelConfigs => normalWheelConfigs;
        public WheelConfig SafeWheelConfig => safeWheelConfig;
        public WheelConfig SuperWheelConfig => superWheelConfig;
        public int RewardScaleInterval => rewardScaleInterval;
        public float RewardScaleMultiplier => rewardScaleMultiplier;

        public ZoneType GetZoneType(int zoneNumber)
        {
            if (zoneNumber <= 0)
            {
                return ZoneType.Normal;
            }

            if (zoneNumber % superZoneInterval == 0)
            {
                return ZoneType.Super;
            }

            if (zoneNumber % safeZoneInterval == 0)
            {
                return ZoneType.Safe;
            }

            return ZoneType.Normal;
        }

        public WheelConfig GetWheelConfig(int zoneNumber)
        {
            ZoneType zoneType = GetZoneType(zoneNumber);

            if (zoneType == ZoneType.Super)
            {
                return superWheelConfig;
            }

            if (zoneType == ZoneType.Safe)
            {
                return safeWheelConfig;
            }

            if (normalWheelConfigs == null || normalWheelConfigs.Count == 0)
            {
                return null;
            }

            int index = (zoneNumber - 1) % normalWheelConfigs.Count;
            return normalWheelConfigs[Mathf.Max(0, index)];
        }

        public int GetZoneRewardMultiplier(int zoneNumber)
        {
            if (zoneNumber <= 0)
            {
                return 1;
            }

            int tier = (zoneNumber - 1) / rewardScaleInterval;
            return Mathf.Max(1, Mathf.RoundToInt(Mathf.Pow(rewardScaleMultiplier, tier)));
        }

        public bool CanCollectAtZone(int zoneNumber)
        {
            ZoneType zoneType = GetZoneType(zoneNumber);
            return zoneType == ZoneType.Safe || zoneType == ZoneType.Super;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (superZoneInterval % safeZoneInterval != 0)
            {
                Debug.LogWarning("Super zone interval should be a multiple of safe zone interval.", this);
            }
        }
#endif
    }
}
