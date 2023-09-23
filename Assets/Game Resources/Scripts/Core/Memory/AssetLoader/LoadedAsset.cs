using System;
using UnityEngine;

namespace GameCore
{
    namespace Memory
    {
        public sealed class LoadedAsset : MonoBehaviour
        {
            public AssetLoader.IAssetLoaderBehaviour MasterBehaviour;
            public UnityEngine.Object AssetInstance;
            public event Action<AssetLoader.IAssetLoaderBehaviour, LoadedAsset> OnDestraction;


            private void OnDestroy()
            {
                OnDestraction?.Invoke(MasterBehaviour, this);
            }
        }
    }
}