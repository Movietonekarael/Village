using System;


namespace GameCore
{
    namespace GUI
    {
        [Serializable]
        public sealed class QuitApplicationButtonActionCreator : MenuButtonActionCreator
        {
            public override MenuButtonAction Create(MenuButtonAction nextAction = null)
            {
                return new QuitApplicationButtonAction(nextAction);
            }
        }
    }
}