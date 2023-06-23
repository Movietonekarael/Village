using GameCore.GUI;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Zenject;


namespace GameCore.Installers
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

