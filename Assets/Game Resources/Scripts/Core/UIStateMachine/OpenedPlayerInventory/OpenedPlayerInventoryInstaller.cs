using GameCore.GUI;
using Zenject;


namespace GameCore
{
    namespace Installers
    {
        public class OpenedPlayerInventoryInstaller : MonoInstaller
        {
            public override void InstallBindings()
            {
                BindView();
                BindController();
            }

            private void BindView()
            {
                UIElementInstaller.BindUiElement<OpenedPlayerInventoryView,
                                                 IOpenedPlayerInventoryView,
                                                 IUIView<OpenedPlayerInventoryViewParameters, IOpenedPlayerInventoryController>>(Container);
            }

            private void BindController()
            {
                UIElementInstaller.BindUiElement<OpenedPlayerInventoryController,
                                                 IOpenedPlayerInventoryController,
                                                 IUIController<OpenedPlayerInventoryViewParameters>>(Container);
            }
        }
    }
}