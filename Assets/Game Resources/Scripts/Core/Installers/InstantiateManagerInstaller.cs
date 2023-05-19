using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace GameCore.Installers
{
    public class InstantiateManagerInstaller : MonoInstaller
    {
        [SerializeField] private Transform _parentTransform;

        public override void InstallBindings()
        {
            var instance = CreateInstance();
            BindInstance(instance);
        }

        private InstantiateManager CreateInstance()
        {
            var instance = Container.InstantiateComponentOnNewGameObject<InstantiateManager>();
            instance.transform.parent = _parentTransform;
            return instance;
        }

        private void BindInstance(InstantiateManager instance)
        {
            Container.Bind<InstantiateManager>().FromInstance(instance).AsSingle().NonLazy();
        }
    }
}