using GameCore.Services;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;


namespace GameCore
{
    namespace GUI
    {
        namespace Windows
        {
            public sealed class ConnectionCodeWindow
            {
                private const string _WINDOW_ASSET_KEY = "UI Staff/Menu/Code Enter Window.prefab";

                public event Action<string> OnOkPressed;

                private bool _windowIsCreated = false;
                private IConnectionCodeWindowMono _window;

                [Inject] private readonly InstantiateService _instantiateService;

                private AsyncOperationHandle<GameObject> _handle;


                public void CreateWindow(Transform canvasTransform)
                {
                    if (!_windowIsCreated)
                        CreateNewWindow(canvasTransform);
                    else
                        Debug.LogWarning("Connection code window is already created.");
                }

                private async void CreateNewWindow(Transform canvasTransform)
                {
                    _handle = Addressables.LoadAssetAsync<GameObject>(_WINDOW_ASSET_KEY);
                    await _handle.Task;

                    if (_handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        var windowPrefab = _handle.Result;
                        var windowObject = _instantiateService.InstantiateObject(windowPrefab, canvasTransform);
                        _window = windowObject.GetComponent<IConnectionCodeWindowMono>();
                        _window.OnOkPressed += HandleCode;
                        _windowIsCreated = true;
                    }
                }

                public void HandleCode(string code)
                {
                    Debug.Log($"Code: {code}");
                    _window.OnOkPressed -= HandleCode;
                    _window.Close();
                    _windowIsCreated = false;
                    OnOkPressed?.Invoke(code);
                    Addressables.Release(_handle);
                }
            }
        }
    }
}