using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameCore
{
    namespace GUI
    {
        [CreateAssetMenu(fileName = "View Data", menuName = "Game UI/Menu panels data/Main Menu", order = 0)]
        public sealed class MainMenuViewParameters : ScriptableObject, IUIParameters
        {
            public AssetReferenceGameObject CanvasReference;
            public AssetReferenceGameObject MainMenuReference;
        }
    }
}