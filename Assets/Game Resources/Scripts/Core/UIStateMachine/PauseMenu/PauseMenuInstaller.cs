using GameCore.GUI;
using Zenject;


namespace GameCore.Installers
{
    public class PauseMenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindView();
            BindController();
        }

        private void BindView()
        {
            UIElementInstaller.BindUiElement<PauseMenuView,
                                             IPauseMenuView,
                                             IUIView<PauseMenuViewParameters, IPauseMenuController>>(Container);
        }

        private void BindController()
        {
            UIElementInstaller.BindUiElement<PauseMenuController,
                                             IPauseMenuController,
                                             IUIController<PauseMenuViewParameters>>(Container);
        }
    }
}

