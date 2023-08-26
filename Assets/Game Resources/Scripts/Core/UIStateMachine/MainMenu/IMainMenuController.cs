using System;

namespace GameCore
{
    namespace GUI
    {
        public interface IMainMenuController : ISpecificController
        {
            public event Action OnStartMultiplayer;
            public void SetStartupAnimationAvailability(bool allowed);
        }
    }
}