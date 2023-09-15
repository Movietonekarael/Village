using GameCore.GameControls;
using UnityEngine;


namespace GameCore
{
    namespace GameMovement
    {
        partial class NPCMovementStateMachine
        {
            public abstract class NPCMovementBaseState
            {
                protected IMovement _Movement => _stateMachine._Movement;

                private NPCMovementStateMachine _stateMachine;
                private NPCMovementBaseState _currentSuperState;
                private NPCMovementBaseState _currentSubState;

                protected NPCMovementStateMachine _StateMachine { get { return _stateMachine; } }

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
                    var characterActor = _StateMachine.CharacterActor;

                    if (characterActor != null && !characterActor.IsStable)
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
}