using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace GameCore.Installers
{
    public class HintsCanvasInstaller : MonoInstaller
    {
        [SerializeField] private RectTransform _canvasTransform;

        public override void InstallBindings()
        {
            BindHintCanvasTransform();
        }

        private void BindHintCanvasTransform()
        {
            Container.Bind<RectTransform>().WithId("HintsCanvas").FromInstance(_canvasTransform).NonLazy();
        }
    }
}

