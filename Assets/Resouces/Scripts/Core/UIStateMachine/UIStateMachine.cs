using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.GameControls;
using GameCore.Inventory;

namespace GameCore.GUI
{
    public partial class UIStateMachine : MonoBehaviour
    {
        private InputHandler _inputHandler;

        [SerializeField] private FastInventoryPanelUI _fastInventoryPanelUI;
        [SerializeField] private PlayerInventoryPanelUI _playerInventoryPanelUI;


        private BaseUIState _currentState;

        private ClosedInventoryUIState _closedInventoryUIState;
        private OpenedPlayerInventoryUIState _openedPlayerInventoryUIState;



        private event Action OnOpenCloseInventoryEvent;
        private event Action<int> OnSwitchSelectedItemEvent;

        private void OnEnable()
        {
            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            _inputHandler.OnOpenCloseInventory += OnOpenCloseInventory;
            _inputHandler.OnInventoryKeyPressed += OnSwitchSelectedItem;
        }

        private void OnDisable()
        {
            UnRegisterHandlers();
        }

        private void UnRegisterHandlers()
        {
            _inputHandler.OnOpenCloseInventory -= OnOpenCloseInventory;
            _inputHandler.OnInventoryKeyPressed -= OnSwitchSelectedItem;
        }

        private void OnOpenCloseInventory()
        {
            OnOpenCloseInventoryEvent?.Invoke();
        }

        private void OnSwitchSelectedItem(int value)
        {
            OnSwitchSelectedItemEvent?.Invoke(value);
        }

        private void Awake()
        {
            _inputHandler = InputHandler.GetInstance("PlayerInventory");

            _closedInventoryUIState = new(this, _fastInventoryPanelUI);
            _openedPlayerInventoryUIState = new(this, _playerInventoryPanelUI);
        }

        private void Start()
        {
            _currentState = _closedInventoryUIState;
            _currentState.EnterState();
        }
    }

}

