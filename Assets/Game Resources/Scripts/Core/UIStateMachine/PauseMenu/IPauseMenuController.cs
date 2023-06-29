using System;

namespace GameCore.GUI
{
    public interface IPauseMenuController : ISpecificController
    {
        public event Action OnContinueGame;
        public void ContinueGame();
        public void QuitGame();
    }
}

