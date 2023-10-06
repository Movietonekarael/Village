namespace GameCore
{
    namespace GameMovement
    {
        partial class NPCMovementStateMachine
        {
            public class NPCMovementJumpState : NPCMovementBaseState, IRootState
            {
                public NPCMovementJumpState(NPCMovementStateMachine currentStateMachine)
                : base(currentStateMachine) { }

                public override void EnterState()
                {
                    _StateMachine.OnJumpStartEnded += JumpStartEnded;
                    _StateMachine.OnJumpEnded += JumpEnded;

                    if (_StateMachine.AnimatorController)
                        _StateMachine.AnimatorController.SetBool(_StateMachine._isJumpingBoolHash, true);
                    HandleJump();
                }

                public override void UpdateState()
                {
                    CheckJumpEnd();
                    CheckJumpInterruption();
                }

                public override void FixedUpdateState()
                {
                    HandleGravity();
                }

                public override void ExitState()
                {
                    _StateMachine.OnJumpStartEnded -= JumpStartEnded;
                    _StateMachine.OnJumpEnded -= JumpEnded;

                    if (_StateMachine.AnimatorController)
                    {
                        _StateMachine.AnimatorController.SetBool(_StateMachine._isJumpingBoolHash, false);
                        _StateMachine.AnimatorController.SetBool(_StateMachine._isJumpingEndBoolHash, false);
                    }
                }

                //----------------------------------------------------------Local methods------------------------------------------------------//

                private void JumpStartEnded()
                {
                    _StateMachine._isJumping = true;
                }

                private void JumpEnded()
                {
                    _StateMachine._isJumping = false;

                    SwitchState(_StateMachine._groundedState);
                }

                private void CheckJumpEnd()
                {
                    if (_StateMachine._isJumping && _StateMachine.CharacterActor.IsFalling)
                    {
                        EndJumpAnimation();
                    }


                    void EndJumpAnimation()
                    {
                        if (_StateMachine.AnimatorController)
                        {
                            _StateMachine.AnimatorController.SetBool(_StateMachine._isJumpingEndBoolHash, true);
                        }
                    }
                }

                private void CheckJumpInterruption()
                {
                    if (_StateMachine._isJumping && _StateMachine.CharacterActor.IsGrounded)
                    {
                        InterruptJump();
                        SwitchState(_StateMachine._groundedState);
                    }


                    void InterruptJump()
                    {
                        if (_StateMachine.AnimatorController)
                        {
                            _StateMachine.AnimatorController.SetTrigger(_StateMachine._interruptJumpTriggerHash);
                        }
                    }
                }

                private void HandleJump()
                {
                    var characterActor = _StateMachine.CharacterActor;
                    var jumpDirection = characterActor.Up;

                    characterActor.ForceNotGrounded();
                    characterActor.Velocity += jumpDirection * _StateMachine._jumpVelocityImpulse;
                }
            }
        }
    }
}