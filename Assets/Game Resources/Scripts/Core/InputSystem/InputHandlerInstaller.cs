using GameCore.GameControls;
using GameCore.GUI;
using System.ComponentModel;
using UnityEngine;
using Zenject;


namespace GameCore.Installers
{
    public class InputHandlerInstaller : MonoInstaller
    {
        [SerializeField] private InputHandler _inputHandler;
        [SerializeField] private Transform _parentTransform;

        [SerializeField] private bool _cameraRotatorAllowed = true;
        [SerializeField] private bool _casmeraZoomAllowed = true;
        [SerializeField] private bool _movementAllowed = true;
        [SerializeField] private bool _inventoryKeysPressAllowed = true;
        [SerializeField] private bool _openCloseInventoryAllowed = true;

        public override void InstallBindings()
        {
            var inputHandlerInstance = CreateInstance();
            BindInputHandler(inputHandlerInstance);
            BindInterface<InputHandler, ICameraRotator>(inputHandlerInstance, _cameraRotatorAllowed);
            BindInterface<InputHandler, ICameraZoomer>(inputHandlerInstance, _casmeraZoomAllowed);
            BindInterface<InputHandler, IMovement>(inputHandlerInstance, _movementAllowed);
            BindInterface<InputHandler, IInventoryPress>(inputHandlerInstance, _inventoryKeysPressAllowed);
            BindInterface<InputHandler, IOpenCloseInventory>(inputHandlerInstance, _openCloseInventoryAllowed);
        }

        private InputHandler CreateInstance()
        {
            return Container.InstantiatePrefabForComponent<InputHandler>(_inputHandler, _parentTransform);
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
