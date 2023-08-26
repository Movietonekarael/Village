using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;


namespace GameCore
{
    namespace Installers
    {
        public sealed class UICameraInstaller : MonoInstaller
        {
            private const string _CAMERA_NAME = "UI Camera";
            [SerializeField] private Camera _uiCamera;

            public override void InstallBindings()
            {
                var uiCameraInstance = CreateInstance();
                BindUiCamera(uiCameraInstance);
                RegistAsOverlayCamera(uiCameraInstance);
            }

            private Camera CreateInstance()
            {
                var camera = Container.InstantiatePrefabForComponent<Camera>(_uiCamera);
                ChangeName(camera.gameObject);
                return camera;
            }

            private void ChangeName(GameObject gameObject)
            {
                gameObject.name = _CAMERA_NAME;
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