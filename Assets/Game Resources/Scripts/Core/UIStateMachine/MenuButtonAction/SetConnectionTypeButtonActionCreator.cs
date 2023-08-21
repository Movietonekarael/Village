using GameCore.Network;
using System;


namespace GameCore
{
    namespace GUI
    {
        [Serializable]
        public sealed class SetConnectionTypeButtonActionCreator : MenuButtonActionCreator
        {
            public ConnectionType ConnectionType;

            public override MenuButtonAction Create(MenuButtonAction nextAction = null)
            {
                return new SetConnectionTypeButtonAction(ConnectionType, nextAction);
            }
        }
    }
}