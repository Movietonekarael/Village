using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.GameControls;
using GameCore.Inventory;
using UnityEngine.EventSystems;

namespace GameCore.GUI
{
    public sealed class UIStateMachine : MonoBehaviour
    {
        [SerializeField] private BaseUIState _firstState;

        [SerializeField] private GameObject _eventSystemObject;
        [HideInInspector] public EventSystem EventSystem;

        [SerializeField] private GameObject _uiCameraObject;
        [HideInInspector] public Camera UiCamera;
        
        public BaseUIState CurrentState { private get; set; }

        private void OnEnable()
        {
            SetupUiCamera();
        }

        private void SetupUiCamera()
        {
            UiCamera = _uiCameraObject.GetComponent<Camera>();
        }

        private void Awake()
        {
            EventSystem = _eventSystemObject.GetComponent<EventSystem>();
        }

        private void Start()
        {
            CurrentState = _firstState;
            CurrentState.EnterState();
        }
    }

}

