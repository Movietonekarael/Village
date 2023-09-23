using GameCore.GameControls;
using System;
using Unity.Netcode;

namespace GameCore
{
    namespace Network
    {
        public partial class NetworkInputHandler : ICameraAngle
        {
            public event Action<float> OnCameraAngleChanged;
            public ICameraAngle CameraAngle;

            private readonly NetworkVariable<float> _cameraAngle = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


            private void SubscribeForCameraNetworkVariable()
            {
                _cameraAngle.OnValueChanged += CameraAngleChangedOnServer;
            }

            private void UnsubscribeForCameraNetworkVariable()
            {
                _cameraAngle.OnValueChanged -= CameraAngleChangedOnServer;
            }

            private void CameraAngleChangedOnServer(float previousValue, float newValue)
            {
                OnCameraAngleChanged?.Invoke(newValue);
            }

            public void SubscribeForCameraControlEvents()
            {
                CameraAngle.OnCameraAngleChanged += CameraAngleChangedOnClient;
            }

            public void UnsubscribeForcameraControlEvents()
            {
                CameraAngle.OnCameraAngleChanged -= CameraAngleChangedOnClient;
            }

            private void CameraAngleChangedOnClient(float angle)
            {
                _cameraAngle.Value = angle;
            }
        }
    }
}