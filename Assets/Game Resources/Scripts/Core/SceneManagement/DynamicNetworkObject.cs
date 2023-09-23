using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using GameCore.Memory;


namespace SceneManagement
{
    public sealed class DynamicNetworkObject : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject _networkAssetReference;

        private void Start()
        {
            HandleNetworkSpawning();
        }

        private void HandleNetworkSpawning()
        {
            if (NetworkHasStarted())
            {
                SpawnNetworkObject();
            }
        }

        private bool NetworkHasStarted()
        {
            return NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient;
        }

        private async void SpawnNetworkObject()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var newInstance = await AssetLoader.InstantiateAssetSelfCached(_networkAssetReference, transform, transform.position, transform.rotation);
                SpawnNetworkForGameObject(newInstance);
            }
            
            Destroy(gameObject);
        }

        private void SpawnNetworkForGameObject(GameObject gameObject)
        {
            gameObject.GetComponent<NetworkObject>().Spawn(false);
            AddressablesSceneManager.Singleton.AddNetworkObject(gameObject);
        }
    }
}