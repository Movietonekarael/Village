using System;


namespace GameCore
{
    namespace GUI
    {
        namespace Menus
        {
            public interface IMultiplayerMenu
            {
                public event Action OnHostButtonPressed;
                public event Action OnConnectButtonPressed;
                public event Action OnBackButtonPressed;

                public void StartMultiplayerMenu();
                public void EnableAllButtons();
                public void DisableAllButtons();
            }
        }
    }
}