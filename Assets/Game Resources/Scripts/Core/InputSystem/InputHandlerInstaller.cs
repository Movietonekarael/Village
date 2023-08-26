using GameCore.GameControls;
using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace Installers
    {
        public sealed class InputHandlerInstaller : MonoInstaller
        {
            private const string _INPUT_HANDLER_NAME = "InputHandler";

            [SerializeField] private InputHandler _inputHandler;
            [SerializeField] private Transform _parentTransform;

            [SerializeField] private bool _cameraRotatorAllowed = true;
            [SerializeField] private bool _casmeraZoomAllowed = true;
            [SerializeField] private bool _movementAllowed = true;
            [SerializeField] private bool _inventoryKeysPressAllowed = true;
            [SerializeField] private bool _openCloseInventoryAllowed = true;
            [SerializeField] private bool _pauseAllowed = true;

            public override void InstallBindings()
            {
                var inputHandlerInstance = CreateInstance();
                BindInputHandler(inputHandlerInstance);
                BindInterface<InputHandler, ICameraRotator>(inputHandlerInstance, _cameraRotatorAllowed);
                BindInterface<InputHandler, ICameraZoomer>(inputHandlerInstance, _casmeraZoomAllowed);
                BindInterface<InputHandler, IMovement>(inputHandlerInstance, _movementAllowed);
                BindInterface<InputHandler, IInventoryPress>(inputHandlerInstance, _inventoryKeysPressAllowed);
                BindInterface<InputHandler, IOpenCloseInventory>(inputHandlerInstance, _openCloseInventoryAllowed);
                BindInterface<InputHandler, IEscapable>(inputHandlerInstance, _pauseAllowed);
            }

            private InputHandler CreateInstance()
            {
                var inputHandler = Container.InstantiatePrefabForComponent<InputHandler>(_inputHandler, _parentTransform);
                ChangeName(inputHandler.gameObject);
                return inputHandler;
            }

            private void ChangeName(GameObject gameObject)
            {
                gameObject.name = _INPUT_HANDLER_NAME;
            }

            private void BindInputHandler(InputHandler instance)
            {
                Container.Bind<InputHandler>().FromInstance(instance).AsSingle().NonLazy();
            }

            private void BindInterface<InputHandler, I>(InputHandler instance, bool isAllowed) where InputHandler : I
            {
                if (isAllowed)
                {
                    Container.Bind<I>().To<InputHandler>().FromInstance(instance).AsCached().NonLazy();
                }
            }
        }
    }
}