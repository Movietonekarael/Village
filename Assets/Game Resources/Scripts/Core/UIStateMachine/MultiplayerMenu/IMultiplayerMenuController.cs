using System;

namespace GameCore
{
    namespace GUI
    {
        public interface IMultiplayerMenuController : ISpecificController
        {
            public event Action OnBackToMainMenu;

            public void BackToMainMenu();
        }
    }
}