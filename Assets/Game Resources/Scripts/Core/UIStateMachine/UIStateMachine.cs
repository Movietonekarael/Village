using UnityEngine;


namespace GameCore
{
    namespace GUI
    {
        public sealed class UIStateMachine : MonoBehaviour
        {
            [SerializeField] private bool _enterFirstStateOnStart = true;

            [SerializeField]
            [RequireInterface(typeof(IUIState))]
            private UnityEngine.Object _firstStateBase;

            private IUIState _firstState { get => _firstStateBase as IUIState; }

            public IUIState CurrentState { private get; set; }


            private void Start()
            {
                if (_enterFirstStateOnStart)
                    EnterFirstState();
            }

            public void StartStateMachine()
            {
                if (_enterFirstStateOnStart)
                    Debug.LogWarning("UI State Machine already started.");
                else
                    EnterFirstState();
            }

            private void EnterFirstState()
            {
                if (_firstState is not null)
                {
                    CurrentState = _firstState;
                    CurrentState.EnterState();
                }
            }

            private void OnDestroy()
            {
                CurrentState.ExitState();
            }
        }
    }
}