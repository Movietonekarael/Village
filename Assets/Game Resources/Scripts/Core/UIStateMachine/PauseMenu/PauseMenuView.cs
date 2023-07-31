using GameCore.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;


namespace GameCore
{
    namespace GUI
    {
        public sealed class PauseMenuView : UIView<PauseMenuViewParameters, IPauseMenuController, IPauseMenuView>, IPauseMenuView
        {
            private const string _CANVAS_NAME = "PauseMenuCanvas";
            private GameObject _canvasPrefab => _Parameters.CanvasPrefab;
            private GameObject _canvasObject;
            private GameObject _pauseMenuPrefab => _Parameters.PauseMenuPrefab;

            [Inject] private readonly InstantiateService _instantiateService;
            [Inject(Id = "UiCamera")] private readonly Camera _uiCamera;
            [Inject] private readonly UiSelectionService _uiSelectionService;

            private PauseMenu _pauseMenu;

            public override void Activate()
            {
                SetSelectedButton();
                _pauseMenu.OnContinuePressed += ContinuePressed;
                _pauseMenu.OnQuitPressed += QuitPressed;
                _canvasObject.SetActive(true);
            }

            public override void Deactivate()
            {
                _pauseMenu.OnContinuePressed -= ContinuePressed;
                _pauseMenu.OnQuitPressed -= QuitPressed;
                _canvasObject.SetActive(false);
            }

            public override void Deinitialize()
            {
                _instantiateService.DestroyObject(_canvasObject);
            }

            protected override void InstantiateViewElements()
            {
                InstantiateCanvas();
                InstantiatePauseMenu();
                Deactivate();
            }

            private void InstantiateCanvas()
            {
                _canvasObject = _instantiateService.InstantiateObject(_canvasPrefab);
                _canvasObject.name = _CANVAS_NAME;
                var canvas = _canvasObject.GetComponent<Canvas>();
                canvas.worldCamera = _uiCamera;
            }

            private void InstantiatePauseMenu()
            {
                var pauseMenuObject = _instantiateService.InstantiateObjectWithInjections(_pauseMenuPrefab, _canvasObject.transform);
                _pauseMenu = pauseMenuObject.GetComponent<PauseMenu>();
            }

            private void ContinuePressed()
            {
                _Controller.ContinueGame();
            }

            private void QuitPressed()
            {
                _Controller.QuitGame();
            }

            private void SetSelectedButton()
            {
                _uiSelectionService.CurrentSelected = _pauseMenu.ContinueUiSelecter;
            }
        }
    }
}