using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using GameCore.Network;


namespace GameCore
{
    namespace Boot
    {
        public class MenuSceneLoader : MonoBehaviour
        {
            [SerializeField] private AssetReference _testScene;

            public void Host()
            {
                NetworkConnectionService.ConnectionType = ConnectionType.Host;
                LoadTestLevel();
            }

            public void ConnectHost()
            {
                NetworkConnectionService.ConnectionType = ConnectionType.Cliet;
                LoadTestLevel();
            }

            private void LoadTestLevel()
            {
                Addressables.LoadSceneAsync(_testScene, UnityEngine.SceneManagement.LoadSceneMode.Single, true);
            }

            public void Exit()
            {
                Application.Quit();
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif
            }
        }
    }
}