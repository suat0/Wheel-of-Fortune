using VertigoCase.Data;

namespace VertigoCase.Core
{
    public interface IZoneManager
    {
        int CurrentZone { get; }
        ZoneType CurrentZoneType { get; }
        bool CanCollectCurrentZone { get; }

        void Initialize(GameConfig config);
        void ResetZone();
        int AdvanceZone();
        ZoneType GetZoneType(int zoneNumber);
        bool CanCollectAtZone(int zoneNumber);
    }
}
