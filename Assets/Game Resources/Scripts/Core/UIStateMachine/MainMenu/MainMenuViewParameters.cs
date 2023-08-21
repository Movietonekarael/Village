using UnityEngine;

namespace GameCore
{
    namespace GUI
    {
        [CreateAssetMenu(fileName = "View Data", menuName = "Game UI/Menu panels data/Main Menu", order = 0)]
        public sealed class MainMenuViewParameters : ScriptableObject, IUIParameters
        {
            public GameObject CanvasPrefab;

            public MenuButtonActionCreator SingleCreator;
            public MenuButtonActionCreator[] Creator;
        }
    }
}