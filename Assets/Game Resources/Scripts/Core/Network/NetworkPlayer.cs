using GameCore.GameControls;
using GameCore.Services;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace GameCore
{
    namespace Network
    {
        public class NetworkPlayer : NetworkBehaviour
        {
            private NetworkVariable<Vector3> _playerPosition = new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

            [Inject(Id = "Player")] private GameObject _localPlayer;
            private Transform _localPlayerTransform;
            private bool _transformCached = false;


            private void Awake()
            {   
                DontDestroyOnLoad(this.gameObject);
            }

            public void ConnectNetworkPlayer()
            {
                InjectThis();
                CacheLocalPlayerTransform();

                if (!IsServer)
                    SubscribeForNetworkVariablesEventsServerRpc();
            }

            public void CreatePlayerDoll()
            {

            }

            private void InjectThis()
            {
                InstantiateService.Singleton.Inject(this);
            }

            private void CacheLocalPlayerTransform()
            {
                _localPlayerTransform = _localPlayer.transform;
                _transformCached = true;
            }

            [ServerRpc]
            private void SubscribeForNetworkVariablesEventsServerRpc()
            {
                SubscribeForNetworkVariablesEvents();
            }

            private void SubscribeForNetworkVariablesEvents()
            {
                _playerPosition.OnValueChanged += ChangePlayerPosition;
            }

            private void ChangePlayerPosition(Vector3 previous, Vector3 current)
            {
                //_localPlayerTransform.position = current;
                Debug.Log($"Client position: {current}");
            }

            private void FixedUpdate()
            {
                if (!IsServer && _transformCached) 
                {
                    _playerPosition.Value = _localPlayerTransform.position;
                }
            }
        }
    }
}