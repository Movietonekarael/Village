using GameCore.Services;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace GameCore
{
    namespace GUI
    {
        public sealed class MultiplayerMenuView : UIView<MultiplayerMenuViewParameters,
                                                         IMultiplayerMenuController,
                                                         IMultiplayerMenuView>,
                                                  IMultiplayerMenuView
        {
            private AssetReferenceGameObject _canvasReference => _Parameters.CanvasReference;
            private AssetReferenceGameObject _multiplayerMenuReference => _Parameters.MultiplayerMenuReference;

            [Inject] private readonly InstantiateService _instantiateService;
            [Inject(Id = "UiCamera")] private readonly Camera _uiCamera;

            private const string _CANVAS_NAME = "MultiplayerMenuCanvas";

            private GameObject _canvasObject;
            private GameObject _multiplayerMenuObject;

            private AsyncOperationHandle _canvasHandle;
            private AsyncOperationHandle _multiplayerMenuHandle;


            public async override void Activate()
            {
                await InstantiateViewElements();
            }

            private async Task InstantiateViewElements()
            {
                InitializeHandlers();

                await InstantiateCanvas();
                await InstantiateMultiplayerMenu();
            }

            private void InitializeHandlers()
            {
                _canvasHandle = _canvasReference.LoadAssetAsync<GameObject>();
                _multiplayerMenuHandle = _multiplayerMenuReference.LoadAssetAsync<GameObject>();
            }

            private async Task InstantiateCanvas()
            {
                await _canvasHandle.Task;

                _canvasObject = _instantiateService.InstantiateObject(_canvasHandle.Result as GameObject);
                _canvasObject.name = _CANVAS_NAME;
                var canvas = _canvasObject.GetComponent<Canvas>();
                canvas.worldCamera = _uiCamera;
            }

            private async Task InstantiateMultiplayerMenu()
            {
                await _multiplayerMenuHandle.Task;

                var multiplayerMenuPrefab = _multiplayerMenuHandle.Result as GameObject;
                _multiplayerMenuObject = _instantiateService.InstantiateObject(multiplayerMenuPrefab, _canvasObject.transform);
                _canvasObject.name = multiplayerMenuPrefab.name;
            }

            public override void Deactivate()
            {
                DestroyViewElements();
                ReleaseAllAssets();
            }

            private void DestroyViewElements()
            {
                _instantiateService.DestroyObject(_multiplayerMenuObject);
                _instantiateService.DestroyObject(_canvasObject);
            }

            private void ReleaseAllAssets()
            {
                Addressables.Release(_multiplayerMenuHandle);
                Addressables.Release(_canvasHandle);
            }

            public override void Deinitialize()
            {
                if (_multiplayerMenuHandle.IsValid() || _canvasHandle.IsValid()) 
                {
                    DestroyViewElements();
                    ReleaseAllAssets();
                }
            }

            protected override void InstantiateViewElementsOnAwake()
            {

            }
        }
    }
}