using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCore.GameControls;
using GameCore.Inventory;
using UnityEngine.EventSystems;
using Zenject;

namespace GameCore.GUI
{
    public sealed class UIStateMachine : MonoBehaviour
    {
        [SerializeField]
        [RequireInterface(typeof(IUIState))]
        private UnityEngine.Object _firstStateBase;

        private IUIState _firstState { get => _firstStateBase as IUIState; }
        
        public IUIState CurrentState { private get; set; }


        private void Start()
        {
            if (_firstState is not null)
            {
                CurrentState = _firstState;
                CurrentState.EnterState();
            }
        }
    }

}

