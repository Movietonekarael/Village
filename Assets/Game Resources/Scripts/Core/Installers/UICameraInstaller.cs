using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;


namespace GameCore
{
    namespace Installers
    {
        public class UICameraInstaller : MonoInstaller
        {
            [SerializeField] private Camera _uiCamera;

            public override void InstallBindings()
            {
                var uiCameraInstance = CreateInstance();
                BindUiCamera(uiCameraInstance);
                RegistAsOverlayCamera(uiCameraInstance);
            }

            private Camera CreateInstance()
            {
                return Container.InstantiatePrefabForComponent<Camera>(_uiCamera);
            }

            private void BindUiCamera(Camera uiCameraInstance)
            {
                Container.Bind<Camera>().WithId("UiCamera").FromInstance(uiCameraInstance).AsSingle().NonLazy();
            }

            private void RegistAsOverlayCamera(Camera uiCameraInstance)
            {
                var camera = Camera.main;
                camera.GetUniversalAdditionalCameraData().cameraStack.Add(uiCameraInstance);
            }
        }
    }
}