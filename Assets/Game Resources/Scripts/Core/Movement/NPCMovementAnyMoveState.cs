using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Implementation;

namespace GameCore.GameMovement
{
    public abstract partial class NPCMovementStateMachine
    {
        public abstract class NPCMovementAnyMoveState : NPCMovementBaseState
        {
            public NPCMovementAnyMoveState(NPCMovementStateMachine currentStateMachine) 
            : base(currentStateMachine) {}

            protected MotionParameters currentMotionParameters = new();

            public override void EnterState()
            {
                _StateMachine._animatorController.SetBool(_StateMachine._isWalkingBoolHash, true);
                
                _Movement.OnMovementFinish += SetIdleState;
                _Movement.OnMovement += Move;
            }

            public override void UpdateState() { }

            public override void FixedUpdateState()
            {
                HandleMoving();
                HandleRotation();
            }

            private void HandleMoving()
            {
                _StateMachine.LocalDirectionOfMovingChanged();

                var globalDirection = new Vector3(_StateMachine._GlobalDirectionOfMoving.x,
                                                  .0f,
                                                  _StateMachine._GlobalDirectionOfMoving.y);

                var targetVelocity = SetLimitedTargetVelocity(globalDirection);
                var characterActor = _StateMachine._characterActor;
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
                                                                    currentMotionParameters.Acceleration : 
                                                                    currentMotionParameters.Deceleration) * 
                                                                    Time.deltaTime);

            }
            
            private void SetMotionValues(CharacterActor characterActor)
            {
                switch (characterActor.CurrentState)
                {
                    case CharacterActorState.StableGrounded:
                        currentMotionParameters = _StateMachine._stableGroundedParameters;
                        break;
                    case CharacterActorState.UnstableGrounded:
                        currentMotionParameters = _StateMachine._unstableGroundedParameters;
                        break;
                    case CharacterActorState.NotGrounded:
                        currentMotionParameters = _StateMachine._notGroundedParameters;
                        break;
                }
            }

            protected virtual Vector3 SetLimitedTargetVelocity(Vector3 vec)
            {
                return vec.normalized;
            }

            private void HandleRotation()
            {
                _StateMachine._characterActor.Rotation = 
                    Quaternion.Lerp(_StateMachine.transform.rotation, 
                    _StateMachine._NeededRotation, 
                    Time.deltaTime * _StateMachine._RotationSpeed);
            }

            private void Move(Vector2 dir)
            {
                _StateMachine._localDirectionOfMoving = dir;
            }

            private void SetIdleState()
            {
                SwitchState(_StateMachine._idleState);
            }
            
            public override void ExitState()
            {
                _Movement.OnMovementFinish -= SetIdleState;
                _Movement.OnMovement -= Move;
            }

        }
    }
}