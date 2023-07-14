using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace GameCore
{
    namespace Boot
    {
        public class MenuSceneLoader : MonoBehaviour
        {
            [SerializeField] private AssetReference _testScene;

            public void LoadTestLevel()
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