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
        private Tweener spinTween;
        private Sequence transitionSequence;
        private readonly List<WheelSliceView> activeSlices = new List<WheelSliceView>();

        public bool IsSpinning => spinTween != null && spinTween.IsActive() && spinTween.IsPlaying();
        public bool IsTransitioning => transitionSequence != null && transitionSequence.IsActive() && transitionSequence.IsPlaying();
        public WheelSpinResult LastResult { get; private set; }

        public void Configure(WheelConfig wheelConfig, int zoneRewardMultiplier)
        {
            currentWheelConfig = wheelConfig;
            currentZoneRewardMultiplier = Mathf.Max(1, zoneRewardMultiplier);

            if (currentWheelConfig == null)
            {
                Debug.LogError("[WheelController] WheelConfig is null.", this);
                return;
            }

            if (currentWheelConfig.ZoneType != ZoneType.Normal && currentWheelConfig.HasBomb)
            {
                Debug.LogError($"[WheelController] {currentWheelConfig.name} is {currentWheelConfig.ZoneType} but contains a bomb.", this);
            }

            KillActiveTween();

            if (wheelTransform != null)
                wheelTransform.localRotation = Quaternion.identity;

            RebuildSlices();
        }

        public void ConfigureWithTransition(WheelConfig wheelConfig, int zoneRewardMultiplier)
        {
            KillTransition();

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

        public void Spin(Action<WheelSpinResult> onCompleted)
        {
            if (IsSpinning || IsTransitioning)
                return;

            if (currentWheelConfig == null)
            {
                Debug.LogError("[WheelController] Configure must be called before Spin.", this);
                return;
            }

            int resultIndex = WheelResultResolver.GetRandomSliceIndex(currentWheelConfig);
            if (!WheelResultResolver.TryResolve(currentWheelConfig, currentZoneRewardMultiplier, resultIndex, out WheelSpinResult result))
                return;

            LastResult = result;

            if (wheelTransform == null)
            {
                onCompleted?.Invoke(result);
                return;
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
                int displayAmount = sliceConfig.IsBomb
                    ? 0
                    : sliceConfig.Amount * currentZoneRewardMultiplier * Mathf.Max(1, currentWheelConfig.RewardMultiplier);

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

        private void KillActiveTween()
        {
            if (spinTween != null && spinTween.IsActive())
            {
                spinTween.Kill();
                spinTween = null;
            }
        }

        private void KillTransition()
        {
            if (transitionSequence != null && transitionSequence.IsActive())
            {
                transitionSequence.Kill();
                transitionSequence = null;
            }

            if (wheelTransform != null)
                wheelTransform.localScale = Vector3.one;
        }

        private void OnDisable()
        {
            KillActiveTween();
            KillTransition();
        }

        private void OnDestroy()
        {
            KillActiveTween();
            KillTransition();
            ClearSlices();
        }
    }
}
