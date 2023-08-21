using System;


namespace GameCore
{
    namespace GUI
    {
        [Serializable]
        public class MenuButtonActionCreator
        {
            public enum ActionType
            {
                ButtonPress = 0,
                LoadScene = 1,
                SetConnectionType = 2,
                QuitApplication = 3
            }

            public static readonly Type[] actionsList =
            {
                typeof(ButtonPressedActionCreator),
                typeof(LoadSceneButtonActionCreator),
                typeof(SetConnectionTypeButtonActionCreator),
                typeof(QuitApplicationButtonActionCreator)
            };

            public ActionType LastActionType;
            public ActionType CurrentActionType;

            public MenuButtonActionCreator()
            {
                CurrentActionType = ActionType.ButtonPress;
                LastActionType = CurrentActionType;
            }

            public virtual MenuButtonAction Create(MenuButtonAction nextAction = null)
            {
                return null;
            }
        }
    }
}