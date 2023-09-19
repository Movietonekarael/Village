using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Zenject;

namespace GameCore
{
    namespace GUI
    {
        [RequireComponent(typeof(Camera))]
        public sealed class UICameraAttacher : MonoBehaviour
        {
            [Inject(Id = "UiCamera")] private Camera _uiCamera;

            private void Awake()
            {
                var uicameraTransform = _uiCamera.transform;
                var uiCameraParent = uicameraTransform.parent;
                uicameraTransform.parent = null;
                SceneManager.MoveGameObjectToScene(_uiCamera.gameObject, this.gameObject.scene);

                var camera = GetComponent<Camera>();
                var cameraData = camera.GetUniversalAdditionalCameraData();
                cameraData.cameraStack.Add(_uiCamera);

                DontDestroyOnLoad(_uiCamera.gameObject);
                uicameraTransform.parent = uiCameraParent;
            }
        }
    }
}