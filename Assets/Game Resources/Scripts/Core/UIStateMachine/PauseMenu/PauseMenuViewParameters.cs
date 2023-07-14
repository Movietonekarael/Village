using UnityEngine;


namespace GameCore
{
    namespace GUI
    {
        [CreateAssetMenu(fileName = "View Data", menuName = "Game UI/View panels data/Pause menu", order = 3)]
        public sealed class PauseMenuViewParameters : ScriptableObject, IUIParameters
        {
            public GameObject CanvasPrefab;
            public GameObject PauseMenuPrefab;
        }
    }
}