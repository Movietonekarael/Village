using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace SceneManagement
{
    [RequireComponent(typeof(NetworkObject))]
    public sealed class DynamicNetworkObject : NetworkBehaviour
    {
        private struct TransformWrapper
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;

            public TransformWrapper(Transform transform)
            {
                Position = transform.position;
                Rotation = transform.rotation;
                Scale = transform.localScale;
            }

            public readonly void SetTransform(Transform transform)
            {
                transform.position = Position;
                transform.rotation = Rotation;
                transform.localScale = Scale;
            }
        }

        //public AssetReferenceGameObject PrefabReference;
        //[HideInInspector] public AsyncOperationHandle<GameObject>? LoadHandle;

        private void Start()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                GetComponent<NetworkObject>().Spawn(false);
                AddressablesSceneManager.Singleton.AddNetworkObject(gameObject);
                DontDestroyOnLoad(gameObject);
            }
            else if (!IsSpawned)
            {
                Destroy(gameObject);
            }  
        }
        /*
        public async void RespawnPrefab()
        {
            var loadHandle = Addressables.LoadAssetAsync<GameObject>(PrefabReference);
            await loadHandle.Task;

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                var prefab = loadHandle.Result;
                var parent = this.transform.parent;
                var prefabInstance = Instantiate(prefab, parent);

                var originalTransform = new TransformWrapper(this.transform);
                originalTransform.SetTransform(prefabInstance.transform);
                prefabInstance.GetComponent<DynamicNetworkObject>().LoadHandle = loadHandle;
                prefabInstance.GetComponent<NetworkObject>().Spawn();
            }
            Destroy(this.gameObject);
        }*/
        /*
        public override void OnDestroy()
        {
            base.OnDestroy();

            if (LoadHandle != null)
            {
                Addressables.Release(LoadHandle.Value);
            }
        }*/
    }
}