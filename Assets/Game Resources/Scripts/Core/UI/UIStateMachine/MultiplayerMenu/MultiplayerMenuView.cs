using GameCore.GUI.Menus;
using GameCore.GUI.Windows;
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

            private ConnectionCodeWindow _connectionCodeWindow;
            private MessageWindow _messageWindow;


            protected override void InstantiateViewElementsOnAwake() 
            {
                InstantiateWindows();
            }

            private void InstantiateWindows()
            {
                _connectionCodeWindow = _instantiateService.CreateNewWithInjections<ConnectionCodeWindow>();
                _messageWindow = _instantiateService.CreateNewWithInjections<MessageWindow>();
            }

            public async override void Activate()
            {
                await InstantiateViewElements();
                SubscribeForMenuEvents();
                SubscribeForWindowsEvents();
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

            private void SubscribeForWindowsEvents()
            {
                _connectionCodeWindow.OnOkPressed += HandleConnectionCode;
                _messageWindow.OnOkPressed += HandleMessageWindowClosed;
            }

            private void UnsubscribeForWindowsEvents()
            {
                _connectionCodeWindow.OnOkPressed -= HandleConnectionCode;
                _messageWindow.OnOkPressed -= HandleMessageWindowClosed;
            }

            private void HandleConnectionCode(string code)
            {
                _Controller.ConnectToServer(code);
            }

            private void HandleMessageWindowClosed()
            {
                EnableAllButtons();
            }

            private void EnableAllButtons()
            {
                _multiplayerMenu.EnableAllButtons();
            }

            private void HostServer()
            {
                _multiplayerMenu.DisableAllButtons();
                _Controller.StartHostServer();
            }

            private void ConnectToServer()
            {
                _multiplayerMenu.DisableAllButtons();
                _connectionCodeWindow.CreateWindow(_canvasObject.transform);
            }

            private void BackToMainMenu()
            {
                _Controller.BackToMainMenu();
            }

            public void CreateMessageWindow(string message)
            {
                _messageWindow.CreateNewWindow(message, _canvasObject.transform);
            }

            public override void Deactivate()
            {
                UnsubscribeForMenuEvents();
                UnsubscribeForWindowsEvents();
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
                UnsubscribeForWindowsEvents();
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