using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertigoCase.Data;

namespace VertigoCase.Core
{
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Single authority on what the player is allowed to do. Replaces the
        /// previous scattered bool flags so UI-only guards (button interactable)
        /// are never the last line of defense.
        /// </summary>
        private enum GamePhase
        {
            Idle,
            Spinning,
            Transitioning,
            Collecting,
            GameOver
        }

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

        // Collect is only allowed while idle at a safe/super zone (README rule).
        public bool CanCollect =>
            phase == GamePhase.Idle &&
            rewardManager != null &&
            rewardManager.HasRewards &&
            zoneManager != null &&
            zoneManager.CanCollectCurrentZone;

        // Intentionally does NOT check wheelController.IsTransitioning: no event
        // fires when the wheel's visual transition ends, so disabling the button
        // on it would leave the UI stuck. A click in that short window is instead
        // rejected by Spin() returning false, which is a harmless no-op.
        public bool CanSpin =>
            phase == GamePhase.Idle &&
            wheelController != null &&
            wheelController.HasValidConfig &&
            HasRequiredReferences();

        public IReadOnlyList<CollectedReward> CollectedRewards =>
            rewardManager != null
                ? rewardManager.GetCollectedRewards()
                : (IReadOnlyList<CollectedReward>)Array.Empty<CollectedReward>();

        public bool IsSpinning => wheelController != null && wheelController.IsSpinning;

        private GamePhase phase = GamePhase.Idle;

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
            if (!CanSpin)
                return;

            // SpinStarted only fires once the wheel confirms the spin began;
            // otherwise the UI would lock itself waiting for a completion
            // callback that never comes.
            if (!wheelController.Spin(HandleSpinCompleted))
                return;

            phase = GamePhase.Spinning;
            SpinStarted?.Invoke();
        }

        public void CollectRewards()
        {
            if (!CanCollect)
                return;

            phase = GamePhase.Collecting;
            RewardsCollected?.Invoke(rewardManager.GetRewardsSnapshot());
        }

        public void RestartGame()
        {
            if (!HasRequiredReferences())
                return;

            StopAllCoroutines();
            phase = GamePhase.Idle;

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
            // isActiveAndEnabled: a completed-on-disable spin (scene teardown)
            // must not try to start coroutines on an inactive object.
            if (!isActiveAndEnabled || !HasRequiredReferences())
                return;

            if (result.IsBomb)
            {
                phase = GamePhase.GameOver;
                rewardManager.ClearRewards();
                SpinCompleted?.Invoke(result);
                BombHit?.Invoke();
                return;
            }

            if (result.Reward != null)
            {
                rewardManager.AddReward(result.Reward, result.Amount);
            }

            phase = GamePhase.Transitioning;
            SpinCompleted?.Invoke(result);
            StartCoroutine(TransitionToNextZone());
        }

        private IEnumerator TransitionToNextZone()
        {
            if (zoneTransitionDelay > 0f)
                yield return new WaitForSeconds(zoneTransitionDelay);

            zoneManager.AdvanceZone();
            ConfigureWheelForCurrentZone(true);
            phase = GamePhase.Idle;
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
