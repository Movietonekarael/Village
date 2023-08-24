using GameCore.GUI;
using UnityEngine;
using Zenject;

namespace GameCore
{
    namespace Installers
    {
        public sealed class SelectionServiceInstaller : MonoInstaller
        {
            private const string _OBJECT_NAME = "SubmitService";
            [SerializeField] private Transform _parentTransform;

            public override void InstallBindings()
            {
                var serviceGameObject = CreateGameObjectForBinding();
                BindService(serviceGameObject);
            }

            private GameObject CreateGameObjectForBinding()
            {
                var serviceGameObject = new GameObject(_OBJECT_NAME);
                serviceGameObject.transform.parent = _parentTransform;
                return serviceGameObject;
            }

            private void BindService(GameObject serviceGameObject) 
            {
                Container.Bind<UiSelectionService>().FromNewComponentOn(serviceGameObject).AsSingle().NonLazy();
            }
        }
    }
}
