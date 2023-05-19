using Zenject;


namespace GameCore.Installers
{
    public static class UIElementInstaller
    {
        public static void BindUiElement<T, I>(DiContainer diContainer) where T : I, IDeinitializable, IActivatable, new()
        {
            var instance = CreateInstance<T>(diContainer);
            BindInitialization<T, I>(diContainer, instance);
            BindDeinitialization(diContainer, instance);
            BindActivation(diContainer, instance);
        }

        private static T CreateInstance<T>(DiContainer diContainer) where T : new()
        {
            var instance = new T();
            diContainer.Inject(instance);
            return instance;
        }

        private static void BindInitialization<T, I>(DiContainer diContainer, T instance) where T : I
        {
            diContainer.Bind<I>().
                        To<T>().
                        FromInstance(instance).
                        AsCached().
                        NonLazy();
        }

        private static void BindDeinitialization<T>(DiContainer diContainer, T instance) where T : IDeinitializable
        {
            diContainer.Bind<IDeinitializable>().
                        WithId(typeof(T)).
                        To<T>().
                        FromInstance(instance).
                        AsCached().
                        NonLazy();
        }

        private static void BindActivation<T>(DiContainer diContainer, T instance) where T : IActivatable
        {
            diContainer.Bind<IActivatable>().
                        WithId(typeof(T)).
                        To<T>().
                        FromInstance(instance).
                        AsCached().
                        NonLazy();
        }
    }
}