using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.GameControls;
using UnityEngine.UI;
using Lightbug.CharacterControllerPro.Core;


namespace GameCore.GameMovement
{
    /// <summary>
    /// Basic class for moving npc
    /// </summary>
    //[RequireComponent(typeof(Rigidbody))]
    public abstract partial class NPCMovement : MonoBehaviour
    {
        public struct MotionParameters
        {
            public float acceleration;
            public float deceleration;
        }

       
        [Header("Character Actor: ")]
        [SerializeField] protected CharacterActor _characterActor = null;

        [Header("Stable Grounded parameters: ")]
        [SerializeField] private float _stableGroundedAcceleration = 50f;
        [SerializeField] private float _stableGroundedDeceleration = 40f;

        [Header("Unstable Grounded parameters: ")]
        [SerializeField] private float _unstableGroundedAcceleration = 10f;
        [SerializeField] private float _unstableGroundedDeceleration = 2f;

        [Header("Not Grounded parameters: ")]
        [SerializeField] private float _notGroundedAcceleration = 20f;
        [SerializeField] private float _notGroundedDeceleration = 5f;

        [Header("Speed: ")]
        [SerializeField] private float _walkSpeedLimit = 2f;
        [SerializeField] private float _slowRunSpeedLimit = 5f;
        [SerializeField] private float _fastRunSpeedLimit = 6f;

        [Header("Rotation: ")]
        [SerializeField] protected float rotationSpeed = 13.5f;
        protected Quaternion neededRotation;

        [Header("Jump Speed: ")]
        [SerializeField] private float _jumpSpeed = 5f;



        private NPCMovementBaseState _currentState;

        private NPCMovementIdleState _idleState;
        private NPCMovementWalkState _walkState;
        private NPCMovementRunState _runState;
        private NPCMovementGroundedState _groundedState;
        private NPCMovementJumpState _jumpState;

        [Header("Other: ")]
        [SerializeField] private Animator _animatorController;
        private bool _isRunning = false;
        private bool _isJumping = false;


        //IDs of animator parameters
        private readonly int _isWalkingBoolHash = Animator.StringToHash("isWalking");
        private readonly int _isRunningBoolHash = Animator.StringToHash("isRunning");
        private readonly int _dashTriggerHash = Animator.StringToHash("Dash");
        private readonly int _isJumpingBoolHash = Animator.StringToHash("isJumping");
        private readonly int _isJumpingEndBoolHash = Animator.StringToHash("isJumpingEnd");
        private readonly int _interruptJumpTriggerHash = Animator.StringToHash("InterruptJump");


        public VoidHandler OnMovementStart;
        public VoidHandler OnMovementFinish;
        public Vector2Handler OnMovement;
        public VoidHandler OnRunningStateChanged;
        public VoidHandler OnDashed;
        public VoidHandler OnJump;

        private event VoidHandler OnMovementStartEvent;
        private event VoidHandler OnMovementFinishEvent;
        private event Vector2Handler OnMovementEvent;
        private event VoidHandler OnRunningStateChangedEvent;
        private event VoidHandler OnDashedEvent;
        private event VoidHandler OnJumpEvent;
        private event VoidHandler OnJumpEndedEvent;
        private event VoidHandler OnJumpStartEndedEvent;


        protected Vector2 _globalDirectionOfMoving;
        private Vector2 _privateLocalDirectionOfMoving;


        /// <summary>
        /// Local direction of moving
        /// </summary>
        protected Vector2 _localDirectionOfMoving
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
            RegisterHandlers();
            SetRotationZero();
        }


        private void Start()
        {
            if (_characterActor is not null)
            {
                _characterActor.UseRootMotion = false;
                _characterActor.UpdateRootPosition = false;
                _characterActor.UpdateRootRotation = false;

                _characterActor.Velocity = Vector3.zero;
            }


            _idleState = new(this);
            _walkState = new(this);
            _runState = new(this);
            _groundedState = new(this);
            _jumpState = new(this);


            _currentState = _groundedState;
            _currentState.EnterState();
        }

        protected virtual void Update()
        {
            _currentState.UpdateStates();
        }



        private void FixedUpdate()
        {
            _currentState.FixedUpdateStates();
        }

        protected virtual void OnEnable()
        {
            RegisterHandlers();
        }

        protected virtual void OnDisable()
        {
            UnRegisterHandlers();
        }

        private void RegisterHandlers()
        {
            OnMovementStart += SetMovementStartHandler;
            OnMovementFinish += SetMovementFinishHandler;
            OnMovement += SetMovementHandler;
            OnRunningStateChanged += RunningStateChangedHandler;
            OnDashed += DashedHandler;
            OnJump += JumpHandler;
        }

        private void UnRegisterHandlers()
        {
            OnMovementStart -= SetMovementStartHandler;
            OnMovementFinish -= SetMovementFinishHandler;
            OnMovement -= SetMovementHandler;
            OnRunningStateChanged -= RunningStateChangedHandler;
            OnDashed -= DashedHandler;
            OnJump -= JumpHandler;
        }

        private void SetRotationZero()
        {
            neededRotation = new()
            {
                eulerAngles = Vector3.forward
            };
        }

        private void SetMovementStartHandler()
        {
            OnMovementStartEvent?.Invoke();
        }

        private void SetMovementFinishHandler()
        {
            OnMovementFinishEvent?.Invoke();
        }

        private void SetMovementHandler(Vector2 vec)
        {
            OnMovementEvent?.Invoke(vec);
        }

        private void RunningStateChangedHandler()
        {
            OnRunningStateChangedEvent?.Invoke();
        }

        private void DashedHandler()
        {
            OnDashedEvent?.Invoke();
        }

        private void JumpHandler()
        {
            OnJumpEvent?.Invoke();
        }

        public void StartJumpingEnd()
        {
            OnJumpStartEndedEvent?.Invoke();
        }

        public void EndJump()
        {
            OnJumpEndedEvent?.Invoke();
        }

        protected virtual void LocalDirectionOfMovingChanged()
        {
            if(_localDirectionOfMoving != Vector2.zero) 
                _globalDirectionOfMoving = _localDirectionOfMoving;
        }

    }
}

