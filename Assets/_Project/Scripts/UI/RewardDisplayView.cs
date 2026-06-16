using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VertigoCase.Core;

namespace VertigoCase.UI
{
    public class RewardDisplayView : MonoBehaviour
    {
        [SerializeField] private RewardDisplayItemView itemPrefab;
        [SerializeField] private RectTransform itemContainer;

        private readonly List<RewardDisplayItemView> activeItems = new List<RewardDisplayItemView>();

        public void Refresh(IReadOnlyList<CollectedReward> rewards)
        {
            ClearItems();

            if (itemPrefab == null || itemContainer == null || rewards == null)
                return;

            for (int i = 0; i < rewards.Count; i++)
            {
                RewardDisplayItemView item = Instantiate(itemPrefab, itemContainer);
                item.Setup(rewards[i]);
                activeItems.Add(item);

                bool isLast = i == rewards.Count - 1;
                if (isLast)
                {
                    item.transform.localScale = Vector3.zero;
                    item.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
                }
            }
        }

        public void Clear()
        {
            ClearItems();
        }

        private void ClearItems()
        {
            for (int i = 0; i < activeItems.Count; i++)
            {
                if (activeItems[i] != null)
                    Destroy(activeItems[i].gameObject);
            }

            activeItems.Clear();
        }

        private void OnDestroy()
        {
            ClearItems();
        }
    }
}
