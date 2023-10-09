using GameCore.GUI;
using Zenject;


namespace GameCore
{
    namespace Installers
    {
        public static class UIElementInstaller
        {
            public static void BindUiElement<T, I, P>(DiContainer diContainer) where T : I, P, IDeinitializable, IActivatable, new()
                                                                               where I : ISpecific
            {
                var instance = CreateInstance<T>(diContainer);
                BindInterface<T, I>(diContainer, instance);
                BindInterface<T, P>(diContainer, instance);
                BindDeinitialization<T, I>(diContainer, instance);
                BindActivation<T, I>(diContainer, instance);
            }

            private static T CreateInstance<T>(DiContainer diContainer) where T : new()
            {
                var instance = new T();
                diContainer.Inject(instance);
                return instance;
            }

            private static void BindInterface<T, I>(DiContainer diContainer, T instance) where T : I
            {
                diContainer.Bind<I>().
                            To<T>().
                            FromInstance(instance).
                            AsCached().
                            NonLazy();
            }

            private static void BindDeinitialization<T, I>(DiContainer diContainer, T instance) where T : IDeinitializable
            {
                diContainer.Bind<IDeinitializable>().
                            To<T>().
                            FromInstance(instance).
                            AsCached().
                            NonLazy();
            }

            private static void BindActivation<T, I>(DiContainer diContainer, T instance) where T : IActivatable
            {
                diContainer.Bind<IActivatable>().
                            To<T>().
                            FromInstance(instance).
                            AsCached().
                            NonLazy();
            }
        }
    }
}