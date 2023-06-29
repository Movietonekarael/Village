using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameCore.Boot
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

