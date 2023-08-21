using GameCore.Network;


namespace GameCore
{
    namespace GUI
    {
        public sealed class SetConnectionTypeButtonAction : MenuButtonAction
        {
            private readonly ConnectionType _connectionType;

            public SetConnectionTypeButtonAction(ConnectionType connectionType, MenuButtonAction nextAction = null) : base(nextAction)
            {
                _connectionType = connectionType;
            }

            protected override void Execute()
            {
                SetConnectionType();
            }

            private void SetConnectionType()
            {
                NetworkConnectionService.ConnectionType = _connectionType;
            }
        }
    }
}