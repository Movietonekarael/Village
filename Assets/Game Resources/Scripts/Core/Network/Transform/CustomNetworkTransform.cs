using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace GameCore
{
    namespace Network
    {
        public sealed class CustomNetworkTransform : DefaultNetworkBehaviour
        {
            private NetworkVariable<NetworkTransformPR> _networkTransform = new(default,
                                                                                NetworkVariableReadPermission.Everyone,
                                                                                NetworkVariableWritePermission.Server);

            private bool _transformChangingEnded = true;
            private Coroutine _transformChangingRoutine = null;
            private uint _networkTickRate => NetworkManager.Singleton.NetworkTickSystem.TickRate;
            private float _tickTime => 1f / _networkTickRate;

            public event Action OnTransformInterpolationTick;


            protected override void OnClientNetworkSpawn()
            {
                SubscribeNetworkVariables();
            }

            protected override void OnClientNetworkDespawn()
            {
                UnsubscribeNetworkVariables();
            }

            protected override void ServerFixedUpdate()
            {
                var newTransform = new NetworkTransformPR(this.transform);
                if (_networkTransform.Value != newTransform)
                    _networkTransform.Value = newTransform;
            }

            private void SubscribeNetworkVariables()
            {
                _networkTransform.OnValueChanged += TransformChanged;
            }

            private void UnsubscribeNetworkVariables()
            {
                _networkTransform.OnValueChanged -= TransformChanged;
            }

            private void TransformChanged(NetworkTransformPR previousValue, NetworkTransformPR newValue)
            {
                if (!_transformChangingEnded)
                {
                    StopCoroutine(_transformChangingRoutine);
                }

                _transformChangingRoutine = StartCoroutine(InterpolateTransform(newValue));
            }

            private IEnumerator InterpolateTransform(NetworkTransformPR newTransform)
            {
                _transformChangingEnded = false;

                var (newPosition, newRotation) = GetNewPositionAndRotation(newTransform);
                var (previousPosition, previousRotation) = GetPreviousPositionAndRotation();

                yield return ProsessInterpolation(previousPosition, previousRotation, newPosition, newRotation);
                _transformChangingEnded = true;
                

                static (Vector3, Quaternion) GetNewPositionAndRotation(NetworkTransformPR newTransform)
                {
                    var newPosition = newTransform.Position;
                    var newRotation = new Quaternion();
                    QuaternionCompressor.DecompressQuaternion(ref newRotation, newTransform.Rotation);
                    return (newPosition, newRotation);
                }

                (Vector3, Quaternion) GetPreviousPositionAndRotation()
                {
                    return (this.transform.localPosition, this.transform.localRotation);
                }

                IEnumerator ProsessInterpolation(Vector3 previousPosition,
                                                 Quaternion previousRotation,
                                                 Vector3 newPosition,
                                                 Quaternion newRotation)
                {
                    float timePassed = 0;

                    while (timePassed < _tickTime)
                    {
                        timePassed += Time.deltaTime;

                        var timeParameter = timePassed / _tickTime;

                        this.transform.localPosition = Vector3.Lerp(previousPosition, newPosition, timeParameter);
                        this.transform.localRotation = Quaternion.Lerp(previousRotation, newRotation, timeParameter);

                        OnTransformInterpolationTick?.Invoke();
                        yield return null;
                    }
                }
            }
        }
    }
}