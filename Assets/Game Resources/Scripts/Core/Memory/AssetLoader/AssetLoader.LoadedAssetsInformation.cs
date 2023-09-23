using GameCore.Services;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameCore
{
    namespace Memory
    {
        public static partial class AssetLoader
        {
            private struct LoadedAssetsInformation<T> : ILoadedAssetsInformation where T : UnityEngine.Object
            {
                public T AssetInstance;
                public AsyncOperationHandle<T> OperationHandle;


                public LoadedAssetsInformation(T assetInstance, AsyncOperationHandle<T> operationHandle)
                {
                    AssetInstance = assetInstance;
                    OperationHandle = operationHandle;
                }

                public readonly void DestroyAssetInstance()
                {
                    if (AssetInstance is GameObject)
                    {
                        (AssetInstance as GameObject).GetComponent<LoadedAsset>().OnDestraction -= ReleaseDestroyedLoadedAsset;
                    }
                    InstantiateService.Singleton.DestroyObject(AssetInstance);
                }

                public readonly void ReleaseAsset()
                {
                    Addressables.Release(OperationHandle);
                }

                public readonly UnityEngine.Object GetAssetInstance()
                {
                    return AssetInstance;
                }
            }
        }
    }
}