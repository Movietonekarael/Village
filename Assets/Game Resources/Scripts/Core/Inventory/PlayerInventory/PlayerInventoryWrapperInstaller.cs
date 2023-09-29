using GameCore.Inventory;
using Zenject;

namespace GameCore
{
    namespace Installers
    {
        public sealed class PlayerInventoryWrapperInstaller : MonoInstaller
        {
            public override void InstallBindings()
            {
                var playerInventoryWrapper = new PlayerInventoryWrapper();

                Container.Bind<PlayerInventoryWrapper>()
                         .FromInstance(playerInventoryWrapper)
                         .AsCached()
                         .NonLazy();
                Container.Bind<IInventory>()
                         .To<PlayerInventoryWrapper>()
                         .FromInstance(playerInventoryWrapper)
                         .AsCached()
                         .NonLazy();
                Container.Bind<IMovableInventory>()
                         .To<PlayerInventoryWrapper>()
                         .FromInstance(playerInventoryWrapper)
                         .AsCached()
                         .NonLazy();
                Container.Bind<IDropableInventory>()
                         .To<PlayerInventoryWrapper>()
                         .FromInstance(playerInventoryWrapper)
                         .AsCached()
                         .NonLazy();
            }
        }
    }
}