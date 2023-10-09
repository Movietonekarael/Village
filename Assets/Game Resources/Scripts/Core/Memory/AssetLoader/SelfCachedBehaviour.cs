using System;
using UnityEngine;
using static GameCore.Memory.AssetLoader;

namespace GameCore
{
    namespace Memory
    {
        public sealed class SelfCachedBehaviour : MonoBehaviour, IAssetLoaderBehaviour
        {
            public event Action<IAssetLoaderBehaviour> OnDestaction;


            private void OnDestroy()
            {
                OnDestaction?.Invoke(this);
            }
        }
    }
}