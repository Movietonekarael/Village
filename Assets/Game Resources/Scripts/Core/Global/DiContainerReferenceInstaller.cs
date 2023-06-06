using UnityEngine;
using Zenject;

namespace GameCore
{
    [CreateAssetMenu(fileName = "DiContainerReferenceInstaller", menuName = "Installers/DiContainerReferenceInstaller")]
    public class DiContainerReferenceInstaller : ScriptableObjectInstaller<DiContainerReferenceInstaller>
    {
        [Inject] private readonly DiContainer _container;

        public override void InstallBindings()
        {
            DiContainerReference.Container = _container;
        }
    }
}
