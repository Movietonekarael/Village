using UnityEngine;
using UnityEditor;


namespace GameCore
{
    namespace GUI
    {
        public sealed class QuitApplicationButtonAction : MenuButtonAction
        {
            public QuitApplicationButtonAction(MenuButtonAction nextAction = null) : base(nextAction)
            {

            }

            protected override void Execute()
            {
                QuitApplication();
            }

            private void QuitApplication()
            {
                Application.Quit();
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif
            }
        }
    }
}