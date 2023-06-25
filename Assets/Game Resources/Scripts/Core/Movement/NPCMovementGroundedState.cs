using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.GameMovement
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

                _StateMachine._characterActor.alwaysNotGrounded = false;
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
                if (_StateMachine._characterActor.IsGrounded)
                {
                    SwitchState(_StateMachine._jumpState);
                }

            }

            private void ChangeRunState()
            {
                _StateMachine._isRunning = !_StateMachine._isRunning;
                _StateMachine._animatorController.SetBool(_StateMachine._isRunningBoolHash, _StateMachine._isRunning);
            }



        }
    }
}
