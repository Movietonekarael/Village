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
        [SerializeField] private BaseUIState _firstState;
        
        public BaseUIState CurrentState { private get; set; }


        private void Start()
        {
            CurrentState = _firstState;
            CurrentState.EnterState();
        }
    }

}

