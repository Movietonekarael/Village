using UnityEngine;

namespace GameCore.GUI
{
    public interface IPauseMenuView : ISpecificView
    {
        public void SetLastSubmitButton();
        public void RememberSubmitButton();
    }
}

