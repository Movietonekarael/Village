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


        protected virtual void Awake()
        {
            _stateMachine = GetComponent<UIStateMachine>();
        }

        public abstract void EnterState();
        public abstract void ExitState();

        protected void SwitchState(BaseUIState newState)
        {
            ExitState();

            newState.EnterState();

            _stateMachine.CurrentState = newState;
        }
    }
}

