using UnityEngine;

namespace VertigoCase.Data
{
    /// <summary>
    /// User-facing strings, previously hard-coded in the panel views. Keeping
    /// them in an asset lets copy be edited (and later localized) without code
    /// changes. Views fall back to built-in defaults when no asset is assigned.
    /// </summary>
    [CreateAssetMenu(fileName = "UITextConfig", menuName = "Vertigo Case/UI Text Config")]
    public class UITextConfig : ScriptableObject
    {
        [SerializeField, TextArea] private string gameOverTitle = "OH NO, A BOMB EXPLODED RIGHT IN YOUR HANDS!";
        [SerializeField] private string collectTitle = "COLLECTED!";
        [SerializeField, Tooltip("{0} is replaced with the combined reward multiplier.")]
        private string rewardHintFormat = "Up To x{0} Rewards";

        public string GameOverTitle => gameOverTitle;
        public string CollectTitle => collectTitle;
        public string RewardHintFormat => rewardHintFormat;
    }
}
