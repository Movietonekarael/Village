using System;

namespace GameCore
{
    namespace GUI
    {
        public sealed class MainMenuController : UIController<MainMenuViewParameters,
                                                              IMainMenuController,
                                                              MainMenuView,
                                                              IMainMenuView>,
                                                 IMainMenuController
        {
            public event Action OnStartMultiplayer;

            public void SetStartupAnimationAvailability(bool allowed)
            {
                _View.SetStartupAnimationAvailability(allowed);
            }

            protected override void InitializeParameters(MainMenuViewParameters parameters)
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