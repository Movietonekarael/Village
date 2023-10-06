using UnityEngine;
using Lightbug.CharacterControllerPro.Core;


namespace GameCore
{
    namespace GameMovement
    {
        public abstract partial class NPCMovementStateMachine
        {
            public class NPCMovementIdleState : NPCMovementBaseState
            {
                public NPCMovementIdleState(NPCMovementStateMachine currentStateMachine)
                : base(currentStateMachine) { }

                public override void EnterState()
                {
                    if (_StateMachine.AnimatorController)
                        _StateMachine.AnimatorController.SetBool(_StateMachine._isWalkingBoolHash, false);

                    _Movement.OnMovementStart += StartMovement;
                }

                public override void UpdateState() { }

                public override void FixedUpdateState()
                {
                    HandleMoving();
                }

                public override void ExitState()
                {
                    _Movement.OnMovementStart -= StartMovement;
                }

                //-------------------------------------------------------Local methods-----------------------------------------------------------//

                private void StartMovement()
                {
                    if (!_StateMachine._isRunning)
                        SwitchState(_StateMachine._walkState);
                    else
                        SwitchState(_StateMachine._runState);
                }

                private void HandleMoving()
                {
                    var characterActor = _StateMachine.CharacterActor;

                    var deceleration = characterActor.CurrentState switch
                    {
                        CharacterActorState.StableGrounded => _StateMachine._stableGroundedParameters.Deceleration,
                        CharacterActorState.UnstableGrounded => _StateMachine._unstableGroundedParameters.Deceleration,
                        CharacterActorState.NotGrounded => _StateMachine._notGroundedParameters.Deceleration,
                        _ => 0f,
                    };

                    characterActor.PlanarVelocity = Vector3.MoveTowards(characterActor.PlanarVelocity,
                                                                        Vector3.zero,
                                                                        deceleration *
                                                                        Time.deltaTime);
                }
            }
        }
    }
}