using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Implementation;


namespace GameCore.GameMovement
{
    public abstract partial class NPCMovement
    {
        public class NPCMovementIdleState : NPCMovementBaseState
        {
            public NPCMovementIdleState(NPCMovement currentStateMachine) 
            : base(currentStateMachine) {}

            public override void EnterState()
            {
                StateMachine._animatorController.SetBool(StateMachine._isWalkingBoolHash, false);
                
                StateMachine.OnMovementStartEvent += StartMovement;
            }

            public override void UpdateState() { }

            public override void FixedUpdateState()
            {
                HandleMoving();
            }

            public override void ExitState()
            {
                StateMachine.OnMovementStartEvent -= StartMovement;
            }

            //-------------------------------------------------------Local methods-----------------------------------------------------------//

            private void StartMovement()
            {
                if (!StateMachine._isRunning)
                    SwitchState(StateMachine._walkState);
                else
                    SwitchState(StateMachine._runState);
            }

            private void HandleMoving()
            {
                var characterActor = StateMachine._characterActor;

                var deceleration = characterActor.CurrentState switch
                {
                    CharacterActorState.StableGrounded => StateMachine._stableGroundedDeceleration,
                    CharacterActorState.UnstableGrounded => StateMachine._unstableGroundedDeceleration,
                    CharacterActorState.NotGrounded => StateMachine._notGroundedDeceleration,
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

