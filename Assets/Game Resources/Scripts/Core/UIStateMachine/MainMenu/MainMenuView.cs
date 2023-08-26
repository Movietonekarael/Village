using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using GameCore.Services;
using System.Threading.Tasks;

namespace GameCore
{
    namespace GUI
    {
        public sealed class MainMenuView : UIView<MainMenuViewParameters, IMainMenuController, IMainMenuView>, IMainMenuView
        {
            private AssetReferenceGameObject _canvasReference => _Parameters.CanvasReference;
            private AssetReferenceGameObject _mainMenuReference => _Parameters.MainMenuReference;

            [Inject] private readonly InstantiateService _instantiateService;
            [Inject(Id = "UiCamera")] private readonly Camera _uiCamera;

            private const string _CANVAS_NAME = "MainMenuCanvas";

            private GameObject _canvasObject;
            private GameObject _mainMenuObject;

            private AsyncOperationHandle _canvasHandle;
            private AsyncOperationHandle _mainMenuHandle;

            private bool _startupAnimationsAllowed;

            private IMainMenu _mainMenu;


            protected override void InstantiateViewElementsOnAwake() { }

            public async override void Activate()
            {
                await InstantiateViewElements();
                SubscribeForMenuEvents();
                StartMainMenu();
            }

            private async Task InstantiateViewElements()
            {
                InitializeHandles();

                await InstantiateCanvas();
                await InstantiateMainMenu();
            }

            private void InitializeHandles()
            {
                _canvasHandle = _canvasReference.LoadAssetAsync<GameObject>();
                _mainMenuHandle = _mainMenuReference.LoadAssetAsync<GameObject>();
            }

            private async Task InstantiateCanvas()
            {
                await _canvasHandle.Task;

                _canvasObject = _instantiateService.InstantiateObject(_canvasHandle.Result as GameObject);
                _canvasObject.name = _CANVAS_NAME;
                var canvas = _canvasObject.GetComponent<Canvas>();
                canvas.worldCamera = _uiCamera;
            }

            private async Task InstantiateMainMenu()
            {
                await _mainMenuHandle.Task;

                _mainMenuObject = _instantiateService.InstantiateObjectWithInjections(_mainMenuHandle.Result as GameObject, _canvasObject.transform);
                _mainMenuObject.name = (_mainMenuHandle.Result as GameObject).name;
                _mainMenu = _mainMenuObject.GetComponent<MainMenu>();
                _mainMenu.SetAnimated(_startupAnimationsAllowed);
            }

            private void StartMainMenu()
            {
                _mainMenu.StartMainMenu();
            }

            private void SubscribeForMenuEvents()
            {
                _mainMenu.OnSinglePlayerButtonPressed += StartSinglePlayer;
                _mainMenu.OnMultiplayerButtonPressed += StartMultiplayer;
                _mainMenu.OnQuitApplicationPressed += QuitApplication;
            }

            private void UnsubscribeForMenuEvents()
            {
                _mainMenu.OnSinglePlayerButtonPressed += StartSinglePlayer;
                _mainMenu.OnMultiplayerButtonPressed += StartMultiplayer;
                _mainMenu.OnQuitApplicationPressed += QuitApplication;
            }

            private void StartSinglePlayer()
            {
                _Controller.StartSinglePlayer();
            }

            private void StartMultiplayer()
            {
                _Controller.StartMultiPlayer();
            }

            private void QuitApplication()
            {
                _Controller.QuitApplication();
            }

            public override void Deactivate()
            {
                UnsubscribeForMenuEvents();
                DestroyViewElements();
                RealeseAllAssets();
            }

            private void DestroyViewElements()
            {
                _instantiateService.DestroyObject(_mainMenuObject);
                _instantiateService.DestroyObject(_canvasObject);
            }

            private void RealeseAllAssets()
            {
                Addressables.Release(_mainMenuHandle);
                Addressables.Release(_canvasHandle);
            }

            public override void Deinitialize()
            {
                UnsubscribeForMenuEvents();
                DeinitializeViewElements();
            }

            private void DeinitializeViewElements()
            {
                if (_mainMenuHandle.IsValid() || _canvasHandle.IsValid()) 
                {
                    DestroyViewElements();
                    RealeseAllAssets();
                }
            }

            public void SetStartupAnimationAvailability(bool allowed)
            {
                _startupAnimationsAllowed = allowed;
            }
        }
    }
}