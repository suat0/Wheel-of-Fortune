using UnityEditor;
using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.EditorTools
{
    public static class ZoneRulesSmokeTest
    {
        [MenuItem("Vertigo Case/Tests/Run Zone Rules Smoke Test")]
        private static void Run()
        {
            GameConfig config = ScriptableObject.CreateInstance<GameConfig>();

            bool passed = true;
            passed &= ExpectZoneType(config, 1, ZoneType.Normal);
            passed &= ExpectZoneType(config, 4, ZoneType.Normal);
            passed &= ExpectZoneType(config, 5, ZoneType.Safe);
            passed &= ExpectZoneType(config, 10, ZoneType.Safe);
            passed &= ExpectZoneType(config, 29, ZoneType.Normal);
            passed &= ExpectZoneType(config, 30, ZoneType.Super);
            passed &= ExpectCollect(config, 1, false);
            passed &= ExpectCollect(config, 5, true);
            passed &= ExpectCollect(config, 30, true);

            Object.DestroyImmediate(config);

            if (passed)
            {
                Debug.Log("[ZoneRulesSmokeTest] Passed. Zone 5 is Safe, zone 30 is Super, collect is only safe/super.");
            }
            else
            {
                Debug.LogError("[ZoneRulesSmokeTest] Failed. Check zone rule implementation.");
            }
        }

        private static bool ExpectZoneType(GameConfig config, int zone, ZoneType expected)
        {
            ZoneType actual = config.GetZoneType(zone);
            if (actual == expected)
            {
                return true;
            }

            Debug.LogError($"[ZoneRulesSmokeTest] Zone {zone}: expected {expected}, got {actual}.");
            return false;
        }

        private static bool ExpectCollect(GameConfig config, int zone, bool expected)
        {
            bool actual = config.CanCollectAtZone(zone);
            if (actual == expected)
            {
                return true;
            }

            Debug.LogError($"[ZoneRulesSmokeTest] Collect at zone {zone}: expected {expected}, got {actual}.");
            return false;
        }
    }
}
