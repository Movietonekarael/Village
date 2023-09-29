using GameCore.Inventory;
using Zenject;

namespace GameCore
{
    namespace Installers
    {
        public sealed class PlayerHoldItemWrapperInstaller : MonoInstaller
        {
            public override void InstallBindings()
            {
                Container.Bind<IPlayerHoldItem>()
                         .To<PlayerHoldItemWrapper>()
                         .FromNew()
                         .AsCached()
                         .NonLazy();
            }
        }
    }
}