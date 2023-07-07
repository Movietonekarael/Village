using GameCore.GUI;
using Zenject;

namespace GameCore.Installers
{
    public class CursorUnlockInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindView();
            BindController();
        }

        private void BindView()
        {
            UIElementInstaller.BindUiElement<CursorUnlockView,
                                             ICursorUnlockView,
                                             IUIView<CursorUnlockViewParameters,
                                             ICursorUnlockController>>(Container);
        }

        private void BindController()
        {
            UIElementInstaller.BindUiElement<CursorUnlockController,
                                             ICursorUnlockController,
                                             IUIController<CursorUnlockViewParameters>>(Container);
        }
    }
}