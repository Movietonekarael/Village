using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.GameMovement
{
    partial class NPCMovementStateMachine
    {
        public abstract class NPCMovementBaseState
        {
            private NPCMovementStateMachine _stateMachine;
            private NPCMovementBaseState _currentSuperState;
            private NPCMovementBaseState _currentSubState;

            protected NPCMovementStateMachine StateMachine { get { return _stateMachine; } }

            public NPCMovementBaseState(NPCMovementStateMachine currentStateMachine)
            {
                _stateMachine = currentStateMachine;
            }


            public abstract void EnterState();
            public abstract void UpdateState();
            public abstract void FixedUpdateState();
            public abstract void ExitState();


            protected void HandleGravity()
            {
                var gravity = 2 * 2.25f / Mathf.Pow(0.5f, 2);
                var characterActor = StateMachine._characterActor;

                if (!characterActor.IsStable)
                    characterActor.VerticalVelocity += gravity * Time.deltaTime * -characterActor.Up;
            }

            public void UpdateStates() 
            { 
                UpdateState();
                if (_currentSubState != null)
                    _currentSubState.UpdateStates();
            }

            public void FixedUpdateStates() 
            {
                FixedUpdateState();
                if (_currentSubState != null)
                    _currentSubState.FixedUpdateStates();
            }

            protected void SwitchState(NPCMovementBaseState newState)
            {
                ExitState();

                newState.EnterState();

                if (this is IRootState)
                {
                    newState.SetSubState(_currentSubState);
                    _stateMachine._currentState = newState;
                }
                else if (_currentSuperState != null)
                    _currentSuperState.SetSubState(newState);
            }

            protected void SetSuperState(NPCMovementBaseState newSuperState) 
            {
                _currentSuperState = newSuperState;
            }

            protected void SetSubState(NPCMovementBaseState newSubState) 
            {
                _currentSubState = newSubState;
                newSubState.SetSuperState(this);
            }

        }
    }
}

