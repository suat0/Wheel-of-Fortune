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

        private readonly List<ZoneIndicatorItemView> activeItems = new List<ZoneIndicatorItemView>();

        public void ConfigureVisibleCount(int count)
        {
            visibleCount = Mathf.Max(3, count);
            if (visibleCount % 2 == 0)
                visibleCount++;
        }

        public void UpdateZone(int currentZone)
        {
            ClearItems();

            if (itemPrefab == null || itemContainer == null)
                return;

            int halfVisible = visibleCount / 2;
            int startZone = Mathf.Max(1, currentZone - halfVisible);

            for (int i = 0; i < visibleCount; i++)
            {
                int zone = startZone + i;
                ZoneType zoneType = gameConfig != null ? gameConfig.GetZoneType(zone) : ZoneType.Normal;
                bool isCurrent = zone == currentZone;
                bool isPassed = zone < currentZone;

                ZoneIndicatorItemView item = Instantiate(itemPrefab, itemContainer);
                item.Setup(zone, zoneType, isCurrent, isPassed);
                activeItems.Add(item);

                item.transform.localScale = Vector3.zero;
                item.transform.DOScale(Vector3.one, 0.25f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(i * 0.04f);

                if (isCurrent)
                {
                    item.transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 6, 0.5f)
                        .SetDelay(0.3f + i * 0.04f);
                }
            }
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
