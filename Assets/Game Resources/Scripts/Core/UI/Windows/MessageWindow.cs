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
            public sealed class MessageWindow
            {
                private const string _MESSAGE_WINDOW_KEY = "UI Staff/Message Window.prefab";

                public event Action OnOkPressed;

                private IMessageWindowMono _window;

                [Inject] private readonly InstantiateService _instantiateService;

                private AsyncOperationHandle<GameObject> _handle;


                public async void CreateNewWindow(string message, Transform canvasTransform)
                {
                    _handle = Addressables.LoadAssetAsync<GameObject>(_MESSAGE_WINDOW_KEY);
                    await _handle.Task;

                    if (_handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        var windowPrefab = _handle.Result;
                        var windowObject = _instantiateService.InstantiateObject(windowPrefab, canvasTransform);
                        _window = windowObject.GetComponent<IMessageWindowMono>();
                        _window.OnOkPressed += CloseMessageWindow;
                        _window.SetMessage(message);
                    }
                }

                private void CloseMessageWindow()
                {
                    _window.OnOkPressed -= CloseMessageWindow;
                    _window.Close();
                    OnOkPressed?.Invoke();
                    Addressables.Release(_handle);
                }
            }
        }
    }
}