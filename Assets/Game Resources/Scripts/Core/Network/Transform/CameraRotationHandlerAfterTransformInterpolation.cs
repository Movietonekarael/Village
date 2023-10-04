using GameCore.GameControls;
using UnityEngine;

namespace GameCore
{
    namespace Network
    {
        [RequireComponent(typeof(CustomNetworkTransform))]
        public sealed class CameraRotationHandlerAfterTransformInterpolation : MonoBehaviour
        {
            private CustomNetworkTransform _customNetworkTransform;
            [SerializeField] private CameraFollowTargetRotator _targetRotator;

            private void Awake()
            {
                _customNetworkTransform = GetComponent<CustomNetworkTransform>();
                SubscribefForInterpolation();
            }

            private void OnDestroy()
            {
                UnsubscribefForInterpolation();
            }

            private void SubscribefForInterpolation()
            {
                _customNetworkTransform.OnTransformInterpolationTick += HandleRotation;
            }

            private void UnsubscribefForInterpolation()
            {
                _customNetworkTransform.OnTransformInterpolationTick -= HandleRotation;
            }

            private void HandleRotation()
            {
                _targetRotator.HandleRotation();
            }
        }
    }
}