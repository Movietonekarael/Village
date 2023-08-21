using UnityEngine.AddressableAssets;


namespace GameCore
{
    namespace GUI
    {
        public sealed class LoadSceneButtonAction : MenuButtonAction
        {
            private readonly string _sceneName;

            public LoadSceneButtonAction(string sceneName, MenuButtonAction nextAction = null) : base(nextAction)
            {
                _sceneName = sceneName;
            }

            protected override void Execute()
            {
                LoadScene();
            }

            private void LoadScene()
            {
                Addressables.LoadSceneAsync(_sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single, true);
            }
        }
    }
}