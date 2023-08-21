using System;


namespace GameCore
{
    namespace GUI
    {
        [Serializable]
        public sealed class ButtonPressedActionCreator : MenuButtonActionCreator
        {
            public override MenuButtonAction Create(MenuButtonAction nextAction = null)
            {
                return new ButtonPressedAction(nextAction);
            }
        }
    }
}