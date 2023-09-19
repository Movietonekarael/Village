using GameCore.GameControls;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

namespace GameCore
{
    namespace Network
    {
        public sealed class NetworkPlayerTransform : NetworkBehaviour
        {
            private Transform _dollTransform;
            private Transform _playerTransform;

            private NetworkVariable<NetworkTransformPR> _networkTransform = new(default, 
                                                                                NetworkVariableReadPermission.Everyone, 
                                                                                NetworkVariableWritePermission.Server);

            private IDollSpawner _dollSpawner;


            public void SetPlayerTransform(Transform playerTransform) 
            {
                _playerTransform = playerTransform;
            }

            private void Awake()
            {
                _dollSpawner = GetComponent<IDollSpawner>();
            }

            public override void OnNetworkSpawn()
            {
                if (!IsServer)
                {
                    SubscribeForDollSpawning();
                    SubscribeNetworkVariables();
                }
            }

            public override void OnNetworkDespawn()
            {
                if (!IsServer)
                {
                    UnsubscribeForDollSpawning();
                    UnsubscribeNetworkVariables();
                }
            }

            private void SubscribeForDollSpawning()
            {
                _dollSpawner.OnDollSpawned += LinkDoll;
            }

            private void UnsubscribeForDollSpawning()
            {
                _dollSpawner.OnDollSpawned -= LinkDoll;
            }

            private void SubscribeNetworkVariables()
            {
                _networkTransform.OnValueChanged += TransformChanged;
            }

            private void UnsubscribeNetworkVariables()
            {
                _networkTransform.OnValueChanged -= TransformChanged;
            }

            private CancellationTokenSource _changeDollTransformCancellationTokenSource = new();

            [ClientRpc]
            private void ChangeTransformClientRpc(NetworkTransformPR newValue)
            {
                if (IsServer) return;

                TransformChanged(new NetworkTransformPR(), newValue);
            }

            private void TransformChanged(NetworkTransformPR previousValue, NetworkTransformPR newValue)
            {
                if (_dollTransform != null)
                {
                    if (!_dollTransformChangingEnded)
                    {
                        StopCoroutine(_dollTransformChangingRoutine);
                    }

                    _dollTransformChangingRoutine = StartCoroutine(InterpolateTransform(newValue));
                }    
            }

            private bool _dollTransformChangingEnded = true;
            private Coroutine _dollTransformChangingRoutine = null;
            private uint _networkTickRate => NetworkManager.Singleton.NetworkTickSystem.TickRate;
            private float _tickTime => 1f / _networkTickRate;
            private CameraFollowTargetRotator _targetRotator;

            private IEnumerator InterpolateTransform(NetworkTransformPR newTransform)
            {
                _dollTransformChangingEnded = false;

                var newPosition = newTransform.Position;
                var newRotation = new Quaternion();

                var previousPosition = _dollTransform.localPosition;
                var previousRotation = _dollTransform.localRotation;

                QuaternionCompressor.DecompressQuaternion(ref newRotation, newTransform.Rotation);

                float timePassed = 0;

                while (timePassed < _tickTime)
                {
                    timePassed += Time.deltaTime;

                    var t = timePassed / _tickTime;

                    _dollTransform.localPosition = Vector3.Lerp(previousPosition, newPosition, t);
                    _dollTransform.localRotation = Quaternion.Lerp(previousRotation, newRotation, t);

                    if (_targetRotator != null) _targetRotator.HandleRotation();
                    yield return null;
                }

                _dollTransformChangingEnded = true;
            }

            private void LinkDoll(GameObject doll)
            {
                _dollTransform = doll.transform;
                _targetRotator = _dollTransform.GetComponentInChildren<CameraFollowTargetRotator>();
                if (_targetRotator != null ) 
                {
                    _targetRotator.AllowToUpdate = false;
                }
                SetDollPositionServerRpc();
            }

            [ServerRpc(RequireOwnership = false)]//?????????????????????????
            private void SetDollPositionServerRpc()
            {
                if (_playerTransform != null)
                {
                    //Debug.Log("Suka");
                    var newTransform = new NetworkTransformPR(_playerTransform);
                    if (_networkTransform.Value != newTransform)
                    {
                        //Debug.Log("Suka2");
                        _networkTransform.Value = newTransform;
                    }

                    ChangeTransformClientRpc(newTransform);
                }                
            }

            private void FixedUpdate()
            {
                if (_playerTransform != null)
                {
                    var newTransform = new NetworkTransformPR(_playerTransform);
                    if (_networkTransform.Value != newTransform)
                        _networkTransform.Value = newTransform;
                }
            }
        }
    }
}