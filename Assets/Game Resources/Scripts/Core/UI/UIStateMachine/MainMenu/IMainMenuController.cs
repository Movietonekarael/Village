using System;
using UnityEngine.AddressableAssets;

namespace GameCore
{
    namespace GUI
    {
        public interface IMainMenuController : ISpecificController
        {
            public event Action OnStartMultiplayer;
            
            public void SetStartupAnimationAvailability(bool allowed);

            public void SetSinglePlayerSceneReference(AssetReference sceneReference);
            public void StartSinglePlayer();
            public void StartMultiPlayer();
            public void QuitApplication();
        }
    }
}