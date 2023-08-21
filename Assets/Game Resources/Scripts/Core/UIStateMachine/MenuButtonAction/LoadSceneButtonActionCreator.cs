using System;


namespace GameCore
{
    namespace GUI
    {
        [Serializable]
        public sealed class LoadSceneButtonActionCreator : MenuButtonActionCreator
        {
            public string LoadSceneName;

            public override MenuButtonAction Create(MenuButtonAction nextAction = null)
            {
                return new LoadSceneButtonAction(LoadSceneName, nextAction);
            }
        }
    }
}