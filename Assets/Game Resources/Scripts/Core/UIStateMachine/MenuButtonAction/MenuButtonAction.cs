namespace GameCore
{
    namespace GUI
    {
        public abstract class MenuButtonAction
        {
            private readonly MenuButtonAction _nextAction;

            public MenuButtonAction(MenuButtonAction nextAction = null)
            {
                _nextAction = nextAction;
            }

            public void HandleAction()
            {
                Execute();
                _nextAction?.HandleAction();
            }

            protected abstract void Execute();
        }
    }
}