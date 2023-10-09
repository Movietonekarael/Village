using UnityEngine;


namespace GameCore
{
    namespace GUI
    {
        [CreateAssetMenu(fileName = "View Data", menuName = "Game UI/View panels data/VirtualPointer", order = -1)]
        public sealed class CursorUnlockViewParameters : ScriptableObject, IUIParameters
        {
            public GameObject CanvasPrefab;
            public GameObject VirtualPointerPrefab;
        }
    }
}