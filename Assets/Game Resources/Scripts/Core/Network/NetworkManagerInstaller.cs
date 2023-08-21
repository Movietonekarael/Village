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

            private NetworkManager CreateInstance()
            {
                var gameObjectInstance = CreateGameObjectInstance();
                if (gameObjectInstance == null)
                    return null;

                var componentExist = gameObjectInstance.TryGetComponent<NetworkManager>(out var instance);
                if (componentExist)
                {
                    return instance;
                }
                else
                {
                    Debug.LogWarning($"Can not find NetworkManager component on instance of prefab attached to {this.name}");
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

            private void BindInstance(NetworkManager instance)
            {
                if (instance == null)
                {
                    Debug.LogWarning($"NetworkManager instance is null.");
                    return;
                }

                Container.Bind<NetworkManager>().FromInstance(instance).AsSingle().NonLazy();
            }
        }
    }
}