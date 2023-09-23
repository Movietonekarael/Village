using UnityEngine;
using Lightbug.CharacterControllerPro.Core;


namespace GameCore
{
    namespace GameMovement
    {
        public abstract partial class NPCMovementStateMachine
        {
            public abstract class NPCMovementAnyMoveState : NPCMovementBaseState
            {
                public NPCMovementAnyMoveState(NPCMovementStateMachine currentStateMachine)
                : base(currentStateMachine) { }

                protected MotionParameters _CurrentMotionParameters = new();

                public override void EnterState()
                {
                    _StateMachine.AnimatorController.SetBool(_StateMachine._isWalkingBoolHash, true);

                    _Movement.OnMovementFinish += SetIdleState;
                    _Movement.OnMovement += Move;
                }

                public override void ExitState()
                {
                    _Movement.OnMovementFinish -= SetIdleState;
                    _Movement.OnMovement -= Move;
                }

                private void Move(Vector2 dir)
                {
                    _StateMachine._LocalDirectionOfMoving = dir;
                }

                private void SetIdleState()
                {
                    SwitchState(_StateMachine._idleState);
                }

                public override void UpdateState() 
                {
                    HandleRotation();
                    HandleMoving();
                }

                public override void FixedUpdateState() { }

                private void HandleMoving()
                {
                    _StateMachine.LocalDirectionOfMovingChanged();
                    SetMotionParameters();
                    SetAccelerateNeed(out var needToAccelerate, out var targetVelocity);
                    SetPlanarVelocity(ref targetVelocity, ref needToAccelerate);


                    void SetMotionParameters()
                    {
                        switch (_StateMachine.CharacterActor.CurrentState)
                        {
                            case CharacterActorState.StableGrounded:
                                _CurrentMotionParameters = _StateMachine._stableGroundedParameters;
                                break;
                            case CharacterActorState.UnstableGrounded:
                                _CurrentMotionParameters = _StateMachine._unstableGroundedParameters;
                                break;
                            case CharacterActorState.NotGrounded:
                                _CurrentMotionParameters = _StateMachine._notGroundedParameters;
                                break;
                        }
                    }

                    void SetAccelerateNeed(out bool needToAccelerate, out Vector3 targetVelocity)
                    {
                        var characterActorState = _StateMachine.CharacterActor.CurrentState;
                        var globalDirection = new Vector3(_StateMachine._GlobalDirectionOfMoving.x,
                                                          .0f,
                                                          _StateMachine._GlobalDirectionOfMoving.y);
                        targetVelocity = SetLimitedTargetVelocity(globalDirection);
                        needToAccelerate = false;

                        switch (characterActorState)
                        {
                            case CharacterActorState.StableGrounded:
                                needToAccelerate = true;
                                break;
                            case CharacterActorState.UnstableGrounded:
                                needToAccelerate = targetVelocity.sqrMagnitude >= _StateMachine.CharacterActor.PlanarVelocity.sqrMagnitude;
                                break;
                            case CharacterActorState.NotGrounded:
                                needToAccelerate = targetVelocity.sqrMagnitude >= _StateMachine.CharacterActor.PlanarVelocity.sqrMagnitude;
                                break;
                        }
                    }

                    void SetPlanarVelocity(ref Vector3 targetVelocity, ref bool needToAccelerate)
                    {
                        var characterActor = _StateMachine.CharacterActor;
                        characterActor.PlanarVelocity = Vector3.MoveTowards(characterActor.PlanarVelocity,
                                                                            targetVelocity,
                                                                            (needToAccelerate ?
                                                                            _CurrentMotionParameters.Acceleration :
                                                                            _CurrentMotionParameters.Deceleration) *
                                                                            Time.deltaTime);
                    }
                }

                private void HandleRotation()
                {
                    _StateMachine.CharacterActor.Rotation =
                        Quaternion.Lerp(_StateMachine.CharacterActor.Rotation,
                        _StateMachine._NeededRotation,
                        Time.deltaTime * _StateMachine._RotationSpeed);
                }

                protected virtual Vector3 SetLimitedTargetVelocity(Vector3 vec)
                {
                    return vec.normalized;
                }
            }
        }
    }
}