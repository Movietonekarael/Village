using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.GameMovement
{
    public abstract partial class NPCMovement
    {
        public class NPCMovementRunState : NPCMovementAnyMoveState
        {
            private bool _dashed = false;

            public NPCMovementRunState(NPCMovement currentStateMachine) 
            : base(currentStateMachine) { }

            public override void EnterState()
            {
                base.EnterState();

                StateMachine.OnDashedEvent += Dash;
            }

            public override void UpdateState()
            {
                base.UpdateState();

                if (!StateMachine._isRunning)
                    SetWalkState();
            }

            public override void ExitState()
            {
                base.ExitState();

                _dashed = false;
                StateMachine.OnDashedEvent -= Dash;
            }

            //----------------------------------------------------------Local methods------------------------------------------------------//

            private void SetWalkState()
            {
                SwitchState(StateMachine._walkState);
            }

            private void Dash()
            {
                if (!_dashed)
                {
                    _dashed = true;
                    StateMachine._animatorController.SetTrigger(StateMachine._dashTriggerHash);
                }
            }

            protected override Vector3 SetLimitedTargetVelocity(Vector3 vec)
            {
                if (!_dashed)
                    return vec.normalized * StateMachine._slowRunSpeedLimit;
                else
                    return vec.normalized * StateMachine._fastRunSpeedLimit;
            }
        }
    }
}

