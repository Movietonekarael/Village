namespace GameCore
{
    namespace GameMovement
    {
        partial class NPCMovementStateMachine
        {
            public class NPCMovementGroundedState : NPCMovementBaseState, IRootState
            {
                public NPCMovementGroundedState(NPCMovementStateMachine currentStateMachine)
                : base(currentStateMachine)
                {
                    var state = _StateMachine._idleState;
                    state.EnterState();
                    SetSubState(state);
                }

                public override void EnterState()
                {
                    _Movement.OnRunningChanged += ChangeRunState;
                    _Movement.OnJumped += HandleJump;

                    if (_StateMachine.CharacterActor != null)
                        _StateMachine.CharacterActor.alwaysNotGrounded = false;
                }

                public override void UpdateState() { }

                public override void FixedUpdateState()
                {
                    HandleGravity();
                }

                public override void ExitState()
                {
                    _Movement.OnRunningChanged -= ChangeRunState;
                    _Movement.OnJumped -= HandleJump;
                }


                //----------------------------------------------------------Local methods------------------------------------------------------//

                private void HandleJump()
                {
                    if (_StateMachine.CharacterActor.IsGrounded)
                    {
                        SwitchState(_StateMachine._jumpState);
                    }

                }

                private void ChangeRunState()
                {
                    _StateMachine._isRunning = !_StateMachine._isRunning;
                    if (_StateMachine.AnimatorController != null)
                        _StateMachine.AnimatorController.SetBool(_StateMachine._isRunningBoolHash, _StateMachine._isRunning);
                }
            }
        }
    }
}