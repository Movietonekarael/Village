using System;


namespace GameCore
{
    namespace GUI
    {
        namespace Menus
        {
            public interface IMainMenu
            {
                public event Action OnSinglePlayerButtonPressed;
                public event Action OnMultiplayerButtonPressed;
                public event Action OnQuitApplicationPressed;

                public void StartMainMenu();
                public void SetAnimated(bool isAnimated);
            }
        }
    }
}