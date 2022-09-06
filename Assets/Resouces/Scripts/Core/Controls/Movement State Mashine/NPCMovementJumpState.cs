using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.GameMovement
{
    partial class NPCMovement
    {
        public class NPCMovementJumpState : NPCMovementBaseState, IRootState
        {
            public NPCMovementJumpState(NPCMovement currentStateMachine) 
            : base(currentStateMachine) { }

            public override void EnterState()
            {
                StateMachine.OnJumpStartEndedEvent += JumpStartEnded;
                StateMachine.OnJumpEndedEvent += JumpEnded;

                StateMachine._animatorController.SetBool(StateMachine._isJumpingBoolHash, true);
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
                StateMachine.OnJumpStartEndedEvent -= JumpStartEnded;
                StateMachine.OnJumpEndedEvent -= JumpEnded;

                StateMachine._animatorController.SetBool(StateMachine._isJumpingBoolHash, false);
                StateMachine._animatorController.SetBool(StateMachine._isJumpingEndBoolHash, false); 
            }

            //----------------------------------------------------------Local methods------------------------------------------------------//

            private void JumpStartEnded()
            {
                StateMachine._isJumping = true;
            }

            private void JumpEnded()
            {
                StateMachine._isJumping = false;

                SwitchState(StateMachine._groundedState);
            }

            private void CheckJumpEnd()
            {
                if (StateMachine._isJumping && StateMachine._characterActor.IsFalling)
                {
                    StateMachine._animatorController.SetBool(StateMachine._isJumpingEndBoolHash, true);
                }
            }

            private void CheckJumpInterruption()
            {
                if (StateMachine._isJumping && StateMachine._characterActor.IsGrounded)
                {
                    StateMachine._animatorController.SetTrigger(StateMachine._interruptJumpTriggerHash);
                    SwitchState(StateMachine._groundedState);
                }
            }

            private void HandleJump()
            {
                var characterActor = StateMachine._characterActor;
                var jumpDirection = characterActor.Up;

                characterActor.ForceNotGrounded();
                characterActor.Velocity += jumpDirection * StateMachine._jumpSpeed;
            }
        }
    }
}
