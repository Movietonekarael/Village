using GameCore.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace GameCore
{
    namespace Memory
    {
        public static partial class AssetLoader
        {
            private static readonly Dictionary<IAssetLoaderBehaviour, LinkedList<ILoadedAssetsInformation>> _loadBehaviours = new();


            public static async Task<GameObject> InstantiateAssetSelfCached(AssetReferenceGameObject assetReference, 
                                                                            Vector3 position, 
                                                                            Quaternion rotation,
                                                                            Transform parent = null)
            {
                var handle = await WaitUntilAddressableAssetLoaded<GameObject>(assetReference);
                var instance = InstantiateLoadedAsset(ref handle, parent, position, rotation);
                var behaviour = instance.AddComponent<SellCachedBehaviour>() as IAssetLoaderBehaviour;
                AddLoadedAssetComponentIfInstanceIsGameObject(instance, behaviour);
                CacheInstance(instance, behaviour, ref handle);
                return instance;
            }

            public static async Task<T> InstantiateAssetCached<T>(IAssetLoaderBehaviour behaviour, AssetReference assetReference) where T : UnityEngine.Object
            {
                var handle = await WaitUntilAddressableAssetLoaded<T>(assetReference);
                var instance = InstantiateLoadedAsset(ref handle, null, Vector3.zero, Quaternion.identity);
                AddLoadedAssetComponentIfInstanceIsGameObject(instance, behaviour);
                CacheInstance(instance, behaviour, ref handle);
                return instance;
            }

            private static async Task<AsyncOperationHandle<P>> WaitUntilAddressableAssetLoaded<P>(AssetReference assetReference) where P : UnityEngine.Object
            {
                var handle = Addressables.LoadAssetAsync<P>(assetReference);
                await handle.Task;
                return handle;
            }

            private static P InstantiateLoadedAsset<P>(ref AsyncOperationHandle<P> handle, Transform parent, Vector3 position, Quaternion rotation) where P : UnityEngine.Object
            {
                var prefab = handle.Result;
                var instance = InstantiateService.Singleton.InstantiateObject(prefab, position, rotation, parent);
                instance.name = prefab.name;
                return instance;
            }

            private static void AddLoadedAssetComponentIfInstanceIsGameObject<P>(P instance, IAssetLoaderBehaviour behaviour) where P : UnityEngine.Object
            {
                if (instance is GameObject)
                {
                    var loadedAssetMono = (instance as GameObject).AddComponent<LoadedAsset>();
                    loadedAssetMono.AssetInstance = instance;
                    loadedAssetMono.MasterBehaviour = behaviour;
                    loadedAssetMono.OnDestraction += ReleaseDestroyedLoadedAsset;
                }
            }

            private static void CacheInstance<P>(P instance, IAssetLoaderBehaviour behaviour, ref AsyncOperationHandle<P> handle) where P : UnityEngine.Object
            {
                if (_loadBehaviours.ContainsKey(behaviour))
                {
                    AddInstanceToExistingKey(instance, behaviour, ref handle);
                }
                else
                {
                    CreateNewKeyAndAddInstanceToIt(instance, behaviour, ref handle);
                }
            }

            private static void AddInstanceToExistingKey<P>(P instance, IAssetLoaderBehaviour behaviour, ref AsyncOperationHandle<P> handle) where P : UnityEngine.Object
            {
                var loadedAssets = _loadBehaviours[behaviour];
                loadedAssets.AddLast(new LoadedAssetsInformation<P>(instance, handle));
            }

            private static void CreateNewKeyAndAddInstanceToIt<P>(P instance, IAssetLoaderBehaviour behaviour, ref AsyncOperationHandle<P> handle) where P : UnityEngine.Object
            {
                var loadedAssets = new LinkedList<ILoadedAssetsInformation>();
                loadedAssets.AddLast(new LoadedAssetsInformation<P>(instance, handle));
                _loadBehaviours.Add(behaviour, loadedAssets);
                behaviour.OnDestaction += ReleaseDestroyedBehaviourLoadedAssets;
            }

            public static void ReleaseDestroyedBehaviourLoadedAssets(IAssetLoaderBehaviour behaviour)
            {
                UnsubscribeLoaderBehaviour(behaviour);
                DestroyAllInstantiatedAssetsForKeyAndReleaseThemFromMemory(behaviour);
                RemoveKey(behaviour);


                void UnsubscribeLoaderBehaviour(IAssetLoaderBehaviour behaviour)
                {
                    behaviour.OnDestaction -= ReleaseDestroyedBehaviourLoadedAssets;
                }

                void DestroyAllInstantiatedAssetsForKeyAndReleaseThemFromMemory(IAssetLoaderBehaviour behaviour)
                {
                    var loadedAssets = _loadBehaviours[behaviour];
                    foreach (var loadedAsset in loadedAssets)
                    {
                        loadedAsset.DestroyAssetInstance();
                        loadedAsset.ReleaseAsset();
                    }
                }

                void RemoveKey(IAssetLoaderBehaviour behaviour)
                {
                    _loadBehaviours.Remove(behaviour);
                }
            }

            public static void ReleaseDestroyedLoadedAsset(IAssetLoaderBehaviour behaviour, LoadedAsset loadedAssetMono)
            {
                UnsubscribeLoadedAsset(loadedAssetMono);
                RemoveLoadedAssetFromDictionaryKey(behaviour, loadedAssetMono, out var loadedAssets);
                RemoveKeyIfItHasNoAssets(behaviour, loadedAssets);


                void UnsubscribeLoadedAsset(LoadedAsset loadedAssetMono)
                {
                    loadedAssetMono.OnDestraction -= ReleaseDestroyedLoadedAsset;
                }

                void RemoveLoadedAssetFromDictionaryKey(IAssetLoaderBehaviour behaviour, LoadedAsset loadedAssetMono, out LinkedList<ILoadedAssetsInformation> loadedAssets)
                {
                    loadedAssets = _loadBehaviours[behaviour];
                    var loadedAssetNode = loadedAssets.First;

                    while (loadedAssetNode is not null)
                    {
                        var loadedAsset = loadedAssetNode.Value;
                        if (loadedAsset.GetAssetInstance() == loadedAssetMono.AssetInstance)
                        {
                            loadedAsset.ReleaseAsset();
                            loadedAssets.Remove(loadedAssetNode);
                            break;
                        }
                        loadedAssetNode = loadedAssetNode.Next;
                    }
                }

                void RemoveKeyIfItHasNoAssets(IAssetLoaderBehaviour behaviour, LinkedList<ILoadedAssetsInformation> loadedAssets)
                {
                    if (loadedAssets.Count == 0)
                    {
                        behaviour.OnDestaction -= ReleaseDestroyedBehaviourLoadedAssets;
                        _loadBehaviours.Remove(behaviour);

                    }
                }
            }
        }
    }
}