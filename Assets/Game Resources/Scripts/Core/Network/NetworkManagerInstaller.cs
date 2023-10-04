using GameCore.Network;
using Unity.Netcode;
using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace Installers
    {
        public sealed class NetworkManagerInstaller : MonoInstaller
        {
            [SerializeField] private GameObject _prefab;


            public override void InstallBindings()
            {
                var instance = CreateInstance();
                BindInstance(instance);
            }

            private NetworkManagerPrefabs CreateInstance()
            {
                var gameObjectInstance = CreateGameObjectInstance();
                if (gameObjectInstance == null)
                    return null;

                var componentExist = gameObjectInstance.TryGetComponent<NetworkManagerPrefabs>(out var instance);
                if (componentExist)
                {
                    return instance;
                }
                else
                {
                    Debug.LogWarning($"Can not find NetworkManagerPrefabs component on instance of prefab attached to {this.name}");
                    return null;
                }
            }

            private GameObject CreateGameObjectInstance()
            {
                if (_prefab != null)
                {
                    return InstantiatePrefab();
                }
                else
                {
                    Debug.LogWarning($"There is no prefab attached to {this.name}");
                    return null;
                }
            }

            private GameObject InstantiatePrefab()
            {
                var instance = Instantiate(_prefab);
                instance.name = _prefab.name;
                return instance;
            }

            private void BindInstance(NetworkManagerPrefabs instance)
            {
                if (instance == null)
                {
                    Debug.LogWarning($"NetworkManagerPrefabs instance is null.");
                    return;
                }

                Container.Bind<INetworkManagerPrefabs>()
                         .To<NetworkManagerPrefabs>()
                         .FromInstance(instance)
                         .AsSingle()
                         .NonLazy();
            }
        }
    }
}