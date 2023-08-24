using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace Installers
    {
        public sealed class PlayerInputInstaller : MonoInstaller
        {
            [SerializeField] private UnityEngine.InputSystem.PlayerInput _playerInput;
            [SerializeField] private Transform _parentTransform;

            public override void InstallBindings()
            {
                var playerInputInstance = CreateInstance();
                BindPlayerInput(playerInputInstance);
            }

            private UnityEngine.InputSystem.PlayerInput CreateInstance()
            {
                return Container.InstantiatePrefabForComponent<UnityEngine.InputSystem.PlayerInput>(_playerInput, _parentTransform);
            }

            private void BindPlayerInput(UnityEngine.InputSystem.PlayerInput playerInputInstance)
            {
                Container.Bind<UnityEngine.InputSystem.PlayerInput>().FromInstance(playerInputInstance).AsSingle().NonLazy();
            }
        }
    }
}