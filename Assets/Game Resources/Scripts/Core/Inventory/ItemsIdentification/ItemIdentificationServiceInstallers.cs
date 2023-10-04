using GameCore.Inventory;
using Zenject;

namespace GameCore
{
    namespace Installers
    {
        public sealed class ItemIdentificationServiceInstallers : MonoInstaller
        {
            public override void InstallBindings()
            {
                Container.Bind<ItemIdentificationService>()
                         .FromNew()
                         .AsSingle()
                         .NonLazy();
            }
        }
    }
}