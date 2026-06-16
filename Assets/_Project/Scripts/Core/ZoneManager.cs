using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.Core
{
    public class ZoneManager : MonoBehaviour, IZoneManager
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField, Min(1)] private int startingZone = 1;

        private int currentZone;

        public int CurrentZone => currentZone;
        public ZoneType CurrentZoneType => GetZoneType(currentZone);
        public bool CanCollectCurrentZone => CanCollectAtZone(currentZone);
        public GameConfig Config => gameConfig;

        private void Awake()
        {
            ResetZone();
        }

        public void Initialize(GameConfig config)
        {
            gameConfig = config;
            ResetZone();
        }

        public void ResetZone()
        {
            currentZone = Mathf.Max(1, startingZone);
        }

        public int AdvanceZone()
        {
            currentZone++;
            return currentZone;
        }

        public ZoneType GetZoneType(int zoneNumber)
        {
            return gameConfig != null ? gameConfig.GetZoneType(zoneNumber) : ZoneType.Normal;
        }

        public bool CanCollectAtZone(int zoneNumber)
        {
            return gameConfig != null && gameConfig.CanCollectAtZone(zoneNumber);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            startingZone = Mathf.Max(1, startingZone);
        }
#endif
    }
}
