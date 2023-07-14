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

            [Inject(Id = "UiCamera")] private readonly Camera _uiCamera;
            [Inject] private readonly EventSystem _eventSystem;

            private PauseMenu _pauseMenu;

            public override void Activate()
            {
                SetSubmitButton();
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
                InstantiateService.DestroyObject(_canvasObject);
            }

            protected override void InstantiateViewElements()
            {
                InstantiateCanvas();
                InstantiatePauseMenu();
                Deactivate();
            }

            private void InstantiateCanvas()
            {
                _canvasObject = InstantiateService.InstantiateObject(_canvasPrefab);
                _canvasObject.name = _CANVAS_NAME;
                var canvas = _canvasObject.GetComponent<Canvas>();
                canvas.worldCamera = _uiCamera;
            }

            private void InstantiatePauseMenu()
            {
                var pauseMenuObject = InstantiateService.InstantiateObject(_pauseMenuPrefab, _canvasObject.transform);
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

            private void SetSubmitButton()
            {
                _eventSystem.SetSelectedGameObject(_pauseMenu.ContinueButton);
            }
        }
    }
}