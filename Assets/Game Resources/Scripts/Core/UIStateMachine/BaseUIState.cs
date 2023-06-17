using GameCore.GameControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace GameCore.GUI
{
    [RequireComponent(typeof(UIStateMachine))]
    public abstract class BaseUIState : MonoBehaviour
    {
        [Inject] protected InputHandler _InputHandler;
        private UIStateMachine _stateMachine;
        protected UIStateMachine _StateMachine { get { return _stateMachine; } }


        private void Awake()
        {
            _stateMachine = GetComponent<UIStateMachine>();
            OnAwake();
        }

        protected abstract void OnAwake();
        public abstract void EnterState();
        protected abstract void ExitState();

        protected void SwitchState(BaseUIState newState)
        {
            ExitState();

            newState.EnterState();

            _stateMachine.CurrentState = newState;
        }
    }
}

