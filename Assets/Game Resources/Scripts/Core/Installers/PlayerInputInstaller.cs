using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace Installers
    {
        public sealed class PlayerInputInstaller : MonoInstaller
        {
            private const string _PLAYER_INPUT_NAME = "PlayerInput";

            [SerializeField] private UnityEngine.InputSystem.PlayerInput _playerInput;
            [SerializeField] private Transform _parentTransform;

            public override void InstallBindings()
            {
                var playerInputInstance = CreateInstance();
                BindPlayerInput(playerInputInstance);
            }

            private UnityEngine.InputSystem.PlayerInput CreateInstance()
            {
                var playerInput = Container.InstantiatePrefabForComponent<UnityEngine.InputSystem.PlayerInput>(_playerInput, _parentTransform);
                ChangeName(playerInput.gameObject);
                return playerInput;
            }

            private void ChangeName(GameObject gameObject)
            {
                gameObject.name = _PLAYER_INPUT_NAME;
            }

            private void BindPlayerInput(UnityEngine.InputSystem.PlayerInput playerInputInstance)
            {
                Container.Bind<UnityEngine.InputSystem.PlayerInput>().FromInstance(playerInputInstance).AsSingle().NonLazy();
            }
        }
    }
}