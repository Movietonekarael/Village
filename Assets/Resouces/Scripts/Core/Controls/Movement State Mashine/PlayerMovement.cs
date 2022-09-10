using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MovietoneMath;
using GameCore.GameControls;



namespace GameCore.GameMovement
{
    public class PlayerMovement : NPCMovement
    {
        private PlayerController _playerController;

        /// Needs for calculating viewing direction
        [SerializeField] new private Transform camera;
        [SerializeField] private Transform cameraTarget;


        protected override void OnEnable()
        {
            _playerController.OnMovementStart += OnMovementStart;
            _playerController.OnMovementFinish += OnMovementFinish;
            _playerController.OnMovement += OnMovement;
            _playerController.OnRunningChanged += OnRunningStateChanged;
            _playerController.OnDashed += OnDashed;
            _playerController.OnJumpPressed += OnJump;

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            _playerController.OnMovementStart -= OnMovementStart;
            _playerController.OnMovementFinish -= OnMovementFinish;
            _playerController.OnMovement -= OnMovement;
            _playerController.OnRunningChanged -= OnRunningStateChanged;
            _playerController.OnDashed -= OnDashed;
            _playerController.OnJumpPressed -= OnJump;

            base.OnDisable();
        }

        protected override void Awake()
        {
            base.Awake();

            _playerController = PlayerController.instance;
            if (_playerController is null)
                Debug.LogWarning("There is not PlayerController in the scene to attach to PlayerMovement script.");
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void LocalDirectionOfMovingChanged()
        {
            // Gets angle of local moving direction
            float hr1 = MathM.Vector.GetAngleOfVector2(_localDirectionOfMoving);

            Vector2 cameraDirection = new(cameraTarget.position.x - camera.position.x,
                                          cameraTarget.position.z - camera.position.z);
            // Gets angle of global camera direcction
            float hr2 = MathM.Vector.GetAngleOfVector2(cameraDirection);

            // Calculates and rewrites global moving direction
            float angleRotation = hr1 + hr2;
            _globalDirectionOfMoving = MathM.Vector.GetVector2OfAngle(angleRotation);

            // Sets needed rotation depends on global moving vector
            neededRotation.eulerAngles = new(.0f, angleRotation, .0f);
        }
    }
}