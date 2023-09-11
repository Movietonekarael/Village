using GameCore.Network;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace GameCore
{

    namespace Installers
    {
        public sealed class NetworkConnectionServiceInstaller : MonoInstaller
        {
            private const string _SERVICE_NAME = "NetworkConnectionService";
            [SerializeField] private GameObject _prefab;

            public override void InstallBindings()
            {
                var instance = CreateInstance();
                BindInstance(instance);
            }

            private NetworkConnectionService CreateInstance()
            {
                var instance = CreateGameObject().GetComponent<NetworkConnectionService>();
                return instance;
            }

            private GameObject CreateGameObject()
            {
                var gameObject = Instantiate(_prefab);
                var networkObject = gameObject.GetComponent<NetworkObject>();
                networkObject.Spawn(false);

                return gameObject;
            }

            private void BindInstance(NetworkConnectionService instance)
            {
                Container.Bind<NetworkConnectionService>().FromInstance(instance).AsSingle().NonLazy();
            }
        }
    }
}