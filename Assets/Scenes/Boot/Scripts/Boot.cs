using UnityEngine;
using UnityEngine.AddressableAssets;


namespace GameCore
{
    namespace Boot
    {
        public class Boot : MonoBehaviour
        {
            [SerializeField] private AssetReference _firstScene;

            private void Start()
            {
                Addressables.LoadSceneAsync(_firstScene, UnityEngine.SceneManagement.LoadSceneMode.Single, true);
            }
        }
    }
}