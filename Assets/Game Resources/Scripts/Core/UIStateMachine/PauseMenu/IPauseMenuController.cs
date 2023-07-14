using System;


namespace GameCore
{
    namespace GUI
    {
        public interface IPauseMenuController : ISpecificController
        {
            public event Action OnContinueGame;
            public void ContinueGame();
            public void QuitGame();
        }
    }
}