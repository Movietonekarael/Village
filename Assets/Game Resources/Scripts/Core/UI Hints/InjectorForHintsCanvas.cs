using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace GameCore.Injectors
{
    [RequireComponent(typeof(Canvas))]
    public class InjectorForHintsCanvas : MonoBehaviour
    {
        [Inject(Id = "UiCamera")] private Camera _uiCamera;

        private void Awake()
        {
            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = _uiCamera;
        }
    }
}
