using System;

namespace GameCore
{
    namespace GUI
    {
        public interface IMultiplayerMenu
        {
            public event Action OnHostButtonPressed;
            public event Action OnConnectButtonPressed;
            public event Action OnBackButtonPressed;

            public void StartMultiplayerMenu();
        }
    }
}