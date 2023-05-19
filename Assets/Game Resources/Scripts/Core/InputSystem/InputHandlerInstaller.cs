using GameCore.GameControls;
using GameCore.GUI;
using UnityEngine;
using Zenject;


namespace GameCore.Installers
{
    public class InputHandlerInstaller : MonoInstaller
    {
        [SerializeField] private InputHandler _inputHandler;
        [SerializeField] private Transform _parentTransform;

        public override void InstallBindings()
        {
            var inputHandlerInstance = CreateInstance();
            BindInputHandler(inputHandlerInstance);
        }

        private InputHandler CreateInstance()
        {
            return Container.InstantiatePrefabForComponent<InputHandler>(_inputHandler, _parentTransform);
        }

        private void BindInputHandler(InputHandler instance)
        {
            Container.Bind<InputHandler>().FromInstance(instance).AsSingle().NonLazy();
        }
    }
}
