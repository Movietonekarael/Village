using System;
using UnityEngine;
using GameCore.GameControls;
using Lightbug.CharacterControllerPro.Core;
using GameCore.Animation;

namespace GameCore
{
    namespace GameMovement
    {
        public abstract partial class NPCMovementStateMachine : MonoBehaviour, ISerializedInterfaceBehaviour
        {
            protected IMovement _Movement;

            [Header("Character Actor: ")]
            public CharacterActor CharacterActor = null;

            [Header("Stable Grounded parameters: ")]
            [SerializeField] private MotionParameters _stableGroundedParameters = new(50f, 40f);

            [Header("Unstable Grounded parameters: ")]
            [SerializeField] private MotionParameters _unstableGroundedParameters = new(10f, 2f);

            [Header("Not Grounded parameters: ")]
            [SerializeField] private MotionParameters _notGroundedParameters = new(20f, 5f);

            [Header("Velocity: ")]
            [SerializeField] private float _walkVelocityLimit = 2f;
            [SerializeField] private float _slowRunVelocityLimit = 5f;
            [SerializeField] private float _fastRunVelocityLimit = 6f;

            [Header("Rotation: ")]
            [Tooltip("Rotation has about logarithmic dependence. The higher speed parameter, the higher dependency graph. " +
                     "Actual formula is dC = (N - C(t)) * dt * speed. Where C - current rotation, N - needed rotation.")]
            [SerializeField] protected float _RotationSpeed = 13.5f;
            protected Quaternion _NeededRotation;

            [Header("Jump Velocity Impulse: ")]
            [SerializeField] private float _jumpVelocityImpulse = 8f;



            private NPCMovementBaseState _currentState;

            private NPCMovementIdleState _idleState;
            private NPCMovementWalkState _walkState;
            private NPCMovementRunState _runState;
            private NPCMovementGroundedState _groundedState;
            private NPCMovementJumpState _jumpState;

            [Header("Other: ")]
            [SerializeField]
            [RequireInterface(typeof(IEventedAnimator))]
            private UnityEngine.Object _animatorControllerBase;

            private IEventedAnimator _animatorController;
            private bool _isRunning = false;
            private bool _isJumping = false;


            private readonly int _isWalkingBoolHash = Animator.StringToHash("isWalking");
            private readonly int _isRunningBoolHash = Animator.StringToHash("isRunning");
            private readonly int _dashTriggerHash = Animator.StringToHash("Dash");
            private readonly int _isJumpingBoolHash = Animator.StringToHash("isJumping");
            private readonly int _isJumpingEndBoolHash = Animator.StringToHash("isJumpingEnd");
            private readonly int _interruptJumpTriggerHash = Animator.StringToHash("InterruptJump");


            public event Action OnJumpEnded;
            public event Action OnJumpStartEnded;


            protected Vector2 _GlobalDirectionOfMoving;
            private Vector2 _privateLocalDirectionOfMoving;


            protected Vector2 _LocalDirectionOfMoving
            {
                get
                {
                    return _privateLocalDirectionOfMoving;
                }
                set
                {
                    if (value != Vector2.zero)
                    {
                        _privateLocalDirectionOfMoving = value;
                        LocalDirectionOfMovingChanged();
                    }
                }
            }


            protected virtual void Awake()
            {
                SetupSerializedInterfaces();
                SetUpJumpListener();
                SetRotationZero();
            }

            public void SetupSerializedInterfaces()
            {
                _animatorController = _animatorControllerBase as IEventedAnimator;
            }

            private void SetUpJumpListener()
            {
                var animatorObject = (_animatorController as Component).gameObject;
                if (!animatorObject.TryGetComponent<ListenJumpEvent>(out var listener))
                    listener = animatorObject.AddComponent<ListenJumpEvent>();
                listener.NPC = this;
            }

            private void SetRotationZero()
            {
                _NeededRotation = new()
                {
                    eulerAngles = Vector3.forward
                };
            }

            private void Start()
            {
                SetupCharacterActor();
                InitializeStates();
                EnterCurrentState();
            }

            public void SetupCharacterActor()
            {
                if (CharacterActor != null)
                {
                    CharacterActor.UseRootMotion = false;
                    CharacterActor.UpdateRootPosition = false;
                    CharacterActor.UpdateRootRotation = false;

                    CharacterActor.Velocity = Vector3.zero;
                }
            }

            private void InitializeStates()
            {
                _idleState = new(this);
                _walkState = new(this);
                _runState = new(this);
                _groundedState = new(this);
                _jumpState = new(this);

                _currentState = _groundedState;
            }

            public void EnterCurrentState()
            {
                if (CharacterActor != null)
                    _currentState.EnterState();
            }

            private void Update()
            {
                _currentState.UpdateStates();
            }

            private void FixedUpdate()
            {
                _currentState.FixedUpdateStates();
            }

            public void StartJumpingEnd()
            {
                OnJumpStartEnded?.Invoke();
            }

            public void EndJump()
            {
                OnJumpEnded?.Invoke();
            }

            protected virtual void LocalDirectionOfMovingChanged()
            {
                if (_LocalDirectionOfMoving != Vector2.zero)
                    _GlobalDirectionOfMoving = _LocalDirectionOfMoving;
            }
        }
    }
}