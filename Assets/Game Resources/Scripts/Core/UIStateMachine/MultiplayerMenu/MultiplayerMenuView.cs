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

            private IMultiplayerMenu _multiplayerMenu;


            protected override void InstantiateViewElementsOnAwake() { }

            public async override void Activate()
            {
                await InstantiateViewElements();
                SubscribeForMenuEvents();
                StartMultiplayerMenu();
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
                _multiplayerMenuObject = _instantiateService.InstantiateObjectWithInjections(multiplayerMenuPrefab, _canvasObject.transform);
                _canvasObject.name = multiplayerMenuPrefab.name;
                _multiplayerMenu = _multiplayerMenuObject.GetComponent<IMultiplayerMenu>();
            }

            private void StartMultiplayerMenu()
            {
                _multiplayerMenu.StartMultiplayerMenu();
            }

            private void SubscribeForMenuEvents()
            {
                if (_multiplayerMenu == null)
                    return;

                _multiplayerMenu.OnHostButtonPressed += HostServer;
                _multiplayerMenu.OnConnectButtonPressed += ConnectToServer;
                _multiplayerMenu.OnBackButtonPressed += BackToMainMenu;
            }

            private void UnsubscribeForMenuEvents()
            {
                if (_multiplayerMenu == null)
                    return;

                _multiplayerMenu.OnHostButtonPressed -= HostServer;
                _multiplayerMenu.OnConnectButtonPressed -= ConnectToServer;
                _multiplayerMenu.OnBackButtonPressed -= BackToMainMenu;
            }

            private void HostServer()
            {

            }

            private void ConnectToServer()
            {

            }

            private void BackToMainMenu()
            {
                _Controller.BackToMainMenu();
            }

            public override void Deactivate()
            {
                UnsubscribeForMenuEvents();
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
                UnsubscribeForMenuEvents();
                DeinitializeViewElements();
            }

            private void DeinitializeViewElements()
            {
                if (_multiplayerMenuHandle.IsValid() || _canvasHandle.IsValid())
                {
                    DestroyViewElements();
                    ReleaseAllAssets();
                }
            }
        }
    }
}