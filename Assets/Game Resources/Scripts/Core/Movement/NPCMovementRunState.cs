using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.GameMovement
{
    public abstract partial class NPCMovementStateMachine
    {
        public class NPCMovementRunState : NPCMovementAnyMoveState
        {
            private bool _dashed = false;

            public NPCMovementRunState(NPCMovementStateMachine currentStateMachine) 
            : base(currentStateMachine) { }

            public override void EnterState()
            {
                base.EnterState();

                _Movement.OnDashed += Dash;
            }

            public override void UpdateState()
            {
                base.UpdateState();

                if (!_StateMachine._isRunning)
                    SetWalkState();
            }

            public override void ExitState()
            {
                base.ExitState();

                _dashed = false;
                _Movement.OnDashed -= Dash;
            }

            //----------------------------------------------------------Local methods------------------------------------------------------//

            private void SetWalkState()
            {
                SwitchState(_StateMachine._walkState);
            }

            private void Dash()
            {
                if (!_dashed)
                {
                    _dashed = true;
                    _StateMachine._animatorController.SetTrigger(_StateMachine._dashTriggerHash);
                }
            }

            protected override Vector3 SetLimitedTargetVelocity(Vector3 vec)
            {
                if (!_dashed)
                    return vec.normalized * _StateMachine._slowRunSpeedLimit;
                else
                    return vec.normalized * _StateMachine._fastRunSpeedLimit;
            }
        }
    }
}

