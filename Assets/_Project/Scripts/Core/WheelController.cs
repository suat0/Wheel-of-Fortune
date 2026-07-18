using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VertigoCase.Data;
using VertigoCase.UI;

namespace VertigoCase.Core
{
    public class WheelController : MonoBehaviour, IWheelController
    {
        [SerializeField] private RectTransform wheelTransform;
        [SerializeField] private Transform sliceContainer;
        [SerializeField] private WheelSliceView slicePrefab;

        private WheelConfig currentWheelConfig;
        private int currentZoneRewardMultiplier = 1;
        private Tween spinTween;
        private Sequence transitionSequence;
        private readonly List<WheelSliceView> activeSlices = new List<WheelSliceView>();

        public bool IsSpinning => spinTween != null && spinTween.IsActive() && spinTween.IsPlaying();
        public bool IsTransitioning => transitionSequence != null && transitionSequence.IsActive() && transitionSequence.IsPlaying();
        public bool HasValidConfig => currentWheelConfig != null && currentWheelConfig.SliceCount > 0;
        public bool CanSpin => !IsSpinning && !IsTransitioning && HasValidConfig;
        public WheelSpinResult LastResult { get; private set; }

        public void Configure(WheelConfig wheelConfig, int zoneRewardMultiplier)
        {
            currentWheelConfig = wheelConfig;
            currentZoneRewardMultiplier = Mathf.Max(1, zoneRewardMultiplier);

            KillActiveTween();

            if (wheelTransform != null)
                wheelTransform.localRotation = Quaternion.identity;

            if (currentWheelConfig == null)
            {
                Debug.LogError("[WheelController] WheelConfig is null.", this);
                RebuildSlices();
                return;
            }

            if (currentWheelConfig.ZoneType != ZoneType.Normal && currentWheelConfig.HasBomb)
            {
                Debug.LogError($"[WheelController] {currentWheelConfig.name} is {currentWheelConfig.ZoneType} but contains a bomb.", this);
            }

            RebuildSlices();
        }

        public void ConfigureWithTransition(WheelConfig wheelConfig, int zoneRewardMultiplier)
        {
            // Complete any previous transition so its pending Configure callback
            // still runs — otherwise the wheel could stay on the old zone's
            // config while the game has already advanced.
            KillTransition(complete: true);

            if (wheelTransform == null)
            {
                Configure(wheelConfig, zoneRewardMultiplier);
                return;
            }

            transitionSequence = DOTween.Sequence()
                .Append(wheelTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack))
                .AppendCallback(() =>
                {
                    Configure(wheelConfig, zoneRewardMultiplier);
                    wheelTransform.localScale = Vector3.zero;
                })
                .Append(wheelTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack))
                .OnComplete(() => transitionSequence = null);
        }

        /// <summary>
        /// Starts a spin. Returns false when the spin could not start (already
        /// spinning, mid-transition, or no valid config) so the caller never
        /// waits for a completion callback that will not come.
        /// </summary>
        public bool Spin(Action<WheelSpinResult> onCompleted)
        {
            if (!HasValidConfig)
            {
                Debug.LogError("[WheelController] Configure must be called with a valid config before Spin.", this);
                return false;
            }

            if (IsSpinning || IsTransitioning)
                return false;

            int resultIndex = WheelResultResolver.GetRandomSliceIndex(currentWheelConfig);
            if (!WheelResultResolver.TryResolve(currentWheelConfig, currentZoneRewardMultiplier, resultIndex, out WheelSpinResult result))
                return false;

            LastResult = result;

            if (wheelTransform == null)
            {
                // Deferred one frame so the completion callback never fires
                // before the caller has processed the fact that the spin started.
                spinTween = DOVirtual.DelayedCall(0f, () =>
                {
                    spinTween = null;
                    onCompleted?.Invoke(result);
                });
                return true;
            }

            float targetAngle = CalculateTargetAngle(currentWheelConfig, result.SliceIndex);

            spinTween = wheelTransform
                .DOLocalRotate(new Vector3(0f, 0f, targetAngle), currentWheelConfig.SpinDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutQuart)
                .OnComplete(() =>
                {
                    spinTween = null;
                    onCompleted?.Invoke(result);
                });
            return true;
        }

        [ContextMenu("Test Spin Current Config")]
        public void TestSpinCurrentConfig()
        {
            Spin(result =>
            {
                Debug.Log(
                    $"[WheelController] Test spin completed. Slice: {result.SliceIndex}, " +
                    $"Bomb: {result.IsBomb}, Amount: {result.Amount}",
                    this
                );
            });
        }

        public static float CalculateTargetAngle(WheelConfig wheelConfig, int sliceIndex)
        {
            if (wheelConfig == null || wheelConfig.SliceCount == 0)
                return 0f;

            float sliceAngle = wheelConfig.SliceAngle;
            float targetSliceAngle = sliceIndex * sliceAngle;
            return wheelConfig.ExtraRotations * 360f + targetSliceAngle;
        }

        private void RebuildSlices()
        {
            ClearSlices();

            if (currentWheelConfig == null || sliceContainer == null || slicePrefab == null)
                return;

            for (int i = 0; i < currentWheelConfig.SliceCount; i++)
            {
                WheelSliceConfig sliceConfig = currentWheelConfig.Slices[i];
                WheelSliceView sliceView = Instantiate(slicePrefab, sliceContainer);
                int displayAmount = WheelResultResolver.GetSliceAmount(
                    sliceConfig, currentZoneRewardMultiplier, currentWheelConfig.RewardMultiplier);

                sliceView.Setup(sliceConfig, displayAmount, i * currentWheelConfig.SliceAngle);
                activeSlices.Add(sliceView);
            }
        }

        private void ClearSlices()
        {
            for (int i = 0; i < activeSlices.Count; i++)
            {
                if (activeSlices[i] != null)
                    Destroy(activeSlices[i].gameObject);
            }

            activeSlices.Clear();
        }

        // complete=true finishes the tween instantly (invoking its callback)
        // instead of cancelling it. Used on disable so a mid-spin result is
        // never silently swallowed; Configure/restart paths cancel on purpose.
        private void KillActiveTween(bool complete = false)
        {
            if (spinTween != null && spinTween.IsActive())
            {
                spinTween.Kill(complete);
            }

            spinTween = null;
        }

        private void KillTransition(bool complete = false)
        {
            if (transitionSequence != null && transitionSequence.IsActive())
            {
                transitionSequence.Kill(complete);
            }

            transitionSequence = null;

            if (wheelTransform != null)
                wheelTransform.localScale = Vector3.one;
        }

        private void OnDisable()
        {
            KillActiveTween(complete: true);
            KillTransition(complete: true);
        }

        private void OnDestroy()
        {
            KillActiveTween();
            KillTransition();
            ClearSlices();
        }
    }
}
