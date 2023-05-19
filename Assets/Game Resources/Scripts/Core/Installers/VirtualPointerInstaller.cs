using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace GameCore.Installers
{
    public class VirtualPointerInstaller : MonoInstaller
    {
        [SerializeField] private RectTransform _pointerTransform;

        public override void InstallBindings()
        {
            BindVirtualPointTransform();
        }

        private void BindVirtualPointTransform()
        {
            Container.Bind<RectTransform>().WithId("VirtualPointer").FromInstance(_pointerTransform).NonLazy();
        }
    }
}