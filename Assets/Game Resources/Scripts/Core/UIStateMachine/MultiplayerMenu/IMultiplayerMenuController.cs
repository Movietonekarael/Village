using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace GameCore
{
    namespace GUI
    {
        public interface IMultiplayerMenuController : ISpecificController
        {
            public event Action OnBackToMainMenu;

            public void BackToMainMenu();
            public void StartHostServer();
            public void ConnectToServer(string joinCode);
            public void SetMultiplayerPlayerSceneReference(AssetReference _sceneReference);
        }
    }
}