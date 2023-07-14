using Zenject;

namespace GameCore
{
    public static class DiContainerReference
    {
        private static DiContainer _container;

        public static DiContainer Container
        {
            get => _container;
            set => _container ??= value;
        }

    }
}
