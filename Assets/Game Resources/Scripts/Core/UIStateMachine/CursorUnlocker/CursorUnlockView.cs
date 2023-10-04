using GameCore.GameControls;
using UnityEngine;
using Zenject;
using UnityEngine.EventSystems;
using GameCore.Services;

namespace GameCore
{
    namespace GUI
    {
        public sealed class CursorUnlockView : UIView<CursorUnlockViewParameters, ICursorUnlockController, ICursorUnlockView>, ICursorUnlockView
        {
            private const string _CANVAS_NAME = "VirtualPointerCanvas";
            private GameObject _canvasPrefab => _Parameters.CanvasPrefab;
            private GameObject _canvasObject;
            private GameObject _virtualPointerPrefab => _Parameters.VirtualPointerPrefab;

            [Inject] private readonly InstantiateService _instantiateService;
            [Inject] private readonly InputHandler _inputHandler;
            [Inject] private readonly UiSelectionService _uiSelectionService;
            [Inject(Id = "UiCamera")] private readonly Camera _uiCamera;


            public override void Activate()
            {
                _canvasObject.SetActive(true);
            }

            public override void Deactivate() 
            {
                if (_canvasObject != null)
                    _canvasObject.SetActive(false);
            }

            public override void Deinitialize() 
            {
                _instantiateService.DestroyObject(_canvasObject);
            }

            protected override void InstantiateViewElementsOnAwake()
            {
                InstantiateCanvas();
                InstantiateVirtualPointer();
            }

            public void DisableSelection()
            {
                _uiSelectionService.SelectionEnabled = false;
            }

            public void EnableSelection()
            {
                _uiSelectionService.SelectionEnabled = true;
            }

            private void InstantiateCanvas()
            {
                if (_canvasPrefab == null)
                    return;

                _canvasObject = _instantiateService.InstantiateObject(_canvasPrefab);
                _canvasObject.name = _CANVAS_NAME;

                SetCanvasSettings(_canvasObject);
                SetCanvasTransform(_canvasObject);
            }

            private void SetCanvasSettings(GameObject canvasObject)
            {
                var canvas = canvasObject.GetComponent<Canvas>();
                canvas.worldCamera = _uiCamera;
                canvas.sortingOrder = 1000;
            }

            private void SetCanvasTransform(GameObject canvasObject)
            {
                var transform = canvasObject.GetComponent<RectTransform>();
                _inputHandler.CanvasTransform = transform;
            }

            private void InstantiateVirtualPointer()
            {
                if (_virtualPointerPrefab == null)
                    return;

                var virtualPointerObject = _instantiateService.InstantiateObject(_virtualPointerPrefab, _canvasObject.transform);
                _inputHandler.PointerTransform = virtualPointerObject.GetComponent<RectTransform>();
            }
        }
    }
}