using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceProviders;
using Unity.Collections;
using System;
using System.Linq;


namespace SceneManagement
{
    public sealed partial class AddressablesSceneManager : NetworkBehaviour
    {
        private readonly HashSet<AssetReference> _loadedScenes = new();

        public static event Action<ulong> OnClientSceneManagerSpawned;


        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                SceneManagerSpawnedServerRpc(NetworkManager.Singleton.LocalClientId);
            }

            base.OnNetworkSpawn();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SceneManagerSpawnedServerRpc(ulong clientId)
        {
            OnClientSceneManagerSpawned?.Invoke(clientId);
        }

        [ClientRpc]
        private void LoadSceneClientRpc(FixedString128Bytes sceneGuid, LoadSceneMode loadSceneMode = LoadSceneMode.Single, ClientRpcParams clientRpcParams = default)
        {
            if (IsServer) return;

            AssetReference sceneReference = new(sceneGuid.ToString());
            LoadSceneClientAsync(sceneReference, loadSceneMode);
        }

        public async Task LoadSceneServerAsync(AssetReference sceneReference, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            var loadHandle = Addressables.LoadSceneAsync(sceneReference, loadSceneMode, false);
            await loadHandle.Task;

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                ActivateSceneAsync(loadHandle);
                _loadedScenes.Add(sceneReference);
                SendLoadSceneRpc(sceneReference);
            }
        }

        private async void LoadSceneClientAsync(AssetReference sceneReference, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            var loadHandle = Addressables.LoadSceneAsync(sceneReference, loadSceneMode, false);
            await loadHandle.Task;

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                ActivateSceneAsync(loadHandle, false);
                _loadedScenes.Add(sceneReference);
            }
        }

        private void ActivateSceneAsync(AsyncOperationHandle<SceneInstance> sceneHandle)
        {
            var sceneInstance = sceneHandle.Result;
            sceneInstance.ActivateAsync();
        }

        private void ActivateSceneAsync(AsyncOperationHandle<SceneInstance> sceneHandle, bool activate)
        {
            var sceneInstance = sceneHandle.Result;
            sceneInstance.ActivateAsync();
        }

        private void SendLoadSceneRpc(AssetReference sceneReference)
        {
            if (IsServer)
            {
                var guid = GetGuid(sceneReference);
                LoadSceneClientRpc(guid);
            }
        }

        private FixedString128Bytes GetGuid(AssetReference sceneReference)
        {
            var guid = sceneReference.AssetGUID;
            var fixedGuid = new FixedString128Bytes(guid.ToString());
            return fixedGuid;
        }

        public void SynchronizeScenes(ulong clientId)
        {
            if (!IsServer) return;

            ClientRpcParams clientRpcParams = new()
            {
                Send = new()
                {
                    TargetClientIds = new[] { clientId }
                }
            };

            var scenesReferences = _loadedScenes.ToArray();
            for (var i = 0; i < scenesReferences.Length; i++) 
            {
                var guid = GetGuid(scenesReferences[i]);
                LoadSceneClientRpc(guid, 
                                   i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive,  
                                   clientRpcParams);
            }
        }
    }
}