namespace GameCore
{
    namespace GUI
    {
        namespace Extensions
        {
            public static class MenuButtonActionCreatorExtension
            {
                public static MenuButtonAction GetAction(this MenuButtonActionCreator[] actionCreator)
                {
                    MenuButtonAction lastAction = null;
                    for (var i = actionCreator.Length - 1; i >= 0; i--)
                    {
                        var currentAction = actionCreator[i].Create(lastAction);
                        lastAction = currentAction;
                    }

                    return lastAction;
                }
            }
        }
    }
}