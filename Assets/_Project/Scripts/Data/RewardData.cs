using UnityEngine;

namespace VertigoCase.Data
{
    [CreateAssetMenu(fileName = "RewardData_", menuName = "Vertigo Case/Reward Data")]
    public class RewardData : ScriptableObject
    {
        [SerializeField] private string rewardId;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [SerializeField] private Color sliceColor = Color.white;

        public string RewardId => rewardId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public Color SliceColor => sliceColor;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(rewardId))
            {
                rewardId = name;
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = name.Replace("RewardData_", string.Empty).Replace('_', ' ');
            }
        }
#endif
    }
}
