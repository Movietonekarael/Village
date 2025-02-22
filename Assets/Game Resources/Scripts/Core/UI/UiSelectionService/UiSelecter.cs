﻿using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GameCore
{
    namespace GUI
    {
        public class UiSelecter : MonoBehaviour, ISelectable
        {
            [Inject] private readonly EventSystem _eventSystem;


            public void Select()
            {
                if (this != null && this.gameObject != null)
                    _eventSystem.SetSelectedGameObject(this.gameObject);
            }

            public void Deselect()
            {
                _eventSystem.SetSelectedGameObject(null);
            }
        }
    }
}
