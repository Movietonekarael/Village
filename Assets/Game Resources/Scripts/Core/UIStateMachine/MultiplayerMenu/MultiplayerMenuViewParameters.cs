using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameCore
{
    namespace GUI
    {
        [CreateAssetMenu(fileName = "View Data", menuName = "Game UI/Menu panels data/Multiplayer Menu", order = 1)]
        public sealed class MultiplayerMenuViewParameters : ScriptableObject, IUIParameters
        {
            public AssetReferenceGameObject CanvasReference;
            public AssetReferenceGameObject MultiplayerMenuReference;
        }
    }
}