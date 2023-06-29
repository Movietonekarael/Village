using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.GUI
{
    [CreateAssetMenu(fileName = "View Data", menuName = "Game UI/View panels data/Pause menu", order = 3)]
    public sealed class PauseMenuViewParameters : ScriptableObject, IUIParameters
    {
        public GameObject CanvasPrefab;
        public GameObject PauseMenuPrefab;
    }
}