using System;

namespace GameCore
{
    namespace Memory
    {
        public static partial class AssetLoader
        {
            public interface IAssetLoaderBehaviour
            {
                public event Action<IAssetLoaderBehaviour> OnDestaction;
            }
        }
    }
}