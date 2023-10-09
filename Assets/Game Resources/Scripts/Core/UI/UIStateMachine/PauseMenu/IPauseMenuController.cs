using System;
using UnityEngine.AddressableAssets;

namespace GameCore
{
    namespace GUI
    {
        public interface IPauseMenuController : ISpecificController
        {
            public event Action OnContinueGame;
            public void SetMainMenuSceneReference(AssetReference mainMenuSceneReference);
            public void ContinueGame();
            public void QuitGame();
        }
    }
}