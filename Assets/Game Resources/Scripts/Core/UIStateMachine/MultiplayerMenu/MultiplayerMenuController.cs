using System;

namespace GameCore
{
    namespace GUI
    {
        public sealed class MultiplayerMenuController : UIController<MultiplayerMenuViewParameters,
                                                                     IMultiplayerMenuController,
                                                                     MultiplayerMenuView,
                                                                     IMultiplayerMenuView>,
                                                        IMultiplayerMenuController
        {
            public event Action OnBackToMainMenu;

            public void BackToMainMenu()
            {
                OnBackToMainMenu?.Invoke();
            }

            protected override void InitializeParameters(MultiplayerMenuViewParameters parameters)
            {

            }

            protected override void OnActivate()
            {

            }

            protected override void OnDeactivate()
            {

            }

            protected override void SubscribeForPermanentEvents()
            {

            }

            protected override void SubscribeForTemporaryEvents()
            {

            }

            protected override void UnsubscribeForPermanentEvents()
            {

            }

            protected override void UnsubscribeForTemporaryEvents()
            {

            }
        }
    }
}