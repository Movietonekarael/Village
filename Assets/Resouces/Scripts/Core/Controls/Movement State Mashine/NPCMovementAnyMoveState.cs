using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Implementation;

namespace GameCore.GameMovement
{
    public abstract partial class NPCMovement
    {
        public abstract class NPCMovementAnyMoveState : NPCMovementBaseState
        {
            public NPCMovementAnyMoveState(NPCMovement currentStateMachine) 
            : base(currentStateMachine) {}

            protected MotionParameters currentMotionParameters = new();

            public override void EnterState()
            {
                StateMachine._animatorController.SetBool(StateMachine._isWalkingBoolHash, true);
                
                StateMachine.OnMovementFinishEvent += SetIdleState;
                StateMachine.OnMovementEvent += Move;
            }

            public override void UpdateState() { }

            public override void FixedUpdateState()
            {
                HandleMoving();
                HandleRotation();
            }

            private void HandleMoving()
            {
                StateMachine.LocalDirectionOfMovingChanged();

                var globalDirection = new Vector3(StateMachine._globalDirectionOfMoving.x,
                                                  .0f,
                                                  StateMachine._globalDirectionOfMoving.y);

                var targetVelocity = SetLimitedTargetVelocity(globalDirection);
                var characterActor = StateMachine._characterActor;
                var needToAccelerate = false;

                SetMotionValues(characterActor);


                switch (characterActor.CurrentState)
                {
                    case CharacterActorState.StableGrounded:
                        needToAccelerate = true;
                        break;
                    case CharacterActorState.UnstableGrounded:
                        needToAccelerate = targetVelocity.sqrMagnitude >= characterActor.PlanarVelocity.sqrMagnitude;
                        break;
                    case CharacterActorState.NotGrounded:
                        needToAccelerate = targetVelocity.sqrMagnitude >= characterActor.PlanarVelocity.sqrMagnitude;
                        break;
                }

                characterActor.PlanarVelocity = Vector3.MoveTowards(characterActor.PlanarVelocity, 
                                                                    targetVelocity, 
                                                                    (needToAccelerate ? 
                                                                    currentMotionParameters.acceleration : 
                                                                    currentMotionParameters.deceleration) * 
                                                                    Time.deltaTime);

            }
            
            private void SetMotionValues(CharacterActor characterActor)
            {
                switch (characterActor.CurrentState)
                {
                    case CharacterActorState.StableGrounded:
                        currentMotionParameters.acceleration = StateMachine._stableGroundedAcceleration;
                        currentMotionParameters.deceleration = StateMachine._stableGroundedDeceleration;
                        break;
                    case CharacterActorState.UnstableGrounded:
                        currentMotionParameters.acceleration = StateMachine._unstableGroundedAcceleration;
                        currentMotionParameters.deceleration = StateMachine._unstableGroundedDeceleration;
                        break;
                    case CharacterActorState.NotGrounded:
                        currentMotionParameters.acceleration = StateMachine._notGroundedAcceleration;
                        currentMotionParameters.deceleration = StateMachine._notGroundedDeceleration;
                        break;
                }
            }

            protected virtual Vector3 SetLimitedTargetVelocity(Vector3 vec)
            {
                return vec.normalized;
            }

            private void HandleRotation()
            {
                StateMachine._characterActor.Rotation = 
                    Quaternion.Lerp(StateMachine.transform.rotation, 
                    StateMachine.neededRotation, 
                    Time.deltaTime * StateMachine.rotationSpeed);
            }

            private void Move(Vector2 dir)
            {
                StateMachine._localDirectionOfMoving = dir;
            }

            private void SetIdleState()
            {
                SwitchState(StateMachine._idleState);
            }
            
            public override void ExitState()
            {
                StateMachine.OnMovementFinishEvent -= SetIdleState;
                StateMachine.OnMovementEvent -= Move;
            }

        }
    }
}