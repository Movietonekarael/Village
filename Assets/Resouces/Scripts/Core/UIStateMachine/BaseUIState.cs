using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore.GUI
{
    public partial class UIStateMachine
    {
        public abstract class BaseUIState
        {
            private readonly UIStateMachine _stateMachine;
            protected UIStateMachine StateMachine { get { return _stateMachine; } }


            public BaseUIState(UIStateMachine stateMachine)
            {
                _stateMachine = stateMachine;
            }


            public abstract void EnterState();
            public abstract void ExitState();

            protected void SwitchState(BaseUIState newState)
            {
                ExitState();

                newState.EnterState();

                _stateMachine._currentState = newState;
            }
        }
    }
}

