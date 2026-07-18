using System.Collections.Generic;
using UnityEngine;

namespace VertigoCase.Data
{
    public enum ZoneType
    {
        Normal,
        Safe,
        Super
    }

    [System.Serializable]
    public class WheelSliceConfig
    {
        [SerializeField] private RewardData reward;
        [SerializeField, Min(0)] private int amount = 1;
        [SerializeField] private bool isBomb;
        [SerializeField] private Sprite iconOverride;
        [SerializeField] private Color colorOverride = Color.clear;

        public RewardData Reward => reward;
        public int Amount => amount;
        public bool IsBomb => isBomb;
        public Sprite Icon => iconOverride != null ? iconOverride : reward != null ? reward.Icon : null;
        public Color Color => colorOverride.a > 0f ? colorOverride : reward != null ? reward.SliceColor : Color.white;
    }

    [CreateAssetMenu(fileName = "WheelConfig_", menuName = "Vertigo Case/Wheel Config")]
    public class WheelConfig : ScriptableObject
    {
        [SerializeField] private string displayName = "Normal Wheel";
        [SerializeField] private ZoneType zoneType = ZoneType.Normal;
        [SerializeField] private Sprite wheelSprite;
        [SerializeField] private Sprite pointerSprite;
        [SerializeField, Min(1)] private int rewardMultiplier = 1;
        [SerializeField, Min(1f)] private float spinDuration = 3f;
        [SerializeField, Min(1)] private int extraRotations = 5;
        [SerializeField] private List<WheelSliceConfig> slices = new List<WheelSliceConfig>();

        public string DisplayName => displayName;
        public ZoneType ZoneType => zoneType;
        public Sprite WheelSprite => wheelSprite;
        public Sprite PointerSprite => pointerSprite;
        public int RewardMultiplier => rewardMultiplier;
        public float SpinDuration => spinDuration;
        public int ExtraRotations => extraRotations;
        public IReadOnlyList<WheelSliceConfig> Slices => slices;
        public int SliceCount => slices.Count;
        public float SliceAngle => slices.Count == 0 ? 0f : 360f / slices.Count;

        public bool HasBomb
        {
            get
            {
                for (int i = 0; i < slices.Count; i++)
                {
                    if (slices[i].IsBomb)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (zoneType == ZoneType.Normal && slices.Count > 0 && !HasBomb)
            {
                Debug.LogWarning($"{name} is a normal wheel but has no bomb slice.", this);
            }

            if (zoneType != ZoneType.Normal && HasBomb)
            {
                Debug.LogWarning($"{name} is a safe/super wheel but contains a bomb slice.", this);
            }

            for (int i = 0; i < slices.Count; i++)
            {
                if (!slices[i].IsBomb && slices[i].Amount < 1)
                {
                    Debug.LogWarning($"{name} slice {i} is not a bomb but has amount {slices[i].Amount}; it would award nothing.", this);
                }
            }
        }
#endif
    }
}
