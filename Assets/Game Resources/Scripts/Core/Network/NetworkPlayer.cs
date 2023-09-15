using GameCore.GameControls;
using GameCore.GameMovement;
using GameCore.Services;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace GameCore
{
    namespace Network
    {
        public sealed class NetworkPlayer : NetworkBehaviour
        {
            private void Awake()
            {   
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }
}