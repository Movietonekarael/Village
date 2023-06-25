using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.GameControls;
using UnityEngine.UI;
using Lightbug.CharacterControllerPro.Core;
using System.Threading.Tasks;

namespace GameCore.GameMovement
{
    public abstract partial class NPCMovementStateMachine : MonoBehaviour
    {
        protected IMovement _Movement;
       
        [Header("Character Actor: ")]
        [SerializeField] protected CharacterActor _characterActor = null;

        [Header("Stable Grounded parameters: ")]
        [SerializeField] private MotionParameters _stableGroundedParameters = new (50f, 40f);

        [Header("Unstable Grounded parameters: ")]
        [SerializeField] private MotionParameters _unstableGroundedParameters = new(10f, 2f);

        [Header("Not Grounded parameters: ")]
        [SerializeField] private MotionParameters _notGroundedParameters = new(20f, 5f);

        [Header("Speed: ")]
        [SerializeField] private float _walkSpeedLimit = 2f;
        [SerializeField] private float _slowRunSpeedLimit = 5f;
        [SerializeField] private float _fastRunSpeedLimit = 6f;

        [Header("Rotation: ")]
        [SerializeField] protected float _RotationSpeed = 13.5f;
        protected Quaternion _NeededRotation;

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

        private void Awake()
        {
            SetUpJumpListener();
            SetRotationZero();
        }

        private void SetUpJumpListener()
        {
            var animatorObject = _animatorController.gameObject;
            var listner = animatorObject.AddComponent<ListenJumpEvent>();
            listner.NPC = this;
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
            if(_localDirectionOfMoving != Vector2.zero) 
                _GlobalDirectionOfMoving = _localDirectionOfMoving;
        }

    }
}

