using UnityEngine;
using Zenject.Internal;

namespace GameCore
{
    namespace Network
    {
        public sealed class NetworkManagerPrefabs : MonoBehaviour, INetworkManagerPrefabs
        {
            [SerializeField] private GameObject _defaultNeworkManager;
            [SerializeField] private GameObject _relayNetworkManager;

            private GameObject _currentNetworkManager = null;

            public static NetworkManagerPrefabs Singleton;

            private void Awake()
            {
                DontDestroyOnLoad(this.gameObject);
                if (Singleton == null)
                {
                    Singleton = this;
                }
            }

            public void CreateDefaultNetworkManager()
            {
                _currentNetworkManager = Instantiate(_defaultNeworkManager);
            }

            public void CreateRelayNetworkManager()
            {
                _currentNetworkManager = Instantiate(_relayNetworkManager);
            }

            public void DestroyNetworkManager()
            {
                Destroy(_currentNetworkManager);
                _currentNetworkManager = null;
            }
        }
    }
}