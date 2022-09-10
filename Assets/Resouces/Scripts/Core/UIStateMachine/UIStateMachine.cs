using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.GameControls;
using GameCore.Inventory;

namespace GameCore.GUI
{
    public partial class UIStateMachine : MonoBehaviour
    {
        private PlayerController _playerController;

        [SerializeField] private FastInventoryPanelUI _fastInventoryPanelUI;
        [SerializeField] private PlayerInventoryPanelUI _playerInventoryPanelUI;


        private BaseUIState _currentState;

        private ClosedInventoryUIState _closedInventoryUIState;
        private OpenedPlayerInventoryUIState _openedPlayerInventoryUIState;



        private event VoidHandler OnOpenCloseInventoryEvent;
        private event IntHandler OnSwitchSelectedItemEvent;

        private void OnEnable()
        {
            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            _playerController.OnOpenCloseInventory += OnOpenCloseInventory;
            _playerController.OnInventoryKeyPressed += OnSwitchSelectedItem;
        }

        private void OnDisable()
        {
            UnRegisterHandlers();
        }

        private void UnRegisterHandlers()
        {
            _playerController.OnOpenCloseInventory -= OnOpenCloseInventory;
            _playerController.OnInventoryKeyPressed -= OnSwitchSelectedItem;
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
            _playerController = PlayerController.instance;
            if (_playerController is null)
                Debug.LogWarning("There is not PlayerController in the scene to attach to PlayerInventory script.");

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

