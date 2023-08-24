using GameCore.Services;
using UnityEngine;
using Zenject;

namespace GameCore
{
    namespace Installers
    {
        public sealed class InstantiateServiceInstaller : MonoInstaller
        {
            private const string _SERVICE_NAME = "Instantiate Service";
            [SerializeField] private Transform _parentTransform;

            public override void InstallBindings()
            {
                var instance = CreateInstance();
                BindInstance(instance);
            }

            private InstantiateService CreateInstance()
            {
                var instance = CreateGameObject().AddComponent<InstantiateService>();
                Container.Inject(instance);
                return instance;
            }

            private GameObject CreateGameObject()
            {
                var gameObject = new GameObject();
                gameObject.transform.parent = _parentTransform;
                gameObject.name = _SERVICE_NAME;
                return gameObject;
            }

            private void BindInstance(InstantiateService instance)
            {
                Container.Bind<InstantiateService>().FromInstance(instance).AsSingle().NonLazy();
            }
        }
    }
}