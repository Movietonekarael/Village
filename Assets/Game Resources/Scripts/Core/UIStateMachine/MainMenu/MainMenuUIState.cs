using Unity.Collections;
using System.Collections;
using UnityEngine;
using log4net.Util;

namespace GameCore
{
    namespace GUI
    {
        public sealed class MainMenuUIState : BaseUIState<MainMenuViewParameters, MainMenuController, IMainMenuController>
        {
            [Header("Multiplayer menu state")]
            [SerializeField]
            [RequireInterface(typeof(IUIState))]
            private UnityEngine.Object _multiplayerMenuStateBase;
            private IUIState _multiplayerMenuState { get => _multiplayerMenuStateBase as IUIState; }

            [SerializeField] private bool _startupAnimationsAllowed = true;
            public bool StartupAnimationAllowed
            {
                get => _startupAnimationsAllowed;
                set
                {
                    _startupAnimationsAllowed = value;
                    StartupAnimationAllowedChanged();
                }
            }

            protected override void StartState(params bool[] args)
            {
                _Controller.OnStartMultiplayer += StartMultiplayer;
                SetStartupAnimationBool();
                _Controller.SetStartupAnimationAvailability(_startupAnimationsAllowed);
            }

            protected override void EndState()
            {
                _Controller.OnStartMultiplayer -= StartMultiplayer;
            }

            private void StartMultiplayer()
            {
                SwitchState(_multiplayerMenuState);
            }

            private void SetStartupAnimationBool(params bool[] args)
            {
                if (args.Length > 0)
                {
                    _startupAnimationsAllowed = args[0];
                }
            }

            private void StartupAnimationAllowedChanged()
            {
                _Controller.SetStartupAnimationAvailability(_startupAnimationsAllowed);
            }
        }
    }
}