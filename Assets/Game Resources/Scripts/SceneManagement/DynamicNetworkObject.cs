using Unity.Netcode;
using UnityEngine;


namespace SceneManagement
{
    public sealed partial class DynamicNetworkObject : MonoBehaviour
    {
        [SerializeField] private GameObject _networkPrefab;

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

        private void SpawnNetworkObject()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var newInstance = Instantiate(_networkPrefab, transform.position, transform.rotation, transform.parent);
                SpawnNetworkForGameObject(newInstance);
            }
            
            Destroy(gameObject);
        }

        private void SpawnNetworkForGameObject(GameObject gameObject)
        {
            gameObject.GetComponent<NetworkObject>().Spawn(false);
            AddressablesSceneManager.Singleton.AddNetworkObject(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }
}