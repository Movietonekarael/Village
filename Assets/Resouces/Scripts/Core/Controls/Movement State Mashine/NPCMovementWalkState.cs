using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameCore.GameMovement
{
    public abstract partial class NPCMovement
    {
        public class NPCMovementWalkState : NPCMovementAnyMoveState
        {
            public NPCMovementWalkState(NPCMovement currentStateMachine) 
            : base(currentStateMachine) {}

            public override void UpdateState()
            {
                base.UpdateState();
                if (StateMachine._isRunning)
                    SetRunState();
            }

            protected override Vector3 SetLimitedTargetVelocity(Vector3 vec)
            {
                return vec.normalized * StateMachine._walkSpeedLimit;
            }

            //----------------------------------------------------------Local methods------------------------------------------------------//

            private void SetRunState()
            {
                SwitchState(StateMachine._runState);
            }
        }
    }
}

