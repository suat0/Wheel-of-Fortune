using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VertigoCase.Data;

namespace VertigoCase.UI
{
    public class ZoneIndicatorView : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private ZoneIndicatorItemView itemPrefab;
        [SerializeField] private RectTransform itemContainer;
        [SerializeField, Min(3)] private int visibleCount = 7;

        // Pooled items: the strip always shows visibleCount entries, so the same
        // instances are re-setup on every zone change instead of destroy/instantiate.
        private readonly List<ZoneIndicatorItemView> itemPool = new List<ZoneIndicatorItemView>();

        public void ConfigureVisibleCount(int count)
        {
            visibleCount = Mathf.Max(3, count);
            if (visibleCount % 2 == 0)
                visibleCount++;
        }

        public void UpdateZone(int currentZone)
        {
            if (itemPrefab == null || itemContainer == null)
                return;

            while (itemPool.Count < visibleCount)
                itemPool.Add(Instantiate(itemPrefab, itemContainer));

            int halfVisible = visibleCount / 2;
            int startZone = Mathf.Max(1, currentZone - halfVisible);

            for (int i = 0; i < itemPool.Count; i++)
            {
                ZoneIndicatorItemView item = itemPool[i];
                if (item == null)
                    continue;

                item.transform.DOKill();

                bool inUse = i < visibleCount;
                item.gameObject.SetActive(inUse);
                if (!inUse)
                    continue;

                int zone = startZone + i;
                ZoneType zoneType = gameConfig != null ? gameConfig.GetZoneType(zone) : ZoneType.Normal;
                bool isCurrent = zone == currentZone;
                bool isPassed = zone < currentZone;

                item.Setup(zone, zoneType, isCurrent, isPassed);

                item.transform.localScale = Vector3.zero;
                item.transform.DOScale(Vector3.one, 0.25f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(i * 0.04f)
                    .SetLink(item.gameObject);

                if (isCurrent)
                {
                    item.transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 6, 0.5f)
                        .SetDelay(0.3f + i * 0.04f)
                        .SetLink(item.gameObject);
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            visibleCount = Mathf.Max(3, visibleCount);
            if (visibleCount % 2 == 0)
                visibleCount++;
        }
#endif
    }
}
