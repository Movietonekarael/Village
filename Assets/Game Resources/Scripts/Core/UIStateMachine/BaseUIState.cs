using GameCore.GameControls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System;
using GameCore.Inventory;

namespace GameCore.GUI
{
    [RequireComponent(typeof(UIStateMachine))]
    public abstract class BaseUIState<T, I> : MonoBehaviour, IUIState 
                                              where T : IUIParameters
                                              where I : ISpecificController
    {
        [Inject] protected InputHandler _InputHandler;
        private UIStateMachine _stateMachine;
        protected UIStateMachine _StateMachine { get { return _stateMachine; } }

        [SerializeField] private T _parameters;

        [Inject] private IUIController<T> _uiController;
        [Inject] private IDeinitializable<I> _controllerDeinitializator;
        [Inject] private IActivatable<I> _controllerActivator;

        private void Awake()
        {
            _stateMachine = GetComponent<UIStateMachine>();
            InitializeController();
        }

        private void OnDestroy()
        {
            DeinitializeController();
        }

        protected abstract void StartState();
        protected abstract void EndState();

        private void InitializeController()
        {
            _uiController.Init(_parameters);
        }

        private void DeinitializeController()
        {
            _controllerDeinitializator?.Deinitialize();
        }        

        public void EnterState()
        {
            _controllerActivator?.Activate();
            StartState();
        }

        protected void ExitState()
        {
            _controllerActivator?.Deactivate();
            EndState();
        }

        protected void SwitchState(IUIState newState)
        {
            if (newState != null)
            {
                ExitState();

                newState.EnterState();

                _stateMachine.CurrentState = newState;
            }
        }
    }
}

