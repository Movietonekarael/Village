using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace GameCore
{
    namespace SceneManagement
    {
        public sealed partial class AddressablesSceneManager
        {
            private const string _PREFAB_NAME = "AddressablesSceneManager";

            private static AddressablesSceneManager _singleton = null;
            public static AddressablesSceneManager Singleton => _singleton;


            public static async Task CreateInstance()
            {
                if (_singleton == null)
                {
                    var loadHandle = Addressables.LoadAssetAsync<GameObject>(_PREFAB_NAME);
                    await loadHandle.Task;

                    var prefab = loadHandle.Result;
                    var prefabInstance = Instantiate(prefab);
                    prefabInstance.name = prefab.name;
                    var networkObject = prefabInstance.GetComponent<NetworkObject>();
                    networkObject.Spawn(false);
                    _singleton = prefabInstance.GetComponent<AddressablesSceneManager>();
                }
                else
                {
                    Debug.LogWarning("Instance already created. Quiting creation.");
                    return;
                }
            }
        }
    }
}