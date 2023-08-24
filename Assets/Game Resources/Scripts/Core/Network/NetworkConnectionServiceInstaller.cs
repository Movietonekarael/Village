using GameCore.Network;
using UnityEngine;
using Zenject;

namespace GameCore
{

    namespace Installers
    {
        public sealed class NetworkConnectionServiceInstaller : MonoInstaller
        {
            private const string _SERVICE_NAME = "NetworkConnectionService";
            [SerializeField] private Transform _parentTransform;

            public override void InstallBindings()
            {
                var instance = CreateInstance();
                BindInstance(instance);
            }

            private NetworkConnectionService CreateInstance()
            {
                var instance = CreateGameObject().AddComponent<NetworkConnectionService>();
                return instance;
            }

            private GameObject CreateGameObject()
            {
                var gameObject = new GameObject
                {
                    name = _SERVICE_NAME
                };
                gameObject.transform.parent = _parentTransform;
                return gameObject;
            }

            private void BindInstance(NetworkConnectionService instance)
            {
                Container.Bind<NetworkConnectionService>().FromInstance(instance).AsSingle().NonLazy();
            }
        }
    }
}