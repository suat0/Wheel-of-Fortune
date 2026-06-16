using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private ZoneManager zoneManager;
        [SerializeField] private RewardManager rewardManager;
        [SerializeField] private WheelController wheelController;
        [SerializeField, Min(0f)] private float zoneTransitionDelay = 0.4f;

        public event Action<int, ZoneType> ZoneChanged;
        public event Action SpinStarted;
        public event Action<WheelSpinResult> SpinCompleted;
        public event Action BombHit;
        public event Action<IReadOnlyList<CollectedReward>> RewardsCollected;
        public event Action GameRestarted;

        public int CurrentZone => zoneManager != null ? zoneManager.CurrentZone : 0;
        public ZoneType CurrentZoneType => zoneManager != null ? zoneManager.CurrentZoneType : ZoneType.Normal;
        public GameConfig Config => gameConfig;

        public bool CanCollect =>
            rewardManager != null &&
            rewardManager.HasRewards &&
            !IsSpinning;

        public bool CanSpin => wheelController != null && !wheelController.IsSpinning && !isTransitioning;

        public IReadOnlyList<CollectedReward> CollectedRewards =>
            rewardManager != null
                ? rewardManager.GetCollectedRewards()
                : (IReadOnlyList<CollectedReward>)Array.Empty<CollectedReward>();

        public bool IsSpinning => wheelController != null && wheelController.IsSpinning;

        private bool isTransitioning;

        private void Awake()
        {
            if (zoneManager == null)
                zoneManager = GetComponent<ZoneManager>();
            if (rewardManager == null)
                rewardManager = GetComponent<RewardManager>();
            if (wheelController == null)
                wheelController = GetComponent<WheelController>();

            if (zoneManager != null && gameConfig != null)
                zoneManager.Initialize(gameConfig);
        }

        private void Start()
        {
            ConfigureWheelForCurrentZone();
            ZoneChanged?.Invoke(CurrentZone, CurrentZoneType);
        }

        public void RequestSpin()
        {
            if (!CanSpin || !HasRequiredReferences())
                return;

            SpinStarted?.Invoke();
            wheelController.Spin(HandleSpinCompleted);
        }

        public void CollectRewards()
        {
            if (!CanCollect)
                return;

            RewardsCollected?.Invoke(rewardManager.GetCollectedRewards());
        }

        public void RestartGame()
        {
            if (!HasRequiredReferences())
                return;

            StopAllCoroutines();
            isTransitioning = false;

            zoneManager.ResetZone();
            rewardManager.ClearRewards();
            GameRestarted?.Invoke();

            ConfigureWheelForCurrentZone();
            ZoneChanged?.Invoke(CurrentZone, CurrentZoneType);
        }

        public WheelConfig GetCurrentWheelConfig()
        {
            if (gameConfig == null || zoneManager == null)
                return null;
            return gameConfig.GetWheelConfig(zoneManager.CurrentZone);
        }

        public int GetCurrentZoneMultiplier()
        {
            if (gameConfig == null || zoneManager == null)
                return 1;
            return gameConfig.GetZoneRewardMultiplier(zoneManager.CurrentZone);
        }

        private void HandleSpinCompleted(WheelSpinResult result)
        {
            if (!HasRequiredReferences())
                return;

            if (result.IsBomb)
            {
                rewardManager.ClearRewards();
                SpinCompleted?.Invoke(result);
                BombHit?.Invoke();
                return;
            }

            if (result.Reward != null)
            {
                rewardManager.AddReward(result.Reward, result.Amount);
            }

            isTransitioning = true;
            SpinCompleted?.Invoke(result);
            StartCoroutine(TransitionToNextZone());
        }

        private IEnumerator TransitionToNextZone()
        {
            if (zoneTransitionDelay > 0f)
                yield return new WaitForSeconds(zoneTransitionDelay);

            zoneManager.AdvanceZone();
            ConfigureWheelForCurrentZone(true);
            isTransitioning = false;
            ZoneChanged?.Invoke(CurrentZone, CurrentZoneType);
        }

        private void ConfigureWheelForCurrentZone(bool animate = false)
        {
            if (wheelController == null || gameConfig == null || zoneManager == null)
                return;

            WheelConfig config = gameConfig.GetWheelConfig(zoneManager.CurrentZone);
            if (config == null)
                return;

            int multiplier = gameConfig.GetZoneRewardMultiplier(zoneManager.CurrentZone);

            if (animate)
                wheelController.ConfigureWithTransition(config, multiplier);
            else
                wheelController.Configure(config, multiplier);
        }

        private bool HasRequiredReferences()
        {
            return gameConfig != null && zoneManager != null && rewardManager != null;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                RequestSpin();
            if (Input.GetKeyDown(KeyCode.C))
                CollectRewards();
            if (Input.GetKeyDown(KeyCode.R))
                RestartGame();
        }
#endif
    }
}
