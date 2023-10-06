using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceProviders;
using Unity.Collections;
using System;
using System.Linq;
using GameCore.Network;

namespace GameCore
{
    namespace SceneManagement
    {

        public sealed partial class AddressablesSceneManager : DefaultNetworkBehaviour
        {
            public HashSet<string> LoadedScenesNames = new();
            private HashSet<AssetReference> LoadedScenesReferences = new();

            public static event Action<ulong> OnClientSceneManagerSpawned;
            public static event Action OnSceneLoadedAndActivated;


            private void Awake()
            {
                DontDestroyOnLoad(gameObject);
            }

            protected override void OnClientNetworkSpawn()
            {
                SceneManagerSpawnedServerRpc(NetworkManager.Singleton.LocalClientId);
                if (_singleton == null)
                    _singleton = this;
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
                    LoadedScenesNames.Add(loadHandle.Result.Scene.name);
                    LoadedScenesReferences.Add(sceneReference);
                    SendLoadSceneRpc(sceneReference);
                    OnSceneLoadedAndActivated?.Invoke();
                }
            }

            private async void LoadSceneClientAsync(AssetReference sceneReference, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
            {
                var loadHandle = Addressables.LoadSceneAsync(sceneReference, loadSceneMode, false);
                await loadHandle.Task;

                if (loadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    ActivateSceneAsync(loadHandle, false);
                    LoadedScenesNames.Add(loadHandle.Result.Scene.name);
                    LoadedScenesReferences.Add(sceneReference);
                    OnSceneLoadedAndActivated?.Invoke();
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

                var scenesReferences = LoadedScenesReferences.ToArray();
                for (var i = 0; i < scenesReferences.Length; i++)
                {
                    var guid = GetGuid(scenesReferences[i]);
                    LoadSceneClientRpc(guid,
                                       i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive,
                                       clientRpcParams);
                }
            }

            public void UnloadAll()
            {
                LoadedScenesNames.Clear();
                LoadedScenesReferences.Clear();
            }
        }
    }
}