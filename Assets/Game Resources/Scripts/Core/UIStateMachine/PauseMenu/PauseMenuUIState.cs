using GameCore.GameControls;
using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace GUI
    {
        public sealed class PauseMenuUIState : BaseUIState<PauseMenuViewParameters, PauseMenuController, IPauseMenuController>
        {
            [Inject] private readonly IEscapable _escape;

            [Header("Next state")]
            [SerializeField]
            [RequireInterface(typeof(IUIState))]
            private UnityEngine.Object _mainScreenStateBase;

            private IUIState _mainScreenState { get => _mainScreenStateBase as IUIState; }

            protected override void StartState(params bool[] args)
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
}