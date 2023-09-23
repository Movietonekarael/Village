namespace GameCore
{
    namespace Memory
    {
        public static partial class AssetLoader
        {
            private interface ILoadedAssetsInformation
            {
                public void DestroyAssetInstance();
                public void ReleaseAsset();
                public UnityEngine.Object GetAssetInstance();
            }
        }
    }
}