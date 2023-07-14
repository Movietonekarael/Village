using GameCore.GUI;
using Zenject;


namespace GameCore
{
    namespace Installers
    {
        public class MainScreenInstaller : MonoInstaller
        {
            public override void InstallBindings()
            {
                BindView();
                BindController();
            }

            private void BindView()
            {
                UIElementInstaller.BindUiElement<MainScreenView,
                                                 IMainScreenView,
                                                 IUIView<MainScreenViewParameters, IMainScreenController>>(Container);
            }

            private void BindController()
            {
                UIElementInstaller.BindUiElement<MainScreenController,
                                                 IMainScreenController,
                                                 IUIController<MainScreenViewParameters>>(Container);
            }
        }
    }
}