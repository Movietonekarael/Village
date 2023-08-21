using Unity.Collections;
using System.Collections;

namespace GameCore
{
    namespace GUI
    {
        public sealed class MainMenuUIState : BaseUIState<MainMenuViewParameters, MainMenuController, IMainMenuController>
        {
            protected override void EndState()
            {
                throw new System.NotImplementedException();
            }

            protected override void StartState(params bool[] args)
            {
                throw new System.NotImplementedException();
            }
        }

        public interface IMainMenuController : ISpecificController
        {

        }

        public interface IMainMenuView : ISpecificView
        {

        }

        public sealed class MainMenuView : UIView<MainMenuViewParameters, IMainMenuController, IMainMenuView>, IMainMenuView
        {
            public override void Activate()
            {
                throw new System.NotImplementedException();
            }

            public override void Deactivate()
            {
                throw new System.NotImplementedException();
            }

            public override void Deinitialize()
            {
                throw new System.NotImplementedException();
            }

            protected override void InstantiateViewElements()
            {
                throw new System.NotImplementedException();
            }
        }

        public sealed class MainMenuController : UIController<MainMenuViewParameters,
                                                              IMainMenuController,
                                                              MainMenuView,
                                                              IMainMenuView>,
                                                 IMainMenuController
        {
            protected override void InitializeParameters(MainMenuViewParameters parameters)
            {
                throw new System.NotImplementedException();
            }

            protected override void OnActivate()
            {
                throw new System.NotImplementedException();
            }

            protected override void OnDeactivate()
            {
                throw new System.NotImplementedException();
            }

            protected override void SubscribeForPermanentEvents()
            {
                throw new System.NotImplementedException();
            }

            protected override void SubscribeForTemporaryEvents()
            {
                throw new System.NotImplementedException();
            }

            protected override void UnsubscribeForPermanentEvents()
            {
                throw new System.NotImplementedException();
            }

            protected override void UnsubscribeForTemporaryEvents()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}