using GameCore.GameControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace GameCore.GUI
{
    public sealed class PauseMenuUIState : BaseUIState<PauseMenuViewParameters, IPauseMenuController>
    {
        [Inject] private readonly IEscapable _escape;

        [Header("Next state")]
        [SerializeField]
        [RequireInterface(typeof(IUIState))]
        private UnityEngine.Object _mainScreenStateBase;

        private IUIState _mainScreenState { get => _mainScreenStateBase as IUIState; }

        protected override void StartState()
        {
            _escape.OnEscape += ContinueGame;
            _Controller.OnContinueGame += ContinueGame;
        }

        protected override void EndState()
        {
            _escape.OnEscape -= ContinueGame;
            _Controller.OnContinueGame -= ContinueGame;
        }

        private void ContinueGame()
        {
            SwitchState(_mainScreenState);
        }
    }
}

