using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VertigoCase.Core;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    public class RewardDisplayView : MonoBehaviour
    {
        [SerializeField] private RewardDisplayItemView itemPrefab;
        [SerializeField] private RectTransform itemContainer;

        // Pooled items: reused between refreshes, extras are deactivated.
        private readonly List<RewardDisplayItemView> itemPool = new List<RewardDisplayItemView>();

        /// <summary>
        /// Rebuilds the list. When highlightReward is set, the entry for that
        /// reward gets the pop-in animation — not blindly the last entry, since
        /// stacking onto an existing reward updates it in place.
        /// </summary>
        public void Refresh(IReadOnlyList<CollectedReward> rewards, RewardData highlightReward = null)
        {
            if (itemPrefab == null || itemContainer == null)
                return;

            int needed = rewards != null ? rewards.Count : 0;

            while (itemPool.Count < needed)
                itemPool.Add(Instantiate(itemPrefab, itemContainer));

            for (int i = 0; i < itemPool.Count; i++)
            {
                RewardDisplayItemView item = itemPool[i];
                if (item == null)
                    continue;

                item.transform.DOKill();
                item.transform.localScale = Vector3.one;

                bool inUse = i < needed;
                item.gameObject.SetActive(inUse);
                if (!inUse)
                    continue;

                item.Setup(rewards[i]);

                if (highlightReward != null && rewards[i].Reward == highlightReward)
                {
                    item.transform.localScale = Vector3.zero;
                    item.transform.DOScale(Vector3.one, 0.3f)
                        .SetEase(Ease.OutBack)
                        .SetLink(item.gameObject);
                }
            }
        }

        public void PlayGainFeedback()
        {
            transform.DOKill();
            transform.localScale = Vector3.one;
            transform.DOPunchScale(Vector3.one * 0.1f, 0.25f, 5, 0.4f)
                .SetLink(gameObject);
        }

        public void Clear()
        {
            for (int i = 0; i < itemPool.Count; i++)
            {
                if (itemPool[i] == null)
                    continue;

                itemPool[i].transform.DOKill();
                itemPool[i].gameObject.SetActive(false);
            }
        }
    }
}
