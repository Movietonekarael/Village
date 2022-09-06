using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.GameMovement
{
    partial class NPCMovement
    {
        public class NPCMovementGroundedState : NPCMovementBaseState, IRootState
        {
            public NPCMovementGroundedState(NPCMovement currentStateMachine)
            : base(currentStateMachine)
            {
                var state = StateMachine._idleState;
                state.EnterState();
                SetSubState(state);
            }

            public override void EnterState()
            {
                StateMachine.OnRunningStateChangedEvent += ChangeRunState;
                StateMachine.OnJumpEvent += HandleJump;

                StateMachine._characterActor.alwaysNotGrounded = false;
            }

            public override void UpdateState() { }

            public override void FixedUpdateState() 
            {
                HandleGravity();
            }

            public override void ExitState()
            {
                StateMachine.OnRunningStateChangedEvent -= ChangeRunState;
                StateMachine.OnJumpEvent -= HandleJump;
            }


            //----------------------------------------------------------Local methods------------------------------------------------------//

            private void HandleJump()
            {
                if (StateMachine._characterActor.IsGrounded)
                {
                    SwitchState(StateMachine._jumpState);
                }

            }

            private void ChangeRunState()
            {
                StateMachine._isRunning = !StateMachine._isRunning;
                StateMachine._animatorController.SetBool(StateMachine._isRunningBoolHash, StateMachine._isRunning);
            }



        }
    }
}
