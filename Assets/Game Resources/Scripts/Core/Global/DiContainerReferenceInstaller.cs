using UnityEngine;
using Zenject;

namespace GameCore
{
    namespace Installers
    {
        public class DiContainerReferenceInstaller : MonoInstaller
        {
            [Inject] private readonly DiContainer _container;

            public override void InstallBindings()
            {
                SetupDiContainerReference();
            }

            private void SetupDiContainerReference()
            {
                DiContainerReference.Container = _container;
            }
        }
    }
}
