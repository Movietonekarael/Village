using UnityEngine;


namespace GameCore
{
    namespace GameMovement
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


                    void SetWalkState()
                    {
                        SwitchState(_StateMachine._walkState);
                    }
                }

                public override void ExitState()
                {
                    base.ExitState();

                    _dashed = false;
                    _Movement.OnDashed -= Dash;
                }


                private void Dash()
                {
                    if (!_dashed)
                    {
                        _dashed = true;
                        _StateMachine.AnimatorController.SetTrigger(_StateMachine._dashTriggerHash);
                    }
                }

                protected override Vector3 SetLimitedTargetVelocity(Vector3 vec)
                {
                    if (!_dashed)
                        return vec.normalized * _StateMachine._slowRunVelocityLimit;
                    else
                        return vec.normalized * _StateMachine._fastRunVelocityLimit;
                }
            }
        }
    }
}