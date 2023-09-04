using System;

namespace GameCore
{
    namespace GUI
    {
        public sealed class ConnectionCodeWindow
        {

        }


        public sealed class MultiplayerMenu : Menu<MultiplayerMenuButtonType>, IMultiplayerMenu
        {
            public event Action OnHostButtonPressed;
            public event Action OnConnectButtonPressed;
            public event Action OnBackButtonPressed;


            protected override void OnAwake() { }
            protected override void OnCacheAnimators() { }
            protected override void AdditionalOnDestroy() { }


            protected override void ButtonPressed(uint index)
            {
                var type = (MultiplayerMenuButtonType)index;
                switch(type)
                {
                    case MultiplayerMenuButtonType.HostServer:
                        OnHostButtonPressed?.Invoke();
                        break;
                    case MultiplayerMenuButtonType.ConnectToServer:
                        OnConnectButtonPressed?.Invoke();
                        break;
                    case MultiplayerMenuButtonType.Back:
                        OnBackButtonPressed?.Invoke();
                        break;
                }
            }

            public void StartMultiplayerMenu()
            {
                StartButtonsAnimation();
            }

            private void StartButtonsAnimation()
            {
                foreach (var animator in _ButtonAnimators)
                {
                    animator.Animate(false);
                }
            }
        }
    }
}