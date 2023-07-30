using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace GameCore
{
    namespace GUI
    {
        [RequireComponent(typeof(UIStateMachine))]
        public abstract partial class BaseUIState<T, I, P> : MonoBehaviour, IUIState
                                                          where T : IUIParameters, new()
                                                          where I : P, IUIController<T>, IDeinitializable, IActivatable, new()
                                                          where P : ISpecificController
        {
            private UIStateMachine _stateMachine;
            protected UIStateMachine _StateMachine { get { return _stateMachine; } }

            [SerializeField] private T _parameters;

            [SerializeField] private List<UIStateWrap> _substates;

            private IUIController<T> _uiController;
            protected P _Controller;
            private IDeinitializable _controllerDeinitializator;
            private IActivatable _controllerActivator;

            private void Awake()
            {
                _stateMachine = GetComponent<UIStateMachine>();
                InitializeController();
            }

            private void OnDestroy()
            {
                DeinitializeController();
            }

            protected abstract void StartState(params bool[] args);
            protected abstract void EndState();

            private void InitializeController()
            {
                InitializeParametersIfNull();
                InstantiateController();
                _uiController.Init(_parameters);
            }

            private void InitializeParametersIfNull()
            {
                _parameters ??= new();
            }

            private void InstantiateController()
            {
                Debug.Log("Creating controller.");
                var controller = DiContainerReference.Container.Instantiate<I>();
                _Controller = controller;
                _uiController = controller;
                _controllerDeinitializator = controller;
                _controllerActivator = controller;
            }

            private void DeinitializeController()
            {
                _controllerDeinitializator?.Deinitialize();
            }

            public void EnterState(params bool[] args)
            {
                StartState(args);
                _controllerActivator?.Activate();
                EnterSubstates();
            }

            public void ExitState()
            {
                EndState();
                _controllerActivator?.Deactivate();
                ExitSubstates();
            }

            private void EnterSubstates()
            {
                foreach (var substate in _substates)
                {
                    substate.Value.EnterState(substate.Arguments);
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
}