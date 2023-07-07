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
    public abstract partial class BaseUIState<T, I> : MonoBehaviour, IUIState 
                                                      where T : IUIParameters, new()
                                                      where I : ISpecificController
    {
        private UIStateMachine _stateMachine;
        protected UIStateMachine _StateMachine { get { return _stateMachine; } }

        [SerializeField] private T _parameters;

        [SerializeField] private List<UIStateWrap> _substates;

        [Inject] private readonly IUIController<T> _uiController;
        [Inject] protected I _Controller;
        [Inject] private readonly IDeinitializable<I> _controllerDeinitializator;
        [Inject] private readonly IActivatable<I> _controllerActivator;

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
            InitializeParametersIfNull();
            _uiController.Init(_parameters);
        }

        private void InitializeParametersIfNull()
        {
            _parameters ??= new();
        }

        private void DeinitializeController()
        {
            _controllerDeinitializator?.Deinitialize();
        }        

        public void EnterState()
        {
            _controllerActivator?.Activate();
            StartState();
            EnterSubstates();
        }

        public void ExitState()
        {
            _controllerActivator?.Deactivate();
            EndState();
            ExitSubstates();
        }

        private void EnterSubstates()
        {
            foreach (var substate in _substates) 
            {
                substate.Value.EnterState();
            }
        }

        private void ExitSubstates()
        {
            foreach (var substate in _substates)
            {
                substate.Value.ExitState();
            }
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

